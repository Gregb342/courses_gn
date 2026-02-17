using CoursesGn.Domain.Enums;

namespace CoursesGn.Domain.Models;

/// <summary>
/// Représente un parcours complet de navigation composé de plusieurs flèches connectées.
/// </summary>
public class NavigationCourse
{
    /// <summary>Liste ordonnée des flèches du parcours.</summary>
    public required List<Arrow> Arrows { get; init; }

    /// <summary>Position de départ du parcours en bas de la page.</summary>
    public required StartPosition StartPosition { get; init; }
}
