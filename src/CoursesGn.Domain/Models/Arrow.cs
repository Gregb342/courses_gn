using CoursesGn.Domain.Enums;

namespace CoursesGn.Domain.Models;

/// <summary>
/// Représente une flèche (étape de manœuvre) dans un parcours de navigation.
/// </summary>
public class Arrow
{
    /// <summary>Numéro de l'étape (1-based).</summary>
    public required int StepNumber { get; init; }

    /// <summary>Direction cardinale ou intercardinale de la flèche.</summary>
    public required Direction Direction { get; init; }

    /// <summary>Couleur de la flèche.</summary>
    public required ArrowColor Color { get; init; }

    /// <summary>Point de départ de la flèche (coordonnées abstraites).</summary>
    public required Point2D Start { get; init; }

    /// <summary>Point d'arrivée de la flèche (coordonnées abstraites).</summary>
    public required Point2D End { get; init; }
}
