using CoursesGn.Domain.Enums;
using CoursesGn.Domain.Interfaces;

namespace CoursesGn.Infrastructure.Export;

/// <summary>
/// Exporte les données binaires d'une carte vers un fichier sur disque.
/// </summary>
public class FileExporter : IFileExporter
{
    public string Export(byte[] data, OutputFormat format, Difficulty difficulty, int index, string outputDirectory)
    {
        string extension = format switch
        {
            OutputFormat.Pdf => "pdf",
            OutputFormat.Jpg => "jpg",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Format non supporté.")
        };

        string difficultyTag = difficulty switch
        {
            Difficulty.Level1_Easy      => "facile",
            Difficulty.Level2_Normal    => "normal",
            Difficulty.Level3_Medium    => "moyen",
            Difficulty.Level4_Hard      => "difficile",
            Difficulty.Level5_Nightmare => "cauchemar",
            _ => "diff"
        };

        string fileName = $"carte_nav_{difficultyTag}_{index:D3}.{extension}";
        string filePath = Path.Combine(outputDirectory, fileName);

        File.WriteAllBytes(filePath, data);

        // Vérification : le fichier doit exister et être non vide
        var info = new FileInfo(filePath);
        if (!info.Exists || info.Length == 0)
            throw new IOException($"Le fichier '{fileName}' n'a pas été correctement écrit (taille = {info.Length} octet(s)).");

        return filePath;
    }
}
