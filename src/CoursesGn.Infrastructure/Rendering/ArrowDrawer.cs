using CoursesGn.Domain.Enums;
using CoursesGn.Domain.Models;
using SkiaSharp;

namespace CoursesGn.Infrastructure.Rendering;

/// <summary>
/// Dessine une flèche individuelle sur un canvas SkiaSharp :
/// trait, pointe de flèche, numéro d'étape, et optionnellement le nom de la couleur.
/// </summary>
public class ArrowDrawer
{
    private static readonly Lazy<SKBitmap> _handDrawnArrowHead = new(() => LoadEmbeddedImage("fleche.png"));

    /// <summary>
    /// Dessine une flèche complète.
    /// forceBlack : si true, la flèche est noire (niveaux 4-5).
    /// showColorLabel : si true, le nom de la couleur est écrit le long de la flèche (niveau 2).
    /// </summary>
    public void Draw(
        SKCanvas canvas,
        Point2D screenStart,
        Point2D screenEnd,
        Arrow arrow,
        ArrowStyle style,
        bool forceBlack = false,
        bool showColorLabel = false)
    {
        float x1 = (float)screenStart.X;
        float y1 = (float)screenStart.Y;
        float x2 = (float)screenEnd.X;
        float y2 = (float)screenEnd.Y;

        SKColor skColor = forceBlack
            ? new SKColor(40, 40, 40)
            : SkiaColorMapper.ToSkiaColor(arrow.Color);

        using var paint = CreateArrowPaint(skColor, style);

        // ── Trait de la flèche ──
        if (style == ArrowStyle.HandDrawn)
            DrawHandDrawnLine(canvas, x1, y1, x2, y2, paint);
        else
            canvas.DrawLine(x1, y1, x2, y2, paint);

        // ── Pointe de la flèche ──
        if (style == ArrowStyle.HandDrawn)
            DrawHandDrawnArrowHead(canvas, x1, y1, x2, y2, skColor);
        else
            DrawArrowHead(canvas, x1, y1, x2, y2, skColor);

        // ── Numéro d'étape ──
        DrawStepNumber(canvas, x1, y1, x2, y2, arrow, skColor);

        // ── Nom de la couleur le long de la flèche (niveau 2) ──
        if (showColorLabel)
            DrawColorLabel(canvas, x1, y1, x2, y2, arrow, skColor);
    }

    /// <summary>
    /// Dessine un point noir au point de départ du parcours.
    /// </summary>
    public void DrawStartPoint(SKCanvas canvas, Point2D screenPoint)
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawCircle((float)screenPoint.X, (float)screenPoint.Y, 7f, paint);

