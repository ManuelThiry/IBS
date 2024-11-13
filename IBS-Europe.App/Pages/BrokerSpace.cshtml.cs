using System.ComponentModel.DataAnnotations;
using IBS_Europe.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IBS_Europe.App.Pages
{
    public class BrokerSpace : PageModel
    {
        private readonly IBrokerData _data;
        
        public int SelectedCategory { get; set; }
        
        public bool IsAddBrokerAction { get; set; }
        public bool IsEditdBrokerAction { get; set; }
        
        [BindProperty]
        public AddBrokerModel Input { get; set; } = new AddBrokerModel();

        public List<BrokerViewModel> BrokersLists { get; set; } = new List<BrokerViewModel>();
        public List<BrokerViewModel> BrokersGeneralLists { get; set; } = new List<BrokerViewModel>();
        
        public BrokerSpace(IBrokerData data)
        {
            _data = data;
        }

        public void OnGet()
        {
            LoadBrokers();
        }
        
        public IActionResult OnPost(int selectedCategory)
        {
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddMonths(1)
            };
            
            Response.Cookies.Append("selectedCategory", selectedCategory.ToString(), option);
            return RedirectToPage();
        }
        
        public IActionResult OnPostDelete(int brokerId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage();
            }
            
            var action = Request.Form["action"];

            if (action == "cancel")
            {
                return RedirectToPage();
            }
            _data.DeleteBroker(brokerId);
            return RedirectToPage();
        }

        public IActionResult OnPostUpdateButton(int id)
        {
            ModelState.Clear();
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };

            Response.Cookies.Append("selectedBrokerId", id.ToString(), options);

            var broker = _data.GetBrokerName(id);
            
            Input = new AddBrokerModel
            {
                Name = broker,
            };
            
            IsEditdBrokerAction = true;
            LoadBrokers();
            return Page();
        }
        
        public async Task<IActionResult> OnPostUpdate()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage();
            }
            
            var action = Request.Form["action"];

            if (action == "cancel")
            {
                return RedirectToPage();
            }
            
            bool error = false;
            
            int id = -1;
            if (Request.Cookies.ContainsKey("selectedBrokerId"))
            {
                id = int.Parse(Request.Cookies["selectedBrokerId"]);
            }
            else
            {
                return RedirectToPage();
            }
            
            
            if (!ModelState.IsValid)
            {
                error = true;
            } else 
            
            if (_data.BrokerExists(id, Input.Name, _data.GetCategory(id)))
            {
                ModelState.AddModelError("Input.Name", "Ce nom existe déjà.");
                error = true;
            }
            
            LoadBrokers();
            if (error)
            {
                IsEditdBrokerAction = true;
                return Page();
            }
            
            await _data.UpdateBroker(id, Input.Name);
            return RedirectToPage();
        }
        
        private void LoadBrokers()
        {
            BrokersLists.Clear();

            SelectedCategoryMethod();
            var products = _data.GetBrokers(SelectedCategory, Thread.CurrentThread.CurrentCulture.Name);
            foreach (var product in products)
            {
                BrokersLists.Add(new BrokerViewModel(
                        Id: product.Id,
                        Name: product.Name,
                        Path: product.Path
                    )
                ); 
            }
            
            var general = _data.GetGeneralBrokers(Thread.CurrentThread.CurrentCulture.Name);
            foreach (var product in general)
            {
                BrokersGeneralLists.Add(new BrokerViewModel(
                        Id: product.Id,
                        Name: product.Name,
                        Path: product.Path
                    )
                ); 
                
                
            }
            
        }
        
        public void OnPostAddBroker()
        {
            ModelState.Clear();
            IsAddBrokerAction = true;
            LoadBrokers();
        }

        public async Task<IActionResult> OnPostAdd()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage();
            }
            
            var action = Request.Form["action"];

            if (action == "cancel")
            {
                return RedirectToPage();
            }
            
            bool error = false;
            if ( !ModelState.IsValid)
            {
                error = true;
            }
            else if (_data.BrokerExists(-1, Input.Name, Input.Category))
            {
                ModelState.AddModelError("Input.Name", "Ce nom existe déjà.");
                error = true;
            }
            
            if ( Input.Category == 0 )
            {
                ModelState.AddModelError("Category", "La catégorie est requise.");
                error = true;
            }
            
            if (Input.Pdf == null)
            {
                ModelState.AddModelError("Pdf", "Le fichier est requis.");
                error = true;
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    Input.Pdf.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();

                    if (!ImagesVerification.Pdf(fileBytes))
                    {
                        ModelState.AddModelError("Pdf", "Le fichier doit être un fichier PDF.");
                        error = true;
                    }

                    const int maxFileSizeInBytes = 20 * 1024 * 1024;
                    if (fileBytes.Length > maxFileSizeInBytes)
                    {
                        ModelState.AddModelError("Pdf",
                            "Le fichier est trop volumineux. La taille maximale autorisée est de 20 Mo.");
                        error = true;
                    }
                }
            }

            if ( error )
            {
                IsAddBrokerAction = true;
                LoadBrokers();
                return Page();
            }
            
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Brokers");
            
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            
            var fileName = Path.GetFileNameWithoutExtension(Input.Name) + Path.GetExtension(Input.Pdf.FileName);
            var filePath = Path.Combine(uploadPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Input.Pdf.CopyTo(stream);
            }
            
            await _data.AddBroker(new Domains.Broker()
            {
                Name = Input.Name,
                Category = Input.Category,
                Path = "/Images/Brokers/" + fileName, 
            });
            
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetSwitch(string direction, int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage();
            }
            
            await _data.SwitchPriority(id, direction);
            return RedirectToPage();
        }

        private void SelectedCategoryMethod()
        {
            if (Request.Cookies.ContainsKey("selectedCategory"))
            {
                SelectedCategory = int.Parse(Request.Cookies["selectedCategory"]);
            }
            else
            {
                SelectedCategory = 1;
            }
        }

        public record BrokerViewModel
        (
            int Id,
            string Path,
            string Name
        );
        
        public class AddBrokerModel
        {
            [Required(ErrorMessage = "Le nom est requis.")]
            [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères.")]
            public string Name { get; set; }
            
            public IFormFile Pdf { get; set; }
            
            public int Category { get; set; }
        
        }
    }
}