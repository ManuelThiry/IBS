using System.ComponentModel.DataAnnotations;
using IBS_Europe.App.Resources;
using IBS_Europe.Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public async Task OnGet()
        {
            await LoadBrokers();
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

        public async Task<IActionResult> OnPostUpdateButton(int id)
        {
            ModelState.Clear();
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };

            Response.Cookies.Append("selectedBrokerId", id.ToString(), options);

            var broker = await _data.GetBrokerName(id);
            var product = await _data.GetProduct(id);
            
            Input = new AddBrokerModel
            {
                Name = broker,
                Product = product
            };
            
            IsEditdBrokerAction = true;
            await LoadBrokers();
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
            
            if (await _data.BrokerExists(id, Input.Name, await _data.GetCategory(id)))
            {
                ModelState.AddModelError("Name", SharedResource.B_Exist);
                error = true;
            }
            
            await LoadBrokers();
            if (error)
            {
                IsEditdBrokerAction = true;
                return Page();
            }
            
            await _data.UpdateBroker(id, Input.Name, Input.Product);
            return RedirectToPage();
        }
        
        private async Task LoadBrokers()
        {
            BrokersLists.Clear();

            SelectedCategoryMethod();
            var products = await _data.GetBrokers(SelectedCategory, Thread.CurrentThread.CurrentCulture.Name);
            foreach (var product in products)
            {
                BrokersLists.Add(new BrokerViewModel(
                        Id: product.Id,
                        Name: product.Name,
                        Path: product.Path
                    )
                ); 
            }
            
            var general = await _data.GetGeneralBrokers(Thread.CurrentThread.CurrentCulture.Name);
            foreach (var product in general)
            {
                BrokersGeneralLists.Add(new BrokerViewModel(
                        Id: product.Id,
                        Name: product.Name,
                        Path: product.Path
                    )
                ); 
                
                
            }
            ViewData["ProductsList"] = await _data.GetProductsList();
            
        }
        
        public async Task OnPostAddBroker()
        {
            ModelState.Clear();
            IsAddBrokerAction = true;
            await LoadBrokers();
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
            else if (await _data.BrokerExists(-1, Input.Name, Input.Category))
            {
                ModelState.AddModelError("Name", SharedResource.B_Exist);
                error = true;
            }
            
            if ( Input.Category == 0 )
            {
                ModelState.AddModelError("Category", SharedResource.B_CR);
                error = true;
            }
            
            if (Input.Pdf == null)
            {
                ModelState.AddModelError("Pdf", SharedResource.B_FR);
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
                        ModelState.AddModelError("Pdf", SharedResource.B_FFormat);
                        error = true;
                    }

                    const int maxFileSizeInBytes = 20 * 1024 * 1024;
                    if (fileBytes.Length > maxFileSizeInBytes)
                    {
                        ModelState.AddModelError("Pdf",
                            SharedResource.B_FMAX);
                        error = true;
                    }
                }
            }

            if ( error )
            {
                IsAddBrokerAction = true;
                await LoadBrokers();
                return Page();
            }
            
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Brokers");
            
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            
            var fileName = Cleanup.GenerateUniqueFileName(Input.Pdf.FileName);
            var filePath = Path.Combine(uploadPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Input.Pdf.CopyTo(stream);
            }
            
            await _data.AddBroker(new Domains.Broker()
            {
                Name = Input.Name,
                Category = Input.Category,
                Path = "/images/Brokers/" + fileName, 
                Products = Input.Product
                
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
            [Required(ErrorMessageResourceType = typeof(SharedResource) , ErrorMessageResourceName = "B_NR")]
            [StringLength(50, ErrorMessageResourceType = typeof(SharedResource), ErrorMessageResourceName = "B_N50")]
            public string Name { get; set; }
            
            public IFormFile Pdf { get; set; }
            
            public int Category { get; set; }
            
            public string Product { get; set; }
        
        }
    }
}