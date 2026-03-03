namespace CoursesGn.Infrastructure.Rendering;

/// <summary>
/// Constantes de rendu pour le format A4 à 150 DPI.
/// </summary>
public static class RenderingConstants
{
    // ── Police par défaut ──
    public const string DefaultFontFamily = "Calibri";

    // ── Dimensions A4 en pixels (150 DPI) ──
    public const int A4WidthPixels = 1240;
    public const int A4HeightPixels = 1754;

    // ── Dimensions A4 en points (72 DPI, pour PDF) ──
    public const float A4WidthPoints = 595.28f;
    public const float A4HeightPoints = 841.89f;

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
