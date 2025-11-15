namespace ApplicationLayer.Dto.SignalsCatalog
{
    public class SignalCatalogListItemDto
    {
        public int Id { get; set; }
        public string NameFa { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SignalName { get; set; } = string.Empty;
    }
}