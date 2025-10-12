using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.Binance;

public interface IBinanceSymbolSyncService
{
    Task<List<string>> GetActiveSymbolsAsync();
}