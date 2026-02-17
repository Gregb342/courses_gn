namespace CoursesGn.Domain.Enums;

/// <summary>
/// Niveau de difficulté de la carte de navigation.
/// Chaque niveau modifie la présentation des informations sur la carte.
/// </summary>
public enum Difficulty
{
    /// <summary>
    /// Niveau 1 : Liste textuelle des orientations et couleurs en français.
    /// Pas de carte graphique, juste du texte.
    /// </summary>
    Level1_Easy = 1,

    /// <summary>
    /// Niveau 2 : Flèches colorées + noms de couleurs écrits le long + boussole orientée Nord.
    /// </summary>
    Level2_Normal = 2,

    /// <summary>
    /// Niveau 3 : Flèches colorées, pas de noms de couleurs, boussole orientée aléatoirement.
    /// </summary>
    Level3_Medium = 3,

    /// <summary>
    /// Niveau 4 : Flèches noires, boussole orientée aléatoirement,
    /// couleurs écrites dans un encadré séparé (noms spécifiques plus tard).
    /// </summary>
    Level4_Hard = 4,

    /// <summary>
    /// Niveau 5 : Flèches noires, pas de boussole,
    /// couleurs écrites dans un encadré séparé (noms très spécifiques plus tard).
    /// </summary>
    Level5_Nightmare = 5
}
