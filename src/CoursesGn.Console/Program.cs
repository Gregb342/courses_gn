using CoursesGn.Application.Services;
using CoursesGn.Console.Input;
using CoursesGn.Infrastructure.Export;
using CoursesGn.Infrastructure.Persistence;
using CoursesGn.Infrastructure.Rendering;

// ══════════════════════════════════════════════════
//  Générateur de Cartes de Navigation
// ══════════════════════════════════════════════════

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine();
Console.WriteLine("╔═══════════════════════════════════════════════╗");
Console.WriteLine("║   🧭  Générateur de Cartes de Navigation  🧭  ║");
Console.WriteLine("╚═══════════════════════════════════════════════╝");
Console.WriteLine();

// ── Police personnalisée ──
FontProvider.CustomFontFileName = "VoxPopuli.ttf";

// ── Construction des services ──
var registry = new CardRegistry();
var inputHandler = new UserInputHandler();
var courseGenerator = new CourseGeneratorService();
var cardRenderer = new SkiaCardRenderer();
var fileExporter = new FileExporter();

var orchestrator = new NavigationCardOrchestrator(
    courseGenerator,
    cardRenderer,
    fileExporter,
    registry);

// ══════════════════════════════════════════════════
//  Boucle principale du menu
// ══════════════════════════════════════════════════

bool running = true;
while (running)
{
    var choice = inputHandler.GetMenuChoice(registry.LastNumber, registry.TotalGenerated);

    switch (choice)
    {
        // ── [1] Générer des cartes ──
        case UserInputHandler.MenuChoice.Generate:
        {
            var parameters = inputHandler.GetParameters();

            string styleName = parameters.ArrowStyle switch
            {
                CoursesGn.Domain.Enums.ArrowStyle.Clean     => "clean",
                CoursesGn.Domain.Enums.ArrowStyle.HandDrawn  => "hand_drawn",
                CoursesGn.Domain.Enums.ArrowStyle.Dotted     => "dotted",
                _ => "style"
            };
            string difficultyName = parameters.Difficulty switch
            {
                CoursesGn.Domain.Enums.Difficulty.Level1_Easy      => "facile",
                CoursesGn.Domain.Enums.Difficulty.Level2_Normal     => "normal",
                CoursesGn.Domain.Enums.Difficulty.Level3_Medium     => "moyen",
                CoursesGn.Domain.Enums.Difficulty.Level4_Hard       => "difficile",
                CoursesGn.Domain.Enums.Difficulty.Level5_Nightmare  => "cauchemar",
                _ => "diff"
            };
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string folderName = $"cartes_{styleName}_{difficultyName}_{timestamp}";
            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            Directory.CreateDirectory(outputDirectory);

            string styleLabel = parameters.ArrowStyle switch
            {
                CoursesGn.Domain.Enums.ArrowStyle.Clean     => "Clean",
                CoursesGn.Domain.Enums.ArrowStyle.HandDrawn  => "Dessiné à la main",
                CoursesGn.Domain.Enums.ArrowStyle.Dotted     => "Pointillé",
                _ => parameters.ArrowStyle.ToString()
            };
            string difficultyLabel = parameters.Difficulty switch
            {
                CoursesGn.Domain.Enums.Difficulty.Level1_Easy      => "1 - Facile",
                CoursesGn.Domain.Enums.Difficulty.Level2_Normal     => "2 - Normal",
                CoursesGn.Domain.Enums.Difficulty.Level3_Medium     => "3 - Moyen",
                CoursesGn.Domain.Enums.Difficulty.Level4_Hard       => "4 - Difficile",
                CoursesGn.Domain.Enums.Difficulty.Level5_Nightmare  => "5 - Cauchemar",
                _ => parameters.Difficulty.ToString()
            };

            Console.WriteLine("────────────────────────────────────────────────");
            Console.WriteLine($"  Cartes à générer : {parameters.CardCount}");
            Console.WriteLine($"  Flèches / carte  : {parameters.ArrowCount}");
            Console.WriteLine($"  Format           : {parameters.OutputFormat}");
            Console.WriteLine($"  Style            : {styleLabel}");
            Console.WriteLine($"  Difficulté       : {difficultyLabel}");
            Console.WriteLine($"  Numéros          : {registry.LastNumber + 1} → {registry.LastNumber + parameters.CardCount}");
            Console.WriteLine($"  Répertoire       : {outputDirectory}");
            Console.WriteLine("────────────────────────────────────────────────");
            Console.WriteLine();

            try
            {
                var files = orchestrator.GenerateCards(parameters, outputDirectory);
                Console.WriteLine();
                Console.WriteLine($"  ✅ {parameters.CardCount} carte(s) générée(s) avec succès ! ({files.Count} fichiers)");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"  ❌ Erreur : {ex.Message}");
            }

            Console.WriteLine();
            break;
        }

        // ── [2] Voir l'historique ──
        case UserInputHandler.MenuChoice.ViewHistory:
        {
            Console.WriteLine("────────────────────────────────────────────────");
            Console.WriteLine($"  Compteur actuel   : {registry.LastNumber}");
            Console.WriteLine($"  Cartes enregistrées : {registry.TotalGenerated}");
            Console.WriteLine($"  Fichier registre  : cartes_registry.json");
            Console.WriteLine($"  Fichier CSV       : cartes_registry.csv");
            Console.WriteLine("────────────────────────────────────────────────");
            Console.WriteLine();
            break;
        }

        // ── [3] Remettre le compteur à zéro ──
        case UserInputHandler.MenuChoice.ResetCounter:
        {
            Console.WriteLine($"  ⚠ Le compteur est actuellement à {registry.LastNumber}.");
            Console.Write("  Confirmer la remise à zéro ? (oui / non) : ");
            string? confirm = Console.ReadLine()?.Trim().ToLowerInvariant();
            Console.WriteLine();

            if (confirm == "oui" || confirm == "o")
            {
                registry.Reset();
                Console.WriteLine("  ✅ Compteur remis à zéro. L'historique est conservé dans le CSV.");
            }
            else
            {
                Console.WriteLine("  Annulé.");
            }

            Console.WriteLine();
            break;
        }

        // ── [0] Quitter ──
        case UserInputHandler.MenuChoice.Quit:
            running = false;
            break;
    }
}

Console.WriteLine("  À bientôt !");
Console.WriteLine();

