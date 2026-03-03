using CoursesGn.Domain.Enums;

namespace CoursesGn.Domain.Interfaces;

/// <summary>
/// Exporte les données binaires d'une carte vers un fichier sur disque.
/// </summary>
public interface IFileExporter
{
    /// <summary>
    /// Écrit les données dans un fichier et retourne le chemin complet du fichier créé.
    /// </summary>
    /// <param name="isPnj">Si true, ajoute "pnj" dans le nom du fichier.</param>
    string Export(byte[] data, OutputFormat format, Difficulty difficulty, int index, string outputDirectory, bool isPnj = false);
}
