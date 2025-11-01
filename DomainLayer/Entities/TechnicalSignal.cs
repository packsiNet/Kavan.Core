using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class TechnicalSignal : BaseEntityModel, IAuditableEntity
{
    public string Symbol { get; set; } = string.Empty;
    
    public string IndicatorCategory { get; set; } = string.Empty;
    
    public string IndicatorName { get; set; } = string.Empty;
    
    public string ConditionTitle { get; set; } = string.Empty;
    
    public SignalType SignalType { get; set; }
    
    /// <summary>
    /// نوع جزئی سیگنال برای دسته‌بندی دقیق‌تر
    /// </summary>
    public DetailedSignalType DetailedSignalType { get; set; }
    
    public string TimeFrame { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public decimal? Value { get; set; } // Optional: for storing indicator values
    
    public string? AdditionalData { get; set; } // Optional: for storing JSON data
}

public enum SignalType
{
    Buy = 1,
    Sell = 2,
    Neutral = 3
}