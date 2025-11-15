namespace ApplicationLayer.Dto.SignalsCatalog
{
    public class SignalCatalogNodeDto
    {
        public int Id { get; set; }
        public string NameFa { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SignalName { get; set; } = string.Empty;
        public List<SignalCatalogNodeDto> Children { get; set; } = new();
    }
}