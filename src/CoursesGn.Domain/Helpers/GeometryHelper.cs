using CoursesGn.Domain.Models;

namespace CoursesGn.Domain.Helpers;

/// <summary>
/// Utilitaires géométriques pour la détection d'intersections entre segments.
/// </summary>
public static class GeometryHelper
{
    private const double Epsilon = 1e-10;
    private const double PointTolerance = 1e-6;

    /// <summary>
    /// Vérifie si deux segments [p1→q1] et [p2→q2] se croisent.
    /// Les points partagés (extrémités communes) sont ignorés.
    /// </summary>
    public static bool SegmentsIntersect(Point2D p1, Point2D q1, Point2D p2, Point2D q2)
    {
        // Ignorer les segments qui partagent un point (flèches consécutives)
        if (ArePointsClose(p1, p2) || ArePointsClose(p1, q2) ||
            ArePointsClose(q1, p2) || ArePointsClose(q1, q2))
            return false;

        double d1 = CrossProduct(p2, q2, p1);
        double d2 = CrossProduct(p2, q2, q1);
        double d3 = CrossProduct(p1, q1, p2);
        double d4 = CrossProduct(p1, q1, q2);

        if (((d1 > Epsilon && d2 < -Epsilon) || (d1 < -Epsilon && d2 > Epsilon)) &&
            ((d3 > Epsilon && d4 < -Epsilon) || (d3 < -Epsilon && d4 > Epsilon)))
            return true;

        return false;
    }

    /// <summary>
    /// Calcule le produit vectoriel (B-A) × (C-A).
    /// </summary>
    private static double CrossProduct(Point2D a, Point2D b, Point2D c)
    {
        return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
    }

    /// <summary>
    /// Vérifie si deux points sont suffisamment proches pour être considérés identiques.
    /// </summary>
    private static bool ArePointsClose(Point2D a, Point2D b)
    {
        return Math.Abs(a.X - b.X) < PointTolerance
            && Math.Abs(a.Y - b.Y) < PointTolerance;
    }
}
