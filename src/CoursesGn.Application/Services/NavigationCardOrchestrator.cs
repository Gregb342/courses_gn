using CoursesGn.Domain.Enums;
using CoursesGn.Domain.Interfaces;
using CoursesGn.Domain.Models;

namespace CoursesGn.Application.Services;

/// <summary>
/// Orchestrateur principal : coordonne la génération, le rendu et l'export des cartes.
/// </summary>
public class NavigationCardOrchestrator
{
    private readonly ICourseGenerator _courseGenerator;
    private readonly ICardRenderer _cardRenderer;
    private readonly IFileExporter _fileExporter;

    public NavigationCardOrchestrator(
        ICourseGenerator courseGenerator,
        ICardRenderer cardRenderer,
        IFileExporter fileExporter)
    {
        _courseGenerator = courseGenerator;
        _cardRenderer = cardRenderer;
        _fileExporter = fileExporter;
    }

    /// <summary>
    /// Génère toutes les cartes demandées et les exporte dans le répertoire spécifié.
    /// Retourne la liste des chemins de fichiers créés.
    /// </summary>
    public List<string> GenerateCards(GenerationParameters parameters, string outputDirectory)
    {
        var generatedFiles = new List<string>();

        for (int i = 1; i <= parameters.CardCount; i++)
        {
            Console.WriteLine($"  Génération de la carte {i}/{parameters.CardCount}...");

            var course = _courseGenerator.Generate(parameters);

            byte[] data = _cardRenderer.Render(course, parameters);

            string filePath = _fileExporter.Export(
                data,
                parameters.OutputFormat,
                parameters.Difficulty,
                i,
                outputDirectory);

            generatedFiles.Add(filePath);

            Console.WriteLine($"    ✓ {Path.GetFileName(filePath)}");
        }

        return generatedFiles;
    }
}
