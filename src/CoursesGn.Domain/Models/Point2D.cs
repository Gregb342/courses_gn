namespace CoursesGn.Domain.Models;

/// <summary>
/// Point 2D immuable avec opérateurs arithmétiques.
/// Utilisé pour représenter les coordonnées des flèches.
/// </summary>
public readonly record struct Point2D(double X, double Y)
{
    public static Point2D operator +(Point2D a, Point2D b)
        => new(a.X + b.X, a.Y + b.Y);

    public static Point2D operator -(Point2D a, Point2D b)
        => new(a.X - b.X, a.Y - b.Y);

    public static Point2D operator *(Point2D point, double scalar)
        => new(point.X * scalar, point.Y * scalar);

    public static Point2D operator *(double scalar, Point2D point)
        => point * scalar;

    public double DistanceTo(Point2D other)
        => Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));

    public override string ToString() => $"({X:F2}, {Y:F2})";
}
