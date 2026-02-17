using SkiaSharp;
using System.Reflection;

namespace CoursesGn.Infrastructure.Rendering;

/// <summary>
/// Fournit les polices de caractères pour le rendu.
/// Si une police custom est configurée (fichier .ttf/.otf dans Resources/Fonts),
/// elle est chargée depuis les ressources embarquées.
/// Sinon, la police système définie dans RenderingConstants.DefaultFontFamily est utilisée.
///
/// Utilisation :
///   FontProvider.CustomFontFileName = "VoxPopuli.ttf";  // avant la génération
/// </summary>
public static class FontProvider
{
    private static SKTypeface? _customTypeface;
    private static readonly object _lock = new();

    /// <summary>
    /// Nom du fichier de police custom (sans chemin).
    /// Mettre à null pour utiliser la police système définie dans RenderingConstants.
    /// </summary>
    public static string? CustomFontFileName { get; set; } = null;

    /// <summary>
    /// Retourne le SKTypeface normal à utiliser pour le rendu.
    /// </summary>
    public static SKTypeface GetTypeface(SKFontStyle? style = null)
    {
        style ??= SKFontStyle.Normal;

        if (CustomFontFileName is null)
            return SKTypeface.FromFamilyName(RenderingConstants.DefaultFontFamily, style);

        // Pour une police custom, on charge le fichier puis on tente le style demandé
        var baseTypeface = LoadCustomTypeface();
        if (style == SKFontStyle.Normal)
            return baseTypeface;

        // Tenter d'obtenir le style via le nom de famille chargé
        return SKTypeface.FromFamilyName(baseTypeface.FamilyName, style)
               ?? baseTypeface;
    }

    /// <summary>
    /// Raccourci : police en gras.
    /// </summary>
    public static SKTypeface GetBoldTypeface()
        => GetTypeface(SKFontStyle.Bold);

    /// <summary>
    /// Raccourci : police en italique.
    /// </summary>
    public static SKTypeface GetItalicTypeface()
        => GetTypeface(SKFontStyle.Italic);

    /// <summary>
    /// Raccourci : police en gras italique.
    /// </summary>
    public static SKTypeface GetBoldItalicTypeface()
        => GetTypeface(SKFontStyle.BoldItalic);

    // ──────────────────────────────────────────────

    private static SKTypeface LoadCustomTypeface()
    {
        if (_customTypeface is not null)
            return _customTypeface;

        lock (_lock)
        {
            if (_customTypeface is not null)
                return _customTypeface;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(CustomFontFileName!, StringComparison.OrdinalIgnoreCase));

            if (resourceName is null)
            {
                throw new FileNotFoundException(
                    $"Police embarquée '{CustomFontFileName}' introuvable dans les ressources. " +
                    $"Vérifiez que le fichier est dans Resources/Fonts/ et déclaré comme EmbeddedResource. " +
                    $"Ressources disponibles : {string.Join(", ", assembly.GetManifestResourceNames())}");
            }

            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            _customTypeface = SKTypeface.FromStream(stream)
                ?? throw new InvalidOperationException(
                    $"Impossible de charger la police '{CustomFontFileName}'. Le fichier est peut-être corrompu.");

            return _customTypeface;
        }
    }
}