        // Contour blanc pour le contraste
        using var borderPaint = new SKPaint
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.5f,
            IsAntialias = true
        };
        canvas.DrawCircle((float)screenPoint.X, (float)screenPoint.Y, 7f, borderPaint);
    }

    // ──────────────────────────────────────────────
    //  Pointe de flèche (triangle plein)
    // ──────────────────────────────────────────────

    private static void DrawArrowHead(
        SKCanvas canvas,
        float x1, float y1,
        float x2, float y2,
        SKColor color)
    {
        float arrowLength = MathF.Sqrt(MathF.Pow(x2 - x1, 2) + MathF.Pow(y2 - y1, 2));
        float headLength = Math.Clamp(
            arrowLength * RenderingConstants.ArrowHeadRatio,
            RenderingConstants.MinArrowHeadLength,
            RenderingConstants.MaxArrowHeadLength);

        double angle = Math.Atan2(y2 - y1, x2 - x1);
        double halfAngle = RenderingConstants.ArrowHeadAngleDegrees * Math.PI / 180.0;

        float leftX = x2 - headLength * (float)Math.Cos(angle - halfAngle);
        float leftY = y2 - headLength * (float)Math.Sin(angle - halfAngle);
        float rightX = x2 - headLength * (float)Math.Cos(angle + halfAngle);
        float rightY = y2 - headLength * (float)Math.Sin(angle + halfAngle);

        using var headPaint = new SKPaint
        {
            Color = color,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        using var path = new SKPath();
        path.MoveTo(x2, y2);
        path.LineTo(leftX, leftY);
        path.LineTo(rightX, rightY);
        path.Close();

        canvas.DrawPath(path, headPaint);
    }

    // ──────────────────────────────────────────────
    //  Numéro d'étape (affiché au milieu de la flèche)
    // ──────────────────────────────────────────────

    private static void DrawStepNumber(
        SKCanvas canvas,
        float x1, float y1,
        float x2, float y2,
        Arrow arrow,
        SKColor color)
    {
        float midX = (x1 + x2) / 2f;
        float midY = (y1 + y2) / 2f;

        // Décalage perpendiculaire à la flèche pour ne pas recouvrir le trait
        double angle = Math.Atan2(y2 - y1, x2 - x1);
        float perpX = -(float)Math.Sin(angle) * RenderingConstants.StepNumberOffset;
        float perpY = (float)Math.Cos(angle) * RenderingConstants.StepNumberOffset;

        using var textPaint = new SKPaint
        {
            Color = color,
            TextSize = RenderingConstants.StepNumberFontSize,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center,
            FakeBoldText = true,
            Typeface = FontProvider.GetBoldTypeface()
        };

        canvas.DrawText(
            arrow.StepNumber.ToString(),
            midX + perpX,
            midY + perpY + RenderingConstants.StepNumberFontSize / 3f,
            textPaint);
    }

    // ──────────────────────────────────────────────
    //  Nom de la couleur le long de la flèche (Niveau 2)
    // ──────────────────────────────────────────────

    private static void DrawColorLabel(
        SKCanvas canvas,
        float x1, float y1,
        float x2, float y2,
        Arrow arrow,
        SKColor color)
    {
        float midX = (x1 + x2) / 2f;
        float midY = (y1 + y2) / 2f;

        // Décalage perpendiculaire opposé au numéro d'étape
        double angle = Math.Atan2(y2 - y1, x2 - x1);
        float perpX = (float)Math.Sin(angle) * RenderingConstants.ColorLabelOffset;
        float perpY = -(float)Math.Cos(angle) * RenderingConstants.ColorLabelOffset;

        using var textPaint = new SKPaint
        {
            Color = color,
            TextSize = RenderingConstants.ColorLabelFontSize,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center,
            Typeface = FontProvider.GetItalicTypeface()
        };

        string colorName = SkiaColorMapper.GetFrenchName(arrow.Color);

        canvas.DrawText(
            colorName,
            midX + perpX,
            midY + perpY + RenderingConstants.ColorLabelFontSize / 3f,
            textPaint);
    }

    // ──────────────────────────────────────────────
    //  Styles de trait
    // ──────────────────────────────────────────────

    private static SKPaint CreateArrowPaint(SKColor color, ArrowStyle style)
    {
        var paint = new SKPaint
        {
            Color = color,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = RenderingConstants.ArrowStrokeWidth,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true
        };

        if (style == ArrowStyle.HandDrawn)
        {
            // Trait légèrement plus fin pour le style manuscrit
            paint.StrokeWidth = RenderingConstants.ArrowStrokeWidth * 0.85f;
        }
        else if (style == ArrowStyle.Dotted)
        {
            // Trait en pointillés : 12px visible, 8px invisible
            paint.PathEffect = SKPathEffect.CreateDash(new float[] { 12f, 8f }, 0f);
        }

        return paint;
    }

    /// <summary>
    /// Dessine une ligne avec de légères perturbations pour simuler un tracé à la main.
    /// </summary>
    private static void DrawHandDrawnLine(
        SKCanvas canvas,
        float x1, float y1,
        float x2, float y2,
        SKPaint paint)
    {
        const int segments = 10;
        float dx = (x2 - x1) / segments;
        float dy = (y2 - y1) / segments;

        float segmentLength = MathF.Sqrt(dx * dx + dy * dy);
        if (segmentLength < 0.01f)
            return;

        float perpX = -dy / segmentLength;
        float perpY = dx / segmentLength;

        // Premier tracé avec wobble
        using var path1 = new SKPath();
        path1.MoveTo(x1, y1);

        for (int i = 1; i < segments; i++)
        {
            float wobble = (float)(Random.Shared.NextDouble() - 0.5) * 3f;
            path1.LineTo(
                x1 + dx * i + perpX * wobble,
                y1 + dy * i + perpY * wobble);
        }
        path1.LineTo(x2, y2);
        canvas.DrawPath(path1, paint);

        // Second tracé semi-transparent pour épaissir le rendu manuscrit
        using var path2 = new SKPath();
        path2.MoveTo(x1 + 0.6f, y1 + 0.6f);

        for (int i = 1; i < segments; i++)
        {
            float wobble = (float)(Random.Shared.NextDouble() - 0.5) * 2.5f;
            path2.LineTo(
                x1 + dx * i + perpX * wobble + 0.6f,
                y1 + dy * i + perpY * wobble + 0.6f);
        }
        path2.LineTo(x2 + 0.6f, y2 + 0.6f);

        using var ghostPaint = paint.Clone();
        ghostPaint.StrokeWidth = paint.StrokeWidth * 0.5f;
        ghostPaint.Color = paint.Color.WithAlpha(140);
        canvas.DrawPath(path2, ghostPaint);
    }

    // ──────────────────────────────────────────────
    //  Pointe de flèche dessinée à la main (PNG)
    // ──────────────────────────────────────────────

    /// <summary>
    /// Dessine la pointe de flèche à partir du PNG embarqué "fleche.png".
    /// L'image source pointe vers le haut (angle = -π/2).
    /// On la teinte à la couleur voulue, on la redimensionne et on la pivote
    /// pour correspondre à la direction de la flèche.
    /// </summary>
    private static void DrawHandDrawnArrowHead(
        SKCanvas canvas,
        float x1, float y1,
        float x2, float y2,
        SKColor color)
    {
        var source = _handDrawnArrowHead.Value;
        if (source == null) return;

        // Taille cible de la pointe (en pixels)
        float arrowLength = MathF.Sqrt(MathF.Pow(x2 - x1, 2) + MathF.Pow(y2 - y1, 2));
        float headSize = Math.Clamp(
            arrowLength * 0.22f,
            24f,
            52f);

        // Calculer l'échelle pour redimensionner le PNG
        float scale = headSize / source.Width;

        // L'image source pointe vers le haut → angle de référence = -π/2
        // On doit pivoter de (angleFlèche - (-π/2)) = (angleFlèche + π/2)
        double arrowAngle = Math.Atan2(y2 - y1, x2 - x1);
        float rotationDegrees = (float)((arrowAngle + Math.PI / 2.0) * 180.0 / Math.PI);

        canvas.Save();

        // Déplacer l'origine au bout de la flèche
        canvas.Translate(x2, y2);
        canvas.RotateDegrees(rotationDegrees);
        canvas.Scale(scale);

        // Dessiner l'image centrée sur le point de la pointe
        // On décale un peu vers le bas pour que la pointe touche le bout de la flèche
        float drawX = -source.Width / 2f;
        float drawY = -source.Height * 0.3f;

        // Teinter l'image avec la couleur voulue
        using var tintPaint = new SKPaint
        {
            IsAntialias = true,
            ColorFilter = SKColorFilter.CreateBlendMode(color, SKBlendMode.SrcIn)
        };

        canvas.DrawBitmap(source, drawX, drawY, tintPaint);

        canvas.Restore();
    }

    // ──────────────────────────────────────────────
    //  Chargement de ressource embarquée
    // ──────────────────────────────────────────────

    private static SKBitmap LoadEmbeddedImage(string fileName)
    {
        var assembly = typeof(ArrowDrawer).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

        if (resourceName == null)
            throw new FileNotFoundException($"Ressource embarquée introuvable : {fileName}");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        return SKBitmap.Decode(stream);
    }
}
