using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationLayer.Common.Utilities;
using ApplicationLayer.Dto.Signals;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Entities;
using InfrastructureLayer.BusinessLogic.Services.Signals;
using Moq;
using Xunit;

namespace Kavan.Tests
{
    public class SignalAnalysisServiceTests
    {
        private static ISignalAnalysisService CreateService(
            IQueryable<Cryptocurrency> cryptos,
            IQueryable<Candle_1h> c1h,
            IConditionEvaluatorFactory factory,
            SignalThresholds? thresholds = null)
        {
            var mCrypto = new Mock<IRepository<Cryptocurrency>>();
            mCrypto.Setup(r => r.Query()).Returns(cryptos);

            var m1h = new Mock<IRepository<Candle_1h>>();
            m1h.Setup(r => r.Query()).Returns(c1h);

            // Unused repos in these tests
            var m1m = new Mock<IRepository<Candle_1m>>();
            m1m.Setup(r => r.Query()).Returns(Enumerable.Empty<Candle_1m>().AsQueryable());
            var m5m = new Mock<IRepository<Candle_5m>>();
            m5m.Setup(r => r.Query()).Returns(Enumerable.Empty<Candle_5m>().AsQueryable());
            var m4h = new Mock<IRepository<Candle_4h>>();
            m4h.Setup(r => r.Query()).Returns(Enumerable.Empty<Candle_4h>().AsQueryable());
            var m1d = new Mock<IRepository<Candle_1d>>();
            m1d.Setup(r => r.Query()).Returns(Enumerable.Empty<Candle_1d>().AsQueryable());

            return new SignalAnalysisService(
                mCrypto.Object,
                m1m.Object,
                m5m.Object,
                m1h.Object,
                m4h.Object,
                m1d.Object,
                factory,
                thresholds);
        }

        private class StubEvaluator : IConditionEvaluator
        {
            public string Type => "stub";
            public Task<ConditionEvaluationResult> EvaluateAsync(string symbol, string timeframe, ConditionNodeDto condition, CancellationToken ct)
                => Task.FromResult(new ConditionEvaluationResult
                {
                    Matched = true,
                    ScoreContribution = 10, // will be scaled by service to 100
                    Explanation = "stub matched",
                    MatchedConditions = new List<string> { "stub" }
                });
        }

        private class StubFactory : IConditionEvaluatorFactory
        {
            public IConditionEvaluator? Resolve(string type)
                => type == "stub" ? new StubEvaluator() : null;
        }

        [Fact]
        public async Task AnalyzeAsync_ProducesStrongSignal_WhenStubMatches()
        {
            var cryptos = new List<Cryptocurrency> { new() { Id = 1, Symbol = "BTCUSDT" } }.AsQueryable();
            var candles = Enumerable.Range(0, 100).Select(i => new Candle_1h
            {
                Id = i + 1,
                CryptocurrencyId = 1,
                OpenTime = DateTime.UtcNow.AddHours(-i - 1),
                CloseTime = DateTime.UtcNow.AddHours(-i),
                Open = 50000m,
                High = 50100m,
                Low = 49900m,
                Close = 50050m,
                Volume = 1000m
            }).AsQueryable();

            var service = CreateService(cryptos, candles, new StubFactory(), new SignalThresholds());

            var request = new SignalRequestDto
            {
                Market = "crypto",
                Symbols = new List<string> { "BTCUSDT" },
                Timeframes = new List<string> { "1h" },
                Conditions = new List<ConditionNodeDto> { new() { Type = "stub", Description = "stub condition" } },
                Groups = new List<GroupNodeDto>()
            };

            var results = await service.AnalyzeAsync(request, CancellationToken.None);
            var res = Assert.Single(results);
            Assert.Equal("BTCUSDT", res.Symbol);
            Assert.Equal("1h", res.TimeFrame);
            Assert.True(res.Confidence >= 90); // strong mapped from score ~100
            Assert.Equal("Buy", res.SignalType); // heuristic may produce Buy for stub matched text
        }

        [Fact]
        public async Task AnalyzeAsync_RejectsByVolumeFilter_WhenAverageVolumeBelowMin()
        {
            var cryptos = new List<Cryptocurrency> { new() { Id = 1, Symbol = "BTCUSDT" } }.AsQueryable();
            var lowVolCandles = Enumerable.Range(0, 50).Select(i => new Candle_1h
            {
                Id = i + 1,
                CryptocurrencyId = 1,
                OpenTime = DateTime.UtcNow.AddHours(-i - 1),
                CloseTime = DateTime.UtcNow.AddHours(-i),
                Open = 50000m,
                High = 50100m,
                Low = 49900m,
                Close = 50050m,
                Volume = 1m // deliberately tiny
            }).AsQueryable();

            var service = CreateService(cryptos, lowVolCandles, new StubFactory(), new SignalThresholds());

            var request = new SignalRequestDto
            {
                Market = "crypto",
                Symbols = new List<string> { "BTCUSDT" },
                Timeframes = new List<string> { "1h" },
                Filters = new FilterOptionsDto { ApplyBefore = true, Volume_Min = 100m },
                Conditions = new List<ConditionNodeDto> { new() { Type = "stub" } },
                Groups = new List<GroupNodeDto>()
            };

            var results = await service.AnalyzeAsync(request, CancellationToken.None);
            var res = Assert.Single(results);
            Assert.Equal("Rejected by pre-filters", res.Explanation);
            Assert.Equal(0, res.Confidence);
            Assert.Equal("None", res.SignalType);
        }
    }
}