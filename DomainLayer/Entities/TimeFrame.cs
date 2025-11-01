using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

/// <summary>
/// Entity for storing timeframe information with dual language support
/// </summary>
public class TimeFrame : BaseEntityModel
{
    /// <summary>
    /// Timeframe code (e.g., "1m", "5m", "1h", "4h", "1d")
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// English name of the timeframe
    /// </summary>
    public string NameEnglish { get; set; } = string.Empty;
    
    /// <summary>
    /// Persian name of the timeframe
    /// </summary>
    public string NamePersian { get; set; } = string.Empty;
    
    /// <summary>
    /// Duration in minutes
    /// </summary>
    public int DurationInMinutes { get; set; }
    
    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Whether this timeframe is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}