using CoursesGn.Domain.Enums;
using CoursesGn.Domain.Models;

namespace CoursesGn.Domain.Interfaces;

/// <summary>
/// Effectue le rendu graphique d'un parcours de navigation.
/// </summary>
public interface ICardRenderer
{
    /// <summary>
    /// Génère le contenu binaire de la carte au format demandé (PDF ou JPG).
    /// </summary>
    byte[] Render(NavigationCourse course, GenerationParameters parameters);
}
