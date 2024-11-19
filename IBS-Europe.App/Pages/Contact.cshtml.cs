using System.ComponentModel.DataAnnotations;
using IBS_Europe.App.Pages.Shared.Email;
using IBS_Europe.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using Newtonsoft.Json;
using Type = IBS_Europe.Domains.Type;

namespace IBS_Europe.App.Pages;

public class Contact : PageModel
{
    private readonly IContactData _data;
    public List<EmailViewModel> EmailList { get; set; } = new List<EmailViewModel>();
    public List<InfosViewModel> Informations { get; set; } = new List<InfosViewModel>();
    
    
    [BindProperty] public InformationModel Information { get; set; } = new InformationModel();
    [BindProperty] public EmailModel Email { get; set; } = new EmailModel();

    private readonly EmailService _emailService;

    public bool IsAddInformation { get; set; }
    public bool IsAddEmail { get; set; }    
    
    public bool isOpenInfo { get; set; }
    public bool Success { get; set; }
    [BindProperty] public MailModel Input { get; set; } = new MailModel();


    public Contact(IContactData data, EmailService emailService)
    {
        _data = data;
        _emailService = emailService;
    }

    public void OnGet(bool isOpen, bool messageSent)
    {
        if (isOpen)
        {
            isOpenInfo = isOpen;
        }
        if (messageSent)
        {
            Success = messageSent;
        }
        Load();
    }

    public async Task Load()
    {
        var emails = await _data.GetEmails();
        foreach (var email in emails)
        {
            EmailList.Add(new EmailViewModel(
                    EmailAddress: email.EmailAddress,
                    Name: email.Name,
                    Description: email.Description,
                    Id: email.Id
                )
            );
        }

        var infos = await _data.GetInformations();
        foreach (var info in infos)
        {
            Informations.Add(new InfosViewModel(
                    Description: info.Description,
                    Text: info.Text,
                    Type: (Type)info.Type,
                    Priority: info.Priority,
                    Id: info.Id
                )
            );
        }
        
        LoadImages();
    }

    private bool InfoRequiredErrors()
    {
        bool error = false;
        if (string.IsNullOrEmpty(Information.Description))
        {
            ModelState.AddModelError("Description", "La description est requise.");
            error = true;
        }
        if (string.IsNullOrEmpty(Information.Text))
        {
            ModelState.AddModelError("Text", "Le texte est requis.");
            error = true;
        }
        
        if (Information.Type == 0)
        {
            ModelState.AddModelError("Type", "La catégorie est requise.");
            error = true;
        }

        return error;
    }
    
    private bool EmailRequiredErrors()
    {
        bool error = false;
        if (string.IsNullOrEmpty(Email.Description))
        {
            ModelState.AddModelError("Description", "La description est requise.");
            error = true;
        }
        if (string.IsNullOrEmpty(Email.Name))
        {
            ModelState.AddModelError("Name", "Le nom est requis.");
            error = true;
        }
        
        if (string.IsNullOrEmpty(Email.EmailAddress))
        {
            ModelState.AddModelError("EmailAddress", "L'adresse de contact est requise.");
            error = true;
        }

        return error;
    }
    
    private bool EmailOthersErrors()
    {
        bool error = false;
        if ( Email.Description.Length > 250 )
        {
            ModelState.AddModelError("Description", "La description ne doit pas dépasser 250 caractères.");
            error = true;
        }
        
        if ( Email.Name.Length > 25 )
        {
            ModelState.AddModelError("Name", "Le nom ne doit pas dépasser 25 caractères.");
            error = true;
        }
        
        var emailAttribute = new EmailAddressAttribute();
        if (!emailAttribute.IsValid(Email.EmailAddress))
        {
            ModelState.AddModelError("EmailAddress", "L'adresse de contact n'est pas valide.");
            error = true;
        }

        return error;
    }

    
    

    private bool InfoOthersErrors()
    {
        bool error = false;
        if ( Information.Description.Length > 25 )
        {
            ModelState.AddModelError("Description", "La description ne doit pas dépasser 25 caractères.");
            error = true;
        }
        
        if ( Information.Text.Length > 100 )
        {
            ModelState.AddModelError("Text", "Le texte ne doit pas dépasser 100 caractères.");
            error = true;
        }

        return error;
    }

    public async Task<IActionResult> OnPostEditIButton(string id)
    {
        var items = await _data.GetInformations();
        var item = items.Find(x => x.Id == int.Parse(id));

        
        ModelState.Clear();
        CookieOptions options = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(1)
        };

        Response.Cookies.Append("selectedInformationId", id.ToString(), options);
        
        Information = new InformationModel
        {
            Title = "Modifier une information",
            Description = item.Description,
            Text = item.Text,
            Type = item.Type
        };
        
