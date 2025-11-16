namespace ApplicationLayer.Dto.SignalsRetention
{
    public class PurgeOldSignalsResultDto
    {
        public int Deleted_1m { get; set; }
        public int Deleted_5m { get; set; }
        public int Deleted_1h { get; set; }
        public int Deleted_4h { get; set; }
        public int Deleted_1d { get; set; }
        public int Total => Deleted_1m + Deleted_5m + Deleted_1h + Deleted_4h + Deleted_1d;
    }
}