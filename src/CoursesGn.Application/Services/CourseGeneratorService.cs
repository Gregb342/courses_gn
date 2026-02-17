using CoursesGn.Domain.Enums;
using CoursesGn.Domain.Helpers;
using CoursesGn.Domain.Interfaces;
using CoursesGn.Domain.Models;

namespace CoursesGn.Application.Services;

/// <summary>
/// Service de génération de parcours de navigation cohérents.
/// Produit un tracé de flèches connectées, sans intersection,
/// avec des transitions de direction fluides.
/// </summary>
public class CourseGeneratorService : ICourseGenerator
{
    private const int MaxGlobalAttempts = 100;

    /// <summary>Distance minimale entre un nouveau point et tout point déjà visité.</summary>
    private const double MinPointProximity = 0.9;

    /// <summary>Nombre max de virages consécutifs dans le même sens de rotation.</summary>
    private const int MaxConsecutiveSameTurn = 2;

    /// <summary>
    /// Pondérations pour le choix de la prochaine direction.
    /// Plus le virage est faible, plus le poids est élevé → parcours fluides.
    /// </summary>
    private static readonly Dictionary<int, double> AngleWeights = new()
    {
        { 45,  4.0 },  // Virage léger
        { 90,  2.5 },  // Angle droit
        { 135, 1.0 },  // Virage serré
    };

    public NavigationCourse Generate(GenerationParameters parameters)
    {
        for (int attempt = 0; attempt < MaxGlobalAttempts; attempt++)
        {
            var result = TryGenerateCourse(parameters);
            if (result is not null)
                return result;
        }

        throw new InvalidOperationException(
            $"Impossible de générer un parcours valide avec {parameters.ArrowCount} flèches " +
            $"après {MaxGlobalAttempts} tentatives. Essayez avec moins de flèches.");
    }

    // ──────────────────────────────────────────────
    //  Génération d'un parcours complet
    // ──────────────────────────────────────────────

    private NavigationCourse? TryGenerateCourse(GenerationParameters parameters)
    {
        var startPosition = ChooseRandomStartPosition();
        var arrows = new List<Arrow>();
        var visitedPoints = new List<Point2D> { new(0, 0) };
        var currentPoint = new Point2D(0, 0);
        Direction? previousDirection = null;
        var usedColors = new List<ArrowColor>();
        var turnHistory = new List<int>(); // >0 = droite, <0 = gauche, 0 = neutre

        for (int i = 0; i < parameters.ArrowCount; i++)
        {
            var validDirections = GetValidDirections(
                previousDirection, arrows, currentPoint, visitedPoints, turnHistory);

            if (validDirections.Count == 0)
                return null; // Impasse → on recommence depuis zéro

            var direction = ChooseWeightedDirection(validDirections, previousDirection);
            var endPoint = currentPoint + DirectionHelper.GetVector(direction);
            var color = ChooseColor(usedColors);

            // Enregistrer le sens de rotation du virage
            if (previousDirection.HasValue)
                turnHistory.Add(GetTurnSign(previousDirection.Value, direction));

            var arrow = new Arrow
            {
                StepNumber = i + 1,
                Direction = direction,
                Color = color,
                Start = currentPoint,
                End = endPoint
            };

            arrows.Add(arrow);
            usedColors.Add(color);
            visitedPoints.Add(endPoint);
            currentPoint = endPoint;
            previousDirection = direction;
        }

        return new NavigationCourse
        {
            Arrows = arrows,
            StartPosition = startPosition
        };
    }

    // ──────────────────────────────────────────────
    //  Filtrage des directions valides
    // ──────────────────────────────────────────────

    private static List<Direction> GetValidDirections(
        Direction? previousDirection,
        List<Arrow> existingArrows,
        Point2D currentPoint,
        List<Point2D> visitedPoints,
        List<int> turnHistory)
    {
        var allDirections = DirectionHelper.GetAllDirections();
        var validDirections = new List<Direction>();

        foreach (var direction in allDirections)
        {
            // Règle 1 : Pas de demi-tour (180°) ni de ligne droite (0°)
            if (previousDirection.HasValue)
            {
                int angleDiff = DirectionHelper.GetAngleDifference(previousDirection.Value, direction);
                if (angleDiff == 180 || angleDiff == 0)
                    continue;
            }

            var endPoint = currentPoint + DirectionHelper.GetVector(direction);

            // Règle 2 : Pas d'intersection avec les flèches existantes
            if (WouldCauseIntersection(currentPoint, endPoint, existingArrows))
                continue;

            // Règle 3 : Le point d'arrivée ne doit pas être trop proche d'un point déjà visité
            if (IsTooCloseToVisitedPoints(endPoint, visitedPoints))
                continue;

            // Règle 4 : Pas plus de N virages consécutifs dans le même sens
            if (previousDirection.HasValue &&
                WouldExceedConsecutiveTurns(previousDirection.Value, direction, turnHistory))
                continue;

            validDirections.Add(direction);
        }

        return validDirections;
    }

    // ──────────────────────────────────────────────
    //  Règle 3 : Proximité des points visités
    // ──────────────────────────────────────────────

    private static bool IsTooCloseToVisitedPoints(Point2D candidate, List<Point2D> visitedPoints)
    {
        // On ignore le dernier point (c'est le point de départ de la flèche actuelle)
        int checkUpTo = Math.Max(0, visitedPoints.Count - 1);
        for (int i = 0; i < checkUpTo; i++)
        {
            if (candidate.DistanceTo(visitedPoints[i]) < MinPointProximity)
                return true;
        }
        return false;
    }

    // ──────────────────────────────────────────────
    //  Règle 4 : Limite de virages dans le même sens
    // ──────────────────────────────────────────────

    private static bool WouldExceedConsecutiveTurns(
        Direction previous, Direction next, List<int> turnHistory)
    {
        int newTurnSign = GetTurnSign(previous, next);
        if (newTurnSign == 0 || turnHistory.Count == 0)
            return false;

        // Compter les virages récents dans le même sens
        int consecutive = 0;
        for (int i = turnHistory.Count - 1; i >= 0; i--)
        {
            if (turnHistory[i] == newTurnSign)
                consecutive++;
            else
                break;
        }

        return consecutive >= MaxConsecutiveSameTurn;
    }

    /// <summary>
    /// Détermine le sens de rotation : +1 = horaire (droite), -1 = anti-horaire (gauche).
    /// </summary>
    private static int GetTurnSign(Direction from, Direction to)
    {
        int diff = ((int)to - (int)from + 360) % 360;
        if (diff == 0 || diff == 180) return 0;
        return diff < 180 ? 1 : -1;
    }

    // ──────────────────────────────────────────────
    //  Choix pondéré de la direction
    // ──────────────────────────────────────────────

    private static Direction ChooseWeightedDirection(
        List<Direction> validDirections,
        Direction? previousDirection)
    {
        if (!previousDirection.HasValue || validDirections.Count <= 1)
            return validDirections[Random.Shared.Next(validDirections.Count)];

        var weighted = new List<(Direction Direction, double Weight)>();

        foreach (var direction in validDirections)
        {
            int angleDiff = DirectionHelper.GetAngleDifference(previousDirection.Value, direction);
            double weight = AngleWeights.GetValueOrDefault(angleDiff, 0.3);
            weighted.Add((direction, weight));
        }

        return SelectWeightedRandom(weighted);
    }

    private static Direction SelectWeightedRandom(List<(Direction Direction, double Weight)> items)
    {
        double totalWeight = items.Sum(item => item.Weight);
        double roll = Random.Shared.NextDouble() * totalWeight;
        double cumulative = 0;

        foreach (var (direction, weight) in items)
        {
            cumulative += weight;
            if (roll < cumulative)
                return direction;
        }

        return items.Last().Direction;
    }

    // ──────────────────────────────────────────────
    //  Détection d'intersections
    // ──────────────────────────────────────────────

    private static bool WouldCauseIntersection(
        Point2D newStart,
        Point2D newEnd,
        List<Arrow> existingArrows)
    {
        // On ne vérifie pas la dernière flèche (elle partage un point avec la nouvelle)
        int checkUpTo = Math.Max(0, existingArrows.Count - 1);

        for (int i = 0; i < checkUpTo; i++)
        {
            if (GeometryHelper.SegmentsIntersect(
                    newStart, newEnd,
                    existingArrows[i].Start, existingArrows[i].End))
                return true;
        }

        return false;
    }

    // ──────────────────────────────────────────────
    //  Choix aléatoires
    // ──────────────────────────────────────────────

    private static StartPosition ChooseRandomStartPosition()
    {
        var positions = Enum.GetValues<StartPosition>();
        return positions[Random.Shared.Next(positions.Length)];
    }

    private static ArrowColor ChooseColor(List<ArrowColor> recentColors)
    {
        var allColors = Enum.GetValues<ArrowColor>();

        // Éviter de répéter la même couleur que la flèche précédente
        ArrowColor chosen;
        do
        {
            chosen = allColors[Random.Shared.Next(allColors.Length)];
        }
        while (recentColors.Count > 0 && chosen == recentColors.Last());

        return chosen;
    }
}
