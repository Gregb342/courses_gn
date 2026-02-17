using CoursesGn.Domain.Models;

namespace CoursesGn.Domain.Interfaces;

/// <summary>
/// Génère un parcours de navigation cohérent.
/// </summary>
public interface ICourseGenerator
{
    /// <summary>
    /// Génère un parcours composé de flèches connectées sans intersection.
    /// </summary>
    NavigationCourse Generate(GenerationParameters parameters);
}
