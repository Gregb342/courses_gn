using CoursesGn.Domain.Models;

namespace CoursesGn.Domain.Interfaces;

/// <summary>
/// Gère le compteur global de cartes et l'historique des générations.
/// </summary>
public interface ICardRegistry
{
    int LastNumber { get; }
    int TotalGenerated { get; }
    int NextCardNumber();
    void RecordCard(CardRegistryEntry entry);
    void Reset();
}
