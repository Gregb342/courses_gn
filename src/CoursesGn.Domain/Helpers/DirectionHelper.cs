using CoursesGn.Domain.Enums;
using CoursesGn.Domain.Models;

namespace CoursesGn.Domain.Helpers;

/// <summary>
/// Méthodes utilitaires pour manipuler les directions cardinales.
/// </summary>
public static class DirectionHelper
{
    private static readonly double Sqrt2Over2 = Math.Sqrt(2) / 2.0;

    /// <summary>
    /// Vecteurs unitaires pour chaque direction.
    /// Convention : Y positif = vers le bas (coordonnées écran).
    /// </summary>
    private static readonly Dictionary<Direction, Point2D> DirectionVectors = new()
    {
        { Direction.North,     new Point2D(0, -1) },
        { Direction.NorthEast, new Point2D(Sqrt2Over2, -Sqrt2Over2) },
        { Direction.East,      new Point2D(1, 0) },
        { Direction.SouthEast, new Point2D(Sqrt2Over2, Sqrt2Over2) },
        { Direction.South,     new Point2D(0, 1) },
        { Direction.SouthWest, new Point2D(-Sqrt2Over2, Sqrt2Over2) },
        { Direction.West,      new Point2D(-1, 0) },
        { Direction.NorthWest, new Point2D(-Sqrt2Over2, -Sqrt2Over2) },
    };

    /// <summary>Retourne le vecteur unitaire correspondant à une direction.</summary>
    public static Point2D GetVector(Direction direction)
        => DirectionVectors[direction];

    /// <summary>Retourne la direction opposée (demi-tour).</summary>
    public static Direction GetOpposite(Direction direction)
    {
        int oppositeAngle = ((int)direction + 180) % 360;
        return (Direction)oppositeAngle;
    }

    /// <summary>
    /// Calcule la différence angulaire absolue entre deux directions (0-180°).
    /// </summary>
    public static int GetAngleDifference(Direction a, Direction b)
    {
        int diff = Math.Abs((int)a - (int)b);
        return diff > 180 ? 360 - diff : diff;
    }

    /// <summary>Retourne toutes les directions disponibles.</summary>
    public static Direction[] GetAllDirections()
        => Enum.GetValues<Direction>();

    /// <summary>
    /// Retourne le nom français de la direction.
    /// </summary>
    public static string GetFrenchName(Direction direction) => direction switch
    {
        Direction.North     => "Nord",
        Direction.NorthEast => "Nord-Est",
        Direction.East      => "Est",
        Direction.SouthEast => "Sud-Est",
        Direction.South     => "Sud",
        Direction.SouthWest => "Sud-Ouest",
        Direction.West      => "Ouest",
        Direction.NorthWest => "Nord-Ouest",
        _ => direction.ToString()
    };
}
