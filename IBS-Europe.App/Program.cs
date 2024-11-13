using IBS_Europe.Domains;
using IBS_Europe.Domains.Repository;
using IBS_Europe.Infrastructures;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using IBS_Europe.App.Pages.Shared.Email;
using IBS_Europe.Domains.Translation;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<EmailService>();


builder.Services.AddScoped<IProductsData, ProductsData>();
builder.Services.AddScoped<IPeopleData, PeopleData>();
builder.Services.AddScoped<IPartnersData, PartnersData>();
builder.Services.AddScoped<IBrokerData, BrokerData>();
builder.Services.AddScoped<IContactData, ContactData>();
builder.Services.AddScoped<IProfileData, ProfileData>();
builder.Services.AddTransient<ITranslatorData, TranslatorData>();

var supportedCultures = new[]
{
    new CultureInfo("fr-FR"),
    new CultureInfo("en-US")
};

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Ajouter le service de contexte de base de données
builder.Services.AddDbContext<IBSDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IBSDbContext>()
    .AddDefaultTokenProviders();

// Configurer les options de localisation
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("fr-FR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.ApplyCurrentCultureToResponseHeaders = true;

    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configurer la localisation des vues et des annotations
builder.Services.AddRazorPages()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddControllersWithViews(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

// Le reste de tes services et configurations...

var app = builder.Build();
var deeplSettings = builder.Configuration.GetSection("DeeplSettings").Get<DeeplSettings>();
DeeplTranslate.Init(deeplSettings);


// Appliquer les options de localisation
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    builder.Configuration.AddUserSecrets<Program>();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Configurer les routes
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{culture=en-US}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapRazorPages();
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Récupérer les services Identity nécessaires pour le seeding
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        // Appeler la méthode Seed pour ajouter l'utilisateur par défaut
        await IBSDbContextSeed.SeedDefaultUserAsync(userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
app.Run();