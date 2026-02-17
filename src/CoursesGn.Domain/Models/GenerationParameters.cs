using CoursesGn.Domain.Enums;

namespace CoursesGn.Domain.Models;

/// <summary>
/// Paramètres de génération saisis par l'utilisateur.
/// </summary>
public class GenerationParameters
{
    /// <summary>Nombre de flèches par carte (3-50).</summary>
    public required int ArrowCount { get; init; }

    /// <summary>Nombre de cartes à générer (1-100).</summary>
    public required int CardCount { get; init; }

    /// <summary>Format de fichier de sortie.</summary>
    public required OutputFormat OutputFormat { get; init; }

    /// <summary>Style graphique des flèches.</summary>
    public ArrowStyle ArrowStyle { get; init; } = ArrowStyle.HandDrawn;

    /// <summary>Niveau de difficulté.</summary>
    public Difficulty Difficulty { get; init; } = Difficulty.Level2_Normal;
}
