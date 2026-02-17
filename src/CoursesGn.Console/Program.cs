using CoursesGn.Application.Services;
using CoursesGn.Console.Input;
using CoursesGn.Infrastructure.Export;
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

// ── Saisie des paramètres ──
var inputHandler = new UserInputHandler();
var parameters = inputHandler.GetParameters();

// ── Police personnalisée (commenter la ligne pour revenir à la police par défaut) ──
FontProvider.CustomFontFileName = "VoxPopuli.ttf";

// ── Construction des services ──
var courseGenerator = new CourseGeneratorService();
var cardRenderer = new SkiaCardRenderer();
var fileExporter = new FileExporter();

var orchestrator = new NavigationCardOrchestrator(
    courseGenerator,
    cardRenderer,
    fileExporter);

// ── Création d'un répertoire unique par génération ──
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

// ── Résumé avant génération ──
Console.WriteLine();
Console.WriteLine("────────────────────────────────────────────────");
Console.WriteLine($"  Cartes à générer : {parameters.CardCount}");
Console.WriteLine($"  Flèches / carte  : {parameters.ArrowCount}");
Console.WriteLine($"  Format           : {parameters.OutputFormat}");
Console.WriteLine($"  Style            : {styleLabel}");
Console.WriteLine($"  Difficulté       : {difficultyLabel}");
Console.WriteLine($"  Répertoire       : {outputDirectory}");
Console.WriteLine("────────────────────────────────────────────────");
Console.WriteLine();

// ── Génération ──
try
{
    var files = orchestrator.GenerateCards(parameters, outputDirectory);

    Console.WriteLine();
    Console.WriteLine($"  ✅ {files.Count} carte(s) générée(s) avec succès !");
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine($"  ❌ Erreur : {ex.Message}");
    Console.WriteLine();
}
