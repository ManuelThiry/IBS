﻿using IBS_Europe.Domains;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IBS_Europe.App.Controllers
{
    public class ImageController : Controller
    {
        [HttpPost("upload-image")]
        public async Task<IActionResult> AddFile([FromBody] FileModel file)
        {
            if (file == null)
            {
                return BadRequest("Aucune donnée reçue.");
            }

            try
            {
                // Décoder l'image Base64
                byte[] imageBytes = Convert.FromBase64String(file.Base64);
                
                if (!ImagesVerification.PngJpgOrPdf(imageBytes))
                {
                    return BadRequest(new { message = $"Le fichier \"{file.FileName}\" est invalide. Seuls les fichiers .png, .jpg et .pdf sont autorisés."});
                }
                
                const int maxFileSizeInBytes = 20 * 1024 * 1024;
                if (imageBytes.Length > maxFileSizeInBytes)
                {
                    return BadRequest(new { message = $"Le fichier \"{file.FileName}\" est trop volumineux. La taille maximale autorisée est de 20 Mo."});
                }
                
                
                string uniqueName = Cleanup.GenerateUniqueFileName(file.FileName);
                
                var existingValue = Request.Cookies["contactImage"];

                var map = new Dictionary<string, string>();

                if (existingValue != null)
                {
                    // Désérialiser la chaîne JSON du cookie en dictionnaire
                    map = JsonConvert.DeserializeObject<Dictionary<string, string>>(existingValue);
                    if ( map.ContainsKey(file.FileName) )
                    {
                        return Ok(new { message = 0});
                    }
                }
                map.Add(file.FileName, uniqueName);

                // Sérialiser la map mise à jour en JSON avant de l'ajouter au cookie
                var newValue = JsonConvert.SerializeObject(map);

                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddHours(1)
                };
                
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "upload");

                // Vérifier si le dossier existe, sinon le créer
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                // Déterminer le chemin du fichier à sauvegarder dans wwwroot/upload
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "upload", uniqueName);

                // Sauvegarder l'image dans le dossier spécifié
                await System.IO.File.WriteAllBytesAsync(uploadPath, imageBytes);

                // Ajouter le cookie avec la nouvelle valeur sérialisée
                Response.Cookies.Append("contactImage", newValue, options);

                return Ok(new { message = "Fichier reçu et sauvegardé avec succès", fileName = file.FileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la sauvegarde de l'image", error = ex.Message });
            }
        }

        [HttpPost("delete-image")]
        public async Task<IActionResult> DeleteFile(string name)
        {
            try
            {
                var existingValue = Request.Cookies["contactImage"];

                var map = new Dictionary<string, string>();

                if (existingValue != null)
                {
                    // Désérialiser la chaîne JSON du cookie en dictionnaire
                    map = JsonConvert.DeserializeObject<Dictionary<string, string>>(existingValue);
                }

                if (!map.ContainsKey(name))
                {
                    return NotFound("File not found in cookies.");
                }

                var targetDelete = map[name];
                map.Remove(name);

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "upload", targetDelete);

                // Supprimer le fichier de manière asynchrone
                if (System.IO.File.Exists(filePath))
                {
                    await Task.Run(() => System.IO.File.Delete(filePath));
                }

                var newValue = JsonConvert.SerializeObject(map);

                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddHours(1)
                };

                Response.Cookies.Append("contactImage", newValue, options);

                return Ok();
            }
            catch (Exception ex)
            {
                // Vous pourriez loguer l'exception ici si nécessaire
                return BadRequest(new { message = ex.Message });
            }
        }

    }
    
    public class FileModel
    {
        public string FileName { get; set; } = "";
        public string Base64 { get; set; } = "";
    }
    

}