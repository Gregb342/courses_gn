using CoursesGn.Domain.Enums;
using CoursesGn.Domain.Models;

namespace CoursesGn.Console.Input;

/// <summary>
/// Gère la saisie interactive des paramètres de génération.
/// Les champs optionnels ont des valeurs par défaut (Entrée vide = défaut).
/// </summary>
public class UserInputHandler
{
    // ── Valeurs par défaut ──
    private const int DefaultArrowCount = 8;
    private const OutputFormat DefaultOutputFormat = OutputFormat.Jpg;
    private const ArrowStyle DefaultArrowStyle = ArrowStyle.HandDrawn;
    private const Difficulty DefaultDifficulty = Difficulty.Level2_Normal;

    /// <summary>
    /// Demande à l'utilisateur tous les paramètres de génération.
    /// </summary>
    public GenerationParameters GetParameters()
    {
        int arrowCount = ReadIntegerWithDefault(
            "Nombre de flèches par carte", min: 3, max: 50, defaultValue: DefaultArrowCount);

        int cardCount = ReadInteger("Nombre de cartes à générer", min: 1, max: 100);

        var outputFormat = ReadOutputFormatWithDefault();
        var arrowStyle = ReadArrowStyleWithDefault();
        var difficulty = ReadDifficultyWithDefault();

        return new GenerationParameters
        {
            ArrowCount = arrowCount,
            CardCount = cardCount,
            OutputFormat = outputFormat,
            ArrowStyle = arrowStyle,
            Difficulty = difficulty
        };
    }

    // ──────────────────────────────────────────────
    //  Lecture d'un entier (obligatoire)
    // ──────────────────────────────────────────────

    private static int ReadInteger(string prompt, int min, int max)
    {
        while (true)
        {
            System.Console.Write($"  {prompt} ({min}-{max}) : ");
            string? input = System.Console.ReadLine()?.Trim();

            if (int.TryParse(input, out int value) && value >= min && value <= max)
                return value;

            System.Console.WriteLine($"    ⚠ Veuillez entrer un nombre entre {min} et {max}.");
        }
    }

    // ──────────────────────────────────────────────
    //  Lecture d'un entier avec défaut
    // ──────────────────────────────────────────────

    private static int ReadIntegerWithDefault(string prompt, int min, int max, int defaultValue)
    {
        while (true)
        {
            System.Console.Write($"  {prompt} ({min}-{max}) [défaut: {defaultValue}] : ");
            string? input = System.Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
                return defaultValue;

            if (int.TryParse(input, out int value) && value >= min && value <= max)
                return value;

            System.Console.WriteLine($"    ⚠ Veuillez entrer un nombre entre {min} et {max}, ou Entrée pour {defaultValue}.");
        }
    }

    // ──────────────────────────────────────────────
    //  Format de sortie (défaut: jpg)
    // ──────────────────────────────────────────────

    private static OutputFormat ReadOutputFormatWithDefault()
    {
        while (true)
        {
            System.Console.Write("  Format de sortie (pdf / jpg) [défaut: jpg] : ");
            string? input = System.Console.ReadLine()?.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(input)) return DefaultOutputFormat;
            if (input == "pdf") return OutputFormat.Pdf;
            if (input == "jpg") return OutputFormat.Jpg;

            System.Console.WriteLine("    ⚠ Veuillez entrer 'pdf', 'jpg', ou Entrée pour jpg.");
        }
    }

    // ──────────────────────────────────────────────
    //  Style des flèches (défaut: dessiné à la main)
    // ──────────────────────────────────────────────

    private static ArrowStyle ReadArrowStyleWithDefault()
    {
        while (true)
        {
            System.Console.Write("  Style des flèches (1: Clean, 2: Dessiné à la main, 3: Pointillé) [défaut: 2] : ");
            string? input = System.Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input)) return DefaultArrowStyle;
            if (input == "1") return ArrowStyle.Clean;
            if (input == "2") return ArrowStyle.HandDrawn;
            if (input == "3") return ArrowStyle.Dotted;

            System.Console.WriteLine("    ⚠ Veuillez entrer '1', '2', '3', ou Entrée pour 2.");
        }
    }

    // ──────────────────────────────────────────────
    //  Difficulté (défaut: 2 - Normal)
    // ──────────────────────────────────────────────

    private static Difficulty ReadDifficultyWithDefault()
    {
        while (true)
        {
            System.Console.WriteLine("  Difficulté :");
            System.Console.WriteLine("    1 - Facile (liste textuelle)");
            System.Console.WriteLine("    2 - Normal (flèches colorées + noms)");
            System.Console.WriteLine("    3 - Moyen  (flèches colorées, boussole tournée)");
            System.Console.WriteLine("    4 - Difficile (flèches noires, couleurs séparées)");
            System.Console.WriteLine("    5 - Cauchemar (flèches noires, sans boussole)");
            System.Console.Write("  Choix (1-5) [défaut: 2] : ");
            string? input = System.Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input)) return DefaultDifficulty;

            if (int.TryParse(input, out int level) && level >= 1 && level <= 5)
                return (Difficulty)level;

            System.Console.WriteLine("    ⚠ Veuillez entrer un nombre entre 1 et 5, ou Entrée pour 2.");
        }
    }
}
