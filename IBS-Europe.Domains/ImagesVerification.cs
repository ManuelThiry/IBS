namespace IBS_Europe.Domains;

public static class ImagesVerification
{
    public static bool PngOrJpg(byte[] pdp)
    {
        // Signature d'un fichier PNG
        byte[] pngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        // Vérification si c'est un PNG
        if (pdp.Length >= pngHeader.Length && pdp.Take(pngHeader.Length).SequenceEqual(pngHeader))
        {
            return true;
        }

        // Vérification si c'est un JPEG (premiers octets FF D8)
        if (pdp.Length >= 2 && pdp[0] == 0xFF && pdp[1] == 0xD8)
        {
            return true;
        }

        // Ni PNG ni JPEG
        return false;
    }

    public static bool PngJpgOrPdf(byte[] pdp)
    {
        // Appel de la méthode pour vérifier si c'est un PNG ou JPEG
        if (PngOrJpg(pdp) || Pdf(pdp))
        {
            return true; // C'est soit un PNG, soit un JPEG
        }
        
        return false;
    }
    
    public static bool Pdf(byte[] pdp)
    {
        byte[] pdfHeader = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D };

        // Vérification si c'est un PDF
        if (pdp.Length >= pdfHeader.Length && pdp.Take(pdfHeader.Length).SequenceEqual(pdfHeader))
        {
            return true; // C'est un PDF
        }

        // Ce n'est pas un PDF
        return false;
    }
    
}