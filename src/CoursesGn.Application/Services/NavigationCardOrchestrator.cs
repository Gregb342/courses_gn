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
    private readonly ICardRegistry _registry;

    public NavigationCardOrchestrator(
        ICourseGenerator courseGenerator,
        ICardRenderer cardRenderer,
        IFileExporter fileExporter,
        ICardRegistry registry)
    {
        _courseGenerator = courseGenerator;
        _cardRenderer = cardRenderer;
        _fileExporter = fileExporter;
        _registry = registry;
    }

    /// <summary>
    /// Génère toutes les cartes demandées et les exporte dans le répertoire spécifié.
    /// Pour chaque parcours, génère une carte PJ (au niveau de difficulté choisi)
    /// et une carte PNJ (toujours en mode texte facile).
    /// Retourne la liste des chemins de fichiers créés.
    /// </summary>
    public List<string> GenerateCards(GenerationParameters parameters, string outputDirectory)
    {
        var generatedFiles = new List<string>();
        var generatedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Paramètres identiques mais forcés en Level1_Easy pour la carte PNJ
        var pnjParameters = new GenerationParameters
        {
            ArrowCount = parameters.ArrowCount,
            CardCount = parameters.CardCount,
            OutputFormat = parameters.OutputFormat,
            ArrowStyle = parameters.ArrowStyle,
            Difficulty = Difficulty.Level1_Easy
        };

        for (int i = 1; i <= parameters.CardCount; i++)
        {
            int cardNumber = _registry.NextCardNumber();

            Console.WriteLine($"  Génération de la carte {i}/{parameters.CardCount} (n°{cardNumber})...");

            var course = _courseGenerator.Generate(parameters);

            // ── Carte combinée A4 paysage (PNJ gauche + PJ droite) ──
            byte[] combinedData = _cardRenderer.RenderCombined(course, parameters, pnjParameters, cardNumber);

            string combinedPath = _fileExporter.Export(
                combinedData,
                parameters.OutputFormat,
                parameters.Difficulty,
                cardNumber,
                outputDirectory);

            string combinedFileName = Path.GetFileName(combinedPath);
            if (!generatedFileNames.Add(combinedFileName))
                throw new InvalidOperationException(
                    $"Doublon détecté : le fichier '{combinedFileName}' a déjà été généré dans ce lot. " +
                    "Vérifiez le registre des cartes (option 3 pour remettre le compteur à zéro).");

            generatedFiles.Add(combinedPath);
            Console.WriteLine($"    ✓ {combinedFileName}");

            _registry.RecordCard(new CardRegistryEntry
            {
                CardNumber = cardNumber,
                Type = "PJ+PNJ",
                Difficulty = parameters.Difficulty,
                ArrowCount = parameters.ArrowCount,
                ArrowStyle = parameters.ArrowStyle,
                OutputFormat = parameters.OutputFormat,
                GeneratedAt = DateTime.Now,
                FileName = Path.GetFileName(combinedPath)
            });
        }

        return generatedFiles;
    }
}
