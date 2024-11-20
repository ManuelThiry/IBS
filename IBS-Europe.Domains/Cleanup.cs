using System.Text.RegularExpressions;

namespace IBS_Europe.Domains;

public static class Cleanup
{
    public static string GenerateUniqueFileName(string originalFileName)
    {
        // Générer un UUID ou une chaîne aléatoire pour garantir l'unicité
        var uniqueFileName = Guid.NewGuid().ToString(); 
    
        // Extraire l'extension du fichier original
        var extension = Path.GetExtension(originalFileName);
    
        // Retourner le nom final unique
        return $"{uniqueFileName}{extension}";
    }
    
    public static bool IsPathInDirectory(string filePath, string dir)
    {
        // Assurez-vous que le chemin est absolu (si ce n'est pas déjà le cas)
        string absolutePath = Path.GetFullPath(filePath);

        // Extraire le répertoire du fichier
        string directory = Path.GetDirectoryName(absolutePath);

        // Vérifiez si le chemin contient "Products" (en ignorant la casse)
        return directory.Contains(dir, StringComparison.OrdinalIgnoreCase);
    }

}