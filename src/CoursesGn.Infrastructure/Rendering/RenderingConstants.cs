namespace CoursesGn.Infrastructure.Rendering;

/// <summary>
/// Constantes de rendu pour le format A4 à 150 DPI.
/// </summary>
public static class RenderingConstants
{
    // ── Police par défaut ──
    public const string DefaultFontFamily = "Calibri";

    // ── Dimensions A4 portrait en pixels (150 DPI) ──
    public const int A4WidthPixels = 1240;
    public const int A4HeightPixels = 1754;

    // ── Dimensions A4 portrait en points (72 DPI, pour PDF) ──
    public const float A4WidthPoints = 595.28f;
    public const float A4HeightPoints = 841.89f;

    // ── Dimensions A4 paysage en pixels (150 DPI) ──
    public const int A4LandscapeWidthPixels = 1754;
    public const int A4LandscapeHeightPixels = 1240;
    public const int HalfPageWidthPixels = 877; // 1754 / 2

    // ── Dimensions A4 paysage en points (72 DPI, pour PDF) ──
    public const float A4LandscapeWidthPoints = 841.89f;
    public const float A4LandscapeHeightPoints = 595.28f;
    public const float HalfPageWidthPoints = 420.945f; // 841.89 / 2

    // ── Scale pour projeter un A4 portrait dans une moitié A5 paysage ──
    // 877px / 1240px ≈ 0.7073 (pixels) | 420.945pt / 595.28pt ≈ 0.7073 (points)
    public const float HalfCardScale = 877f / 1240f;

    // ── Séparateur central ──
    public const float SeparatorLineWidth = 2f;

    // ── Marges ──
    public const int Margin = 80;

    // ── Boussole ──
    public const int CompassSize = 260;
    public const int CompassMargin = 20;

    // ── Flèches ──
    public const float ArrowStrokeWidth = 6.0f;
    public const float ArrowHeadAngleDegrees = 28f;
    public const float MinArrowHeadLength = 10f;
    public const float MaxArrowHeadLength = 28f;
    public const float ArrowHeadRatio = 0.14f;

    // ── Numérotation ──
    public const float StepNumberFontSize = 32f;
    public const float StepNumberOffset = 28f;

    // ── Noms de couleurs le long des flèches (Niveau 2) ──
    public const float ColorLabelFontSize = 32f;
    public const float ColorLabelOffset = 36f;

    // ── Encadré couleurs séparé (Niveau 4-5) ──
    public const float ColorBoxFontSize = 38f;
    public const float ColorBoxPadding = 30f;
    public const float ColorBoxLineHeight = 54f;
    public const float ColorBoxCornerRadius = 10f;

    // ── Niveau 1 : rendu textuel ──
    public const float TextLevelFontSize = 50f;
    public const float TextLevelLineHeight = 80f;
    public const float TextLevelTitleFontSize = 55f;

    // ── Qualité JPEG ──
    public const int JpegQuality = 95;
}
