using CoursesGn.Domain.Enums;

namespace CoursesGn.Domain.Models;

/// <summary>
/// Entrée de log enregistrée à chaque carte générée.
/// </summary>
public class CardRegistryEntry
{
    public int CardNumber { get; init; }
    public string Type { get; init; } = string.Empty;       // "PJ" ou "PNJ"
    public Difficulty Difficulty { get; init; }
    public int ArrowCount { get; init; }
    public ArrowStyle ArrowStyle { get; init; }
    public OutputFormat OutputFormat { get; init; }
    public DateTime GeneratedAt { get; init; }
    public string FileName { get; init; } = string.Empty;
}
