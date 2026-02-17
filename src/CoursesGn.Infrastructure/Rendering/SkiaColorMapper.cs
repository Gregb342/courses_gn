using CoursesGn.Domain.Enums;
using SkiaSharp;

namespace CoursesGn.Infrastructure.Rendering;

/// <summary>
/// Correspondance entre les couleurs du domaine et les couleurs SkiaSharp.
/// </summary>
public static class SkiaColorMapper
{
    private static readonly Dictionary<ArrowColor, SKColor> ColorMap = new()
    {
        { ArrowColor.Blue,   new SKColor(30, 100, 220) },
        { ArrowColor.Red,    new SKColor(220, 40, 40) },
        { ArrowColor.Green,  new SKColor(30, 160, 60) },
        { ArrowColor.Orange, new SKColor(240, 140, 20) },
        { ArrowColor.Purple, new SKColor(140, 50, 180) },
        { ArrowColor.Yellow, new SKColor(210, 180, 20) },
    };

    /// <summary>Convertit une ArrowColor du domaine en SKColor.</summary>
    public static SKColor ToSkiaColor(ArrowColor color)
        => ColorMap.GetValueOrDefault(color, SKColors.Black);

    /// <summary>Retourne le nom français de la couleur.</summary>
    public static string GetFrenchName(ArrowColor color) => color switch
    {
        ArrowColor.Blue   => "Bleu",
        ArrowColor.Red    => "Rouge",
        ArrowColor.Green  => "Vert",
        ArrowColor.Orange => "Orange",
        ArrowColor.Purple => "Violet",
        ArrowColor.Yellow => "Jaune",
        _ => color.ToString()
    };
}
