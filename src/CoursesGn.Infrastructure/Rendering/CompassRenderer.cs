using System.Reflection;
using SkiaSharp;

namespace CoursesGn.Infrastructure.Rendering;

/// <summary>
/// Dessine la boussole à partir des images embarquées.
/// Deux variantes : avec points cardinaux (niv. 2) et sans (niv. 3-4).
/// </summary>
public class CompassRenderer
{
    private readonly SKBitmap? _compassWithLabels;
    private readonly SKBitmap? _compassWithoutLabels;

    public CompassRenderer()
    {
        _compassWithLabels = LoadEmbeddedImage("boussole.png");
        _compassWithoutLabels = LoadEmbeddedImage("boussole_sans_points_cardinaux.png");
    }

    /// <summary>
    /// Dessine la boussole à la position spécifiée.
    /// (x, y) = coin supérieur gauche de la zone de dessin.
    /// rotationDegrees = rotation de l'image (0 = nord en haut).
    /// useWithoutLabels = true pour utiliser la version sans points cardinaux.
    /// </summary>
    public void Draw(SKCanvas canvas, float x, float y, float size,
                     float rotationDegrees = 0f, bool useWithoutLabels = false)
    {
        var bitmap = useWithoutLabels ? _compassWithoutLabels : _compassWithLabels;
        if (bitmap is null)
            return;

        float centerX = x + size / 2f;
        float centerY = y + size / 2f;

        canvas.Save();

        if (Math.Abs(rotationDegrees) > 0.01f)
            canvas.RotateDegrees(rotationDegrees, centerX, centerY);

        var destRect = new SKRect(x, y, x + size, y + size);

        using var paint = new SKPaint
        {
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };

        canvas.DrawBitmap(bitmap, destRect, paint);

        canvas.Restore();
    }

    // ──────────────────────────────────────────────

    private static SKBitmap? LoadEmbeddedImage(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
            ?? string.Empty;

        if (string.IsNullOrEmpty(resourceName))
            return null;

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return null;

        return SKBitmap.Decode(stream);
    }
}
