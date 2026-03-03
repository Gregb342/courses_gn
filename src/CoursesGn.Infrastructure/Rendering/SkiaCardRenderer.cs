using CoursesGn.Domain.Enums;
using CoursesGn.Domain.Helpers;
using CoursesGn.Domain.Models;
using CoursesGn.Domain.Interfaces;
using SkiaSharp;

namespace CoursesGn.Infrastructure.Rendering;

/// <summary>
/// Implémentation du rendu de cartes de navigation via SkiaSharp.
/// Gère les 5 niveaux de difficulté :
///   1 = texte seul, 2 = flèches + noms, 3 = flèches + boussole tournée,
///   4 = flèches noires + encadré couleurs, 5 = noir + pas de boussole.
/// </summary>
public class SkiaCardRenderer : ICardRenderer
{
    private readonly ArrowDrawer _arrowDrawer = new();
    private readonly CompassRenderer _compassRenderer = new();

    public byte[] Render(NavigationCourse course, GenerationParameters parameters, string cardLabel)
    {
        return parameters.OutputFormat switch
        {
            OutputFormat.Jpg => RenderAsImage(course, parameters, cardLabel),
            OutputFormat.Pdf => RenderAsPdf(course, parameters, cardLabel),
            _ => throw new ArgumentOutOfRangeException(
                nameof(parameters.OutputFormat), parameters.OutputFormat, "Format non supporté.")
        };
    }

    // ──────────────────────────────────────────────
    //  Rendu JPG
    // ──────────────────────────────────────────────

    private byte[] RenderAsImage(NavigationCourse course, GenerationParameters parameters, string cardLabel)
    {
        var imageInfo = new SKImageInfo(
            RenderingConstants.A4WidthPixels,
            RenderingConstants.A4HeightPixels);

        using var surface = SKSurface.Create(imageInfo);
        var canvas = surface.Canvas;

        RenderContent(canvas, course, parameters, cardLabel);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, RenderingConstants.JpegQuality);
        return data.ToArray();
    }

    // ──────────────────────────────────────────────
    //  Rendu PDF
    // ──────────────────────────────────────────────

    private byte[] RenderAsPdf(NavigationCourse course, GenerationParameters parameters, string cardLabel)
    {
        var memoryStream = new MemoryStream();

        using (var skStream = new SKManagedWStream(memoryStream))
        {
            using (var document = SKDocument.CreatePdf(skStream))
            {
                var canvas = document.BeginPage(
                    RenderingConstants.A4WidthPoints,
                    RenderingConstants.A4HeightPoints);

                float scaleX = RenderingConstants.A4WidthPoints / RenderingConstants.A4WidthPixels;
                float scaleY = RenderingConstants.A4HeightPoints / RenderingConstants.A4HeightPixels;
                canvas.Scale(scaleX, scaleY);

                RenderContent(canvas, course, parameters, cardLabel);

                document.EndPage();
                document.Close();
            }
        }

        return memoryStream.ToArray();
    }

    // ──────────────────────────────────────────────
    //  Dispatch par niveau de difficulté
    // ──────────────────────────────────────────────

    private void RenderContent(
        SKCanvas canvas,
        NavigationCourse course,
        GenerationParameters parameters,
        string cardLabel)
    {
        canvas.Clear(SKColors.White);

        // Dessiner le label (Carte PJ / Carte PNJ) en haut à droite
        DrawCardLabel(canvas, cardLabel);

        if (parameters.Difficulty == Difficulty.Level1_Easy)
        {
            RenderTextLevel(canvas, course, parameters);
            return;
        }

        RenderGraphicLevel(canvas, course, parameters);
    }

    // ──────────────────────────────────────────────
    //  Label Carte PJ / Carte PNJ
    // ──────────────────────────────────────────────

    private static void DrawCardLabel(SKCanvas canvas, string cardLabel)
    {
        if (string.IsNullOrWhiteSpace(cardLabel))
            return;

        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 28f,
            IsAntialias = true,
            TextAlign = SKTextAlign.Right,
            Typeface = FontProvider.GetTypeface()
        };

        float x = RenderingConstants.A4WidthPixels - RenderingConstants.Margin;
        float y = 38f;

        canvas.DrawText(cardLabel, x, y, paint);
    }

    // ══════════════════════════════════════════════
    //  NIVEAU 1 : Rendu textuel
    // ══════════════════════════════════════════════

    private static void RenderTextLevel(
        SKCanvas canvas,
        NavigationCourse course,
        GenerationParameters parameters)
    {
        float pageW = RenderingConstants.A4WidthPixels;
        float pageH = RenderingConstants.A4HeightPixels;
        float titleFontSize = RenderingConstants.TextLevelTitleFontSize;
        float textFontSize = RenderingConstants.TextLevelFontSize;
        float lineHeight = RenderingConstants.TextLevelLineHeight;
        float separatorGapAbove = 30f;  // espace au-dessus du trait
        float separatorGapBelow = 60f;  // espace en-dessous du trait (avant la liste)

        // ── Pinceaux ──
        using var titlePaint = new SKPaint
        {
            Color = new SKColor(40, 40, 40),
            TextSize = titleFontSize,
            IsAntialias = true,
            FakeBoldText = true,
            TextAlign = SKTextAlign.Center,
            Typeface = FontProvider.GetBoldTypeface()
        };

        using var textPaint = new SKPaint
        {
            Color = new SKColor(40, 40, 40),
            TextSize = textFontSize,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center,
            Typeface = FontProvider.GetTypeface()
        };

        using var linePaint = new SKPaint
        {
            Color = new SKColor(180, 180, 180),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.5f,
            IsAntialias = true
        };

        // ── Calcul de la hauteur totale du bloc ──
        int arrowCount = course.Arrows.Count;
        float totalHeight = titleFontSize               // titre
                          + separatorGapAbove            // espace avant trait
                          + separatorGapBelow            // espace après trait
                          + lineHeight * arrowCount;     // lignes de texte

        // ── Centrage vertical ──
        float startY = (pageH - totalHeight) / 2f;
        float centerX = pageW / 2f;

        // ── Titre centré ──
        float y = startY + titleFontSize;
        canvas.DrawText("Carte de Navigation", centerX, y, titlePaint);

        // ── Trait de séparation centré ──
        y += separatorGapAbove;
        float lineMargin = pageW * 0.15f;
        canvas.DrawLine(lineMargin, y, pageW - lineMargin, y, linePaint);

        y += separatorGapBelow;

        // ── Liste des manœuvres ──
        foreach (var arrow in course.Arrows)
        {
            string directionName = DirectionHelper.GetFrenchName(arrow.Direction);
            string colorName = SkiaColorMapper.GetFrenchName(arrow.Color);
            SKColor skColor = SkiaColorMapper.ToSkiaColor(arrow.Color);

            // Construire la ligne complète pour mesurer la largeur totale
            string leftPart = $"{arrow.StepNumber}. {directionName} → ";
            float leftWidth = textPaint.MeasureText(leftPart);

            using var colorPaint = new SKPaint
            {
                Color = skColor,
                TextSize = textFontSize,
                IsAntialias = true,
                FakeBoldText = true,
                Typeface = FontProvider.GetBoldTypeface()
            };
            float colorWidth = colorPaint.MeasureText(colorName);

            // Largeur totale de la ligne → on centre le tout
            float totalLineWidth = leftWidth + colorWidth;
            float lineX = (pageW - totalLineWidth) / 2f;

            canvas.DrawText(leftPart, lineX, y, new SKPaint
            {
                Color = textPaint.Color,
                TextSize = textFontSize,
                IsAntialias = true,
                TextAlign = SKTextAlign.Left,
                Typeface = FontProvider.GetTypeface()
            });

            canvas.DrawText(colorName, lineX + leftWidth, y, colorPaint);

            y += lineHeight;
        }
    }

    // ══════════════════════════════════════════════
    //  NIVEAUX 2-5 : Rendu graphique
    // ══════════════════════════════════════════════

    private void RenderGraphicLevel(
        SKCanvas canvas,
        NavigationCourse course,
        GenerationParameters parameters)
    {
        var difficulty = parameters.Difficulty;
        bool forceBlack = difficulty >= Difficulty.Level4_Hard;
        bool showColorLabel = difficulty == Difficulty.Level2_Normal;
        bool showCompass = difficulty != Difficulty.Level5_Nightmare;
        bool rotateCompass = difficulty >= Difficulty.Level3_Medium;
        bool useCompassWithoutLabels = difficulty >= Difficulty.Level3_Medium;
        bool showColorBox = difficulty >= Difficulty.Level4_Hard;

        var transform = CalculateTransformation(course);

        // Point de départ
        var startScreenPoint = TransformPoint(course.Arrows[0].Start, transform);
        _arrowDrawer.DrawStartPoint(canvas, startScreenPoint);

        // Flèches
        foreach (var arrow in course.Arrows)
        {
            var screenStart = TransformPoint(arrow.Start, transform);
            var screenEnd = TransformPoint(arrow.End, transform);

            _arrowDrawer.Draw(
                canvas, screenStart, screenEnd,
                arrow, parameters.ArrowStyle,
                forceBlack, showColorLabel);
        }

        // Boussole
        if (showCompass)
        {
            float compassX = RenderingConstants.A4WidthPixels
                            - RenderingConstants.Margin
                            - RenderingConstants.CompassSize;
            float compassY = RenderingConstants.CompassMargin;

            float rotation = rotateCompass
                ? Random.Shared.Next(1, 8) * 45f  // 45°, 90°, 135°, … 315°
                : 0f;

            _compassRenderer.Draw(canvas, compassX, compassY, RenderingConstants.CompassSize,
                rotation, useCompassWithoutLabels);
        }

        // Encadré de couleurs séparé (niveaux 4-5)
        if (showColorBox)
            DrawColorBox(canvas, course, parameters);
    }

    // ──────────────────────────────────────────────
    //  Encadré des couleurs (Niveaux 4-5)
    // ──────────────────────────────────────────────

    private static void DrawColorBox(
        SKCanvas canvas,
        NavigationCourse course,
        GenerationParameters parameters)
    {
        float padding = RenderingConstants.ColorBoxPadding;
        float fontSize = RenderingConstants.ColorBoxFontSize;
        float lineHeight = RenderingConstants.ColorBoxLineHeight;
        float cornerRadius = RenderingConstants.ColorBoxCornerRadius;

        int count = course.Arrows.Count;

        // Mesurer la largeur maximale du texte
        using var measurePaint = new SKPaint
        {
            TextSize = fontSize,
            Typeface = FontProvider.GetTypeface()
        };

        float maxTextWidth = 0;
        foreach (var arrow in course.Arrows)
        {
            string label = $"{arrow.StepNumber}. {SkiaColorMapper.GetFrenchName(arrow.Color)}";
            float w = measurePaint.MeasureText(label);
            if (w > maxTextWidth) maxTextWidth = w;
        }

        float boxWidth = maxTextWidth + padding * 2 + 10;
        float boxHeight = lineHeight * count + padding * 2;

        // Positionner en bas à gauche (dans la marge)
        float boxX = RenderingConstants.Margin;
        float boxY = RenderingConstants.A4HeightPixels - RenderingConstants.Margin - boxHeight;

        // Fond (sans bordure)
        using var bgPaint = new SKPaint
        {
            Color = new SKColor(250, 250, 250),
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawRoundRect(boxX, boxY, boxWidth, boxHeight, cornerRadius, cornerRadius, bgPaint);

        // Texte des couleurs
        float textX = boxX + padding;
        float textY = boxY + padding + fontSize;

        // Niveau 5 : effet Stroop — chaque nom de couleur est écrit dans une AUTRE couleur
        bool stroopEffect = parameters.Difficulty == Difficulty.Level5_Nightmare;
        var allColors = Enum.GetValues<Domain.Enums.ArrowColor>();

        foreach (var arrow in course.Arrows)
        {
            string colorName = SkiaColorMapper.GetFrenchName(arrow.Color);
            string label = $"{arrow.StepNumber}. {colorName}";

            SKColor textColor;
            if (stroopEffect)
            {
                // Choisir une couleur différente de la vraie
                Domain.Enums.ArrowColor fakeColor;
                do
                {
                    fakeColor = allColors[Random.Shared.Next(allColors.Length)];
                } while (fakeColor == arrow.Color);
                textColor = SkiaColorMapper.ToSkiaColor(fakeColor);
            }
            else
            {
                textColor = new SKColor(50, 50, 50);
            }

            using var itemPaint = new SKPaint
            {
                Color = textColor,
                TextSize = fontSize,
                IsAntialias = true,
                FakeBoldText = stroopEffect,
                Typeface = FontProvider.GetTypeface()
            };
            canvas.DrawText(label, textX, textY, itemPaint);
            textY += lineHeight;
        }
    }

    // ──────────────────────────────────────────────
    //  Transformation coordonnées abstraites → pixels
    // ──────────────────────────────────────────────

    private static TransformData CalculateTransformation(NavigationCourse course)
    {
        var allPoints = course.Arrows
            .SelectMany(a => new[] { a.Start, a.End })
            .ToList();

        double minX = allPoints.Min(p => p.X);
        double minY = allPoints.Min(p => p.Y);
        double maxX = allPoints.Max(p => p.X);
        double maxY = allPoints.Max(p => p.Y);

        double pathWidth = Math.Max(maxX - minX, 0.001);
        double pathHeight = Math.Max(maxY - minY, 0.001);

        double availableWidth = RenderingConstants.A4WidthPixels - 2.0 * RenderingConstants.Margin;
        double availableHeight = RenderingConstants.A4HeightPixels - 2.0 * RenderingConstants.Margin;

        double scaleX = availableWidth / pathWidth;
        double scaleY = availableHeight / pathHeight;
        double scale = Math.Min(scaleX, scaleY) * 0.92;

        double scaledWidth = pathWidth * scale;
        double scaledHeight = pathHeight * scale;

        double offsetX = RenderingConstants.Margin + (availableWidth - scaledWidth) / 2.0 - minX * scale;
        double offsetY = RenderingConstants.Margin + (availableHeight - scaledHeight) / 2.0 - minY * scale;

        AdjustOffsetToBounds(ref offsetX, ref offsetY, scale, minX, minY, maxX, maxY);

        return new TransformData(scale, offsetX, offsetY);
    }

    private static void AdjustOffsetToBounds(
        ref double offsetX, ref double offsetY,
        double scale,
        double minX, double minY,
        double maxX, double maxY)
    {
        double txMinX = minX * scale + offsetX;
        double txMaxX = maxX * scale + offsetX;
        double txMinY = minY * scale + offsetY;
        double txMaxY = maxY * scale + offsetY;

        int margin = RenderingConstants.Margin;
        int pageW = RenderingConstants.A4WidthPixels;
        int pageH = RenderingConstants.A4HeightPixels;

        if (txMinX < margin)
            offsetX += margin - txMinX;
        else if (txMaxX > pageW - margin)
            offsetX -= txMaxX - (pageW - margin);

        if (txMinY < margin)
            offsetY += margin - txMinY;
        else if (txMaxY > pageH - margin)
            offsetY -= txMaxY - (pageH - margin);
    }

    private static Point2D TransformPoint(Point2D point, TransformData transform)
    {
        return new Point2D(
            point.X * transform.Scale + transform.OffsetX,
            point.Y * transform.Scale + transform.OffsetY);
    }

    // ──────────────────────────────────────────────

    private record TransformData(double Scale, double OffsetX, double OffsetY);
}