        IsAddInformation = true;
        Load();
        return Page();
    }
    
    public async Task<IActionResult> OnPostEditI()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        var action = Request.Form["action"];

        if (action == "cancel")
        {
            return RedirectToPage("Contact", new { isOpen = true });
        }
        
        IsAddInformation = true;
        Information.Title = "Modifier une information";
        
        if (InfoRequiredErrors())
        {
            return Page();
        }
        
        if  ( InfoOthersErrors() )
        {
            return Page();
        }
        else
        {
            int id = -1;
            if (Request.Cookies.ContainsKey("selectedInformationId"))
            {
                id = int.Parse(Request.Cookies["selectedInformationId"]);
            }
            else
            {
                return RedirectToPage();
            }
            await _data.UpdateInformation(new Informations { Id = id, Description = Information.Description, Text = Information.Text, Type = Information.Type });
            return RedirectToPage("Contact", new { isOpen = true });

        }
    }

    public IActionResult OnPostCancel()
    {
        return RedirectToPage();
    }
    
    public IActionResult OnPostDelete(string contactId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        var action = Request.Form["action"];

        if (action == "cancel")
        {
            return RedirectToPage("Contact", new { isOpen = true });
        }
        
        string category = contactId.Split('-')[0];
        int id = int.Parse(contactId.Split('-')[1]);
        
        if ( category == "E" )
        {
            _data.DeleteEmail(id);
        }
        else if ( category == "I" )
        {
            _data.DeleteInformation(id);
        }
        return RedirectToPage("Contact", new { isOpen = true });
    }
    
    public async Task<IActionResult> OnPostEditEButton(string id)
    {
        var items = await _data.GetEmails();
        var item = items.Find(x => x.Id == int.Parse(id));

        
        ModelState.Clear();
        CookieOptions options = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(1)
        };

        Response.Cookies.Append("selectedEmailId", id.ToString(), options);
        
        Email = new EmailModel()
        {
            Title = "Modifier un contact",
            Description = item.Description,
            Name = item.Name,
            EmailAddress = item.EmailAddress
        };
        
        IsAddEmail = true;
        Load();
        return Page();
    }
    
    public async Task<IActionResult> OnPostEditE()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        var action = Request.Form["action"];

        if (action == "cancel")
        {
            return RedirectToPage("Contact", new { isOpen = true });
        }
        
        IsAddEmail = true;
        Email.Title = "Modifier un contact";
        
        if (EmailRequiredErrors())
        {
            return Page();
        }
        
        if  ( EmailOthersErrors() )
        {
            return Page();
        }
        else
        {
            int id = -1;
            if (Request.Cookies.ContainsKey("selectedEmailId"))
            {
                id = int.Parse(Request.Cookies["selectedEmailId"]);
            }
            else
            {
                return RedirectToPage();
            }
            await _data.UpdateEmail(new Email { Id = id, Description = Email.Description, Name = Email.Name, EmailAddress = Email.EmailAddress });
            return RedirectToPage("Contact", new { isOpen = true });
        }
    }

    public async Task<IActionResult> OnPostAddI()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        var action = Request.Form["action"];

        if (action == "cancel")
        {
            return RedirectToPage("Contact", new { isOpen = true });
        }
        
        IsAddInformation = true;
        Information.Title = "Ajouter une information";
        
        if (InfoRequiredErrors())
        {
            return Page();
        }
        
        if  ( InfoOthersErrors() )
        {
            return Page();
        }
        else
        {
            await _data.AddInformation(new Informations { Description = Information.Description, Text = Information.Text, Type = Information.Type });
            return RedirectToPage("Contact", new { isOpen = true });
        }
    }

    public IActionResult OnPostAddIButton()
    {
        Information = new InformationModel
        {
            Title = "Ajouter une information"
        };
        IsAddInformation = true;
        return Page();
    }
    
    public IActionResult OnPostAddEButton()
    {
        Email = new EmailModel()
        {
            Title = "Ajouter un contact"
        };
        IsAddEmail = true;
        return Page();
    }
    
    public async Task<IActionResult> OnPostAddE()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        var action = Request.Form["action"];

        if (action == "cancel")
        {
            return RedirectToPage("Contact", new { isOpen = true });
        }
        
        IsAddEmail = true;
        Email.Title = "Ajouter un contact";
        
        if (EmailRequiredErrors())
        {
            return Page();
        }
        
        if  ( EmailOthersErrors() )
        {
            return Page();
        }
        else
        {
            await _data.AddEmail(new Email { Description = Email.Description, Name = Email.Name, EmailAddress = Email.EmailAddress });
            return RedirectToPage("Contact", new { isOpen = true });
        }
    }
    
    public IActionResult OnPostSwitch(string direction, int priority)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage();
        }
        
        _data.SwitchPriority(priority, direction);
        Load();
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        bool errors = false;
            
            if (!HasEmailOrPhoneNumber())
            {
                ModelState.AddModelError("Input.Phone", "Veuillez entrer un numéro de téléphone ou une adresse email.");
                Load();
                return Page();
            }
            if (ModelState.IsValid)
            {
                var typeOfMessage = await _data.GetTypeOfMessage(Input.Sort);
                var subject = $"Vous avez reçu un message de type {typeOfMessage.Name} de la part de {Input.FirstName} {Input.LastName}";
                var body = $@"
    <!DOCTYPE html>
    <html lang='fr'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <style>
            body {{
                background-color: #f4f4f4;
                font-family: Arial, sans-serif;
                color: #333;
            }}
            table {{
                width: 100%;
                max-width: 600px;
                margin: 0 auto;
                border-spacing: 0;
                background-color: #ffffff;
                border-collapse: collapse;
            }}
            .header, .footer {{
                background-color: #003366; /* Midnight Blue */
                color: white;
                text-align: center;
                padding: 20px;
            }}
            .content {{
                padding: 20px;
                text-align: left;
            }}
            .content p {{
                margin: 10px 0;
                white-space: pre-line; /* This preserves the new lines in the message */
            }}
            .strong-text {{
                font-weight: bold;
                color: #003366;
            }}
        </style>
    </head>
    <body>
        <table>
            <tr>
                <td class='header'>
                    <h1>{typeOfMessage.Name}</h1>
                </td>
            </tr>
            <tr>
                <td class='content'>
                    <p><span class='strong-text'>Nom :</span> {Input.FirstName} {Input.LastName}</p>
                    <p><span class='strong-text'>Email :</span> {Input.Email}</p>
                    <p><span class='strong-text'>Téléphone :</span> {Input.Phone}</p>
                    <p><span class='strong-text'>Message :</span></p>
                    <p class='text-center'>{Input.Message.Replace(Environment.NewLine, "<br>")}</p>
                </td>
            </tr>
            <tr>
                <td class='footer'>
                    <p>&copy; 2024 IBS Europe</p>
                </td>
            </tr>
        </table>
    </body>
    </html>";
                
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };

                var existingValue = Request.Cookies["contactImage"];

                var map = new Dictionary<string, string>();

                if (existingValue != null)
                {
                    // Désérialiser la chaîne JSON du cookie en dictionnaire
                    map = JsonConvert.DeserializeObject<Dictionary<string, string>>(existingValue);
                }

                foreach (var file in map)
                {
                    // Construire le chemin complet vers le fichier
                    var filePath = Path.Combine("wwwroot", "images", "upload", file.Value);

                    // Vérifier si le fichier existe avant de continuer
                    if (System.IO.File.Exists(filePath))
                    {
                        // Ajouter la pièce jointe en lisant directement le fichier
                        bodyBuilder.Attachments.Add(file.Key, System.IO.File.ReadAllBytes(filePath));
                    }
                }

                
                
                if (await _emailService.SendEmailAsync(typeOfMessage.EmailAddress, subject,
                        bodyBuilder.ToMessageBody()))
                {
                    CookieOptions options = new CookieOptions
                    {
                        Expires = DateTime.Now.AddHours(-1)
                    };
                    Response.Cookies.Append("contactImage", "", options);
                    return RedirectToPage("Contact", new { messageSent = true });
                }
                else
                {
                    TempData["ErrorMessage"] = "Une erreur s'est produite lors de l'envoi de votre message. Veuillez réessayer.";
                }
            }
        Load();
        return Page();
    }

    public void LoadImages()
    {
        var existingValue = Request.Cookies["contactImage"];
        
        if (existingValue != null)
        {
            var map = JsonConvert.DeserializeObject<Dictionary<string, string>>(existingValue);

            foreach (var file in map.Keys)
            {
                Input.Images.Add(new ImageModel
                {
                    FileName = file,
                    FilePath = Path.Combine("images","upload",map[file]),
                    Extension = Path.GetExtension(Path.Combine("wwwroot","images","upload",map[file]))
                });
            }
        }
        
    }

    public class ImageModel
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string FilePath { get; set; }
    }

    public record EmailViewModel
    (
        string EmailAddress,
        string Name,
        string Description,
        int Id
    );

    public record InfosViewModel
    (
        string Description,
        string Text,
        Type Type,
        int Priority,
        int Id
    );


    public class MailModel
    {
        [MaxLength(50, ErrorMessage = "Le nom de famille ne doit pas dépasser 50 caractères.")]
        public string LastName { get; set; }

        [MaxLength(100, ErrorMessage = "L'email ne doit pas dépasser 100 caractères.")]
        [EmailAddress(ErrorMessage = "Veuillez entrer une adresse email valide.")]
        public string Email { get; set; }

        [MaxLength(15, ErrorMessage = "Le numéro de téléphone ne doit pas dépasser 15 caractères.")]
        [Phone(ErrorMessage = "Veuillez entrer un numéro de téléphone valide.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Le nom est requis.")]
        [MaxLength(50, ErrorMessage = "Le prénom ne doit pas dépasser 50 caractères.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le message est requis.")]
        [MaxLength(20000, ErrorMessage = "Le message ne doit pas dépasser 20 000 caractères.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un sujet de message.")]
        public string Sort { get; set; }
        
        public List<ImageModel> Images { get; set; } = new List<ImageModel>();
    }
    
    public class InformationModel
    {
        public string Text { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        
        public string Title { get; set; }
    }
    
    public class EmailModel
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public string Title { get; set; }
    }

    public bool HasEmailOrPhoneNumber()
    {
        return !string.IsNullOrWhiteSpace(Input.Email) || !string.IsNullOrWhiteSpace(Input.Phone);
    }
}