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
}
