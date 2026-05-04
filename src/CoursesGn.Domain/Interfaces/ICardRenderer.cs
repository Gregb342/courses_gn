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
    /// <param name="cardLabel">Label affiché dans un coin de la carte (ex : "Carte PJ", "Carte PNJ").</param>
    byte[] Render(NavigationCourse course, GenerationParameters parameters, string cardLabel);

    /// <summary>
    /// Génère une carte A4 paysage combinant la moitié PNJ (gauche) et la moitié PJ (droite).
    /// </summary>
    byte[] RenderCombined(NavigationCourse course, GenerationParameters pjParams, GenerationParameters pnjParams, int cardNumber);
}
