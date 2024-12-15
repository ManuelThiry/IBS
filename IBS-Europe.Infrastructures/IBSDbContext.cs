using IBS_Europe.Infrastructures.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IBS_Europe.Infrastructures;

public class IBSDbContext : IdentityDbContext
{
    public IBSDbContext(DbContextOptions<IBSDbContext> options) : base(options)
    {
    }

    public DbSet<Products> Products { get; set; }
    public DbSet<People> People { get; set; }
    public DbSet<Partners> Partners { get; set; }
    
    public DbSet<Broker> Brokers { get; set; }
    
    public DbSet<Email> Email { get; set; }
    
    public DbSet<Informations> Informations { get; set; }
    
    public DbSet<Translator> Translator { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Products>()
            .Property(p => p.Name)
            .IsRequired();
        
        modelBuilder.Entity<Products>()
            .Property(p => p.Text)
            .IsRequired();
        
        modelBuilder.Entity<People>()
            .Property(p => p.FirstName)
            .IsRequired();
        
        modelBuilder.Entity<People>()
            .Property(p => p.Priority)
            .IsRequired();
        
        modelBuilder.Entity<People>()
            .Property(p => p.Email)
            .IsRequired();
        
        modelBuilder.Entity<People>()
            .Property(p => p.Role)
            .IsRequired();
        
        modelBuilder.Entity<Partners>()
            .Property(p => p.Name)
            .IsRequired();
        
        modelBuilder.Entity<Partners>()
            .Property(p => p.Priority)
            .IsRequired();
        
        modelBuilder.Entity<Broker>()
            .Property(p => p.Name)
            .IsRequired();
        
        modelBuilder.Entity<Broker>()
            .Property(p => p.Category)
            .IsRequired();
        
        modelBuilder.Entity<Broker>()
            .Property(p => p.Path)
            .IsRequired();
        
        modelBuilder.Entity<Broker>()
            .Property(p => p.Priority)
            .IsRequired();
        
        modelBuilder.Entity<Email>()
            .Property(p => p.Name)
            .IsRequired();
        
        modelBuilder.Entity<Email>()
            .Property(p => p.Description)
            .IsRequired();
        
        modelBuilder.Entity<Email>()
            .Property(p => p.EmailAddress)
            .IsRequired();
        
        modelBuilder.Entity<Informations>()
            .Property(p => p.Text)
            .IsRequired();
        
        modelBuilder.Entity<Informations>()
            .Property(p => p.Description)
            .IsRequired();
        
        modelBuilder.Entity<Informations>()
            .Property(p => p.Priority)
            .IsRequired();
        
        modelBuilder.Entity<Translator>()
            .Property(p => p.Text)
            .IsRequired();
        
        modelBuilder.Entity<Translator>()
            .Property(p => p.IsChecked)
            .HasDefaultValue(0);
        
        
        modelBuilder.Entity<Products>().HasData(
            new Products() { Id = 1, FirstTranslatorId = 1, SmallDescription = "Test" , SecondTranslatorId = 28, Path= "images/IBS-logo-bleu-2_HD.JPG", Name = "JuriPASS", Text = "<h2>Qu'est ce que JuriPASS ?</h2>\n            <p><strong>Contrat unique en Belgique !</strong><br>\n                Votre assuré, son conjoint et toute personne\n                vivant habituellement sous son toit,\n                bénéficient d'une Assistance et d'une Protection Juridique :\n                dans le cadre de leur vie privée ;\n               \n                en cas de litige avec leur employeur ;\n               \n                en leur qualité de propriétaire ou de locataire\n                de leur résidence principale ou secondaire.\n                Les biens donnés en location (y compris le recouvrement\n                des loyers impayés) en option.\n               \n                dans le cas de litiges qui concernent leur santé ;\n               \n                pour leur(s) véhicule(s) (Option) ;\n               \n                <br>\n                <br>\n                <strong>Les garanties sont acquises pour les litiges extra contractuels ET contractuels. </strong><br>\n                QUELQUES EXEMPLES : <br>\n                Après une opération, des complications surviennent…\n               \n                L'appartement de rêve des vacances… est au milieu d'un chantier.\n               \n                Votre client fait l'objet d’un licenciement abusif.\n               \n                Le garagiste oublie de remettre de l'huile dans le carter de son véhicule.\n               \n                Un locataire de votre client dégrade le bien ou n’a pas payé son loyer depuis trois mois.\n               \n                <br><br>\n                <strong>Si vous respectez scrupuleusement notre procédure et ne prenez aucune initiative sans concertation préalable avec nous : </strong><br>\n                PAS DE FRANCHISE,\n               \n                PAS DE DELAI D'ATTENTE OU DE CARENCE !\n               \n                Notre prime de base : 131.96 \u20ac (évolue avec l'indice « Abex »)\n            </p>" }
        );
        
        modelBuilder.Entity<People>().HasData(
            new People() { Id = 1, TranslatorId = 2, SecondPhone = "",FirstName = "Gael", LastName = "de Miomandre", Phone = "0412345678", Role = "Administrateur Délégué", Email = "gdm@ibseurope.com", Path = "https://i.pinimg.com/564x/53/76/31/53763136436d736e99c915f41f0ce25d.jpg", Priority = 1},
            new People() { Id = 2, TranslatorId = 3, SecondPhone = "", FirstName = "Alain", LastName = "de Miomandre", Phone = "0412345678", Role = "Président", Email = "adm@ibseurope.com", Path = "https://avatarfiles.alphacoders.com/326/thumb-1920-326625.jpg", Priority = 2},
            new People() { Id = 3, TranslatorId = 4, SecondPhone = "", FirstName = "Patrice", LastName = "Penders", Phone = "0412345678", Role = "Souscripteur de produits", Email = "pap@ibseurope.com", Path = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSkM7w_sYTtWDdtd18g--vJQXR4RxexU_pxlw&s", Priority = 3},
            new People() { Id = 4, TranslatorId = 5,  SecondPhone = "", FirstName = "Salvatore", LastName = "Tomasello", Phone = "0412345678",  Role = "Souscripteur junior", Email = "sat@ibseurope.com", Path = "https://i.pinimg.com/564x/9b/ed/ac/9bedac5d6b820b0ead1810bc3551aa5e.jpg", Priority = 4},
            new People() { Id = 5, TranslatorId = 6, SecondPhone = "", FirstName = "Mathieu", LastName = "Clicq", Phone = "0412345678", Role = "Conseiller assurance", Email = "mac@ibseurope.com", Path = "https://www.cartoonize.net/wp-content/uploads/2024/05/avatar-maker-photo-to-cartoon.png", Priority = 5}
        );

        modelBuilder.Entity<Partners>().HasData(
            new Partners() { Id = 1, Category = 1, Name = "AG Insurance", Path = "https://www.ibseurope.com/images/suppliers/AG.jpg", Priority = 1, Website = "https://www.ibseurope.com/fr/index.cfm"},
            new Partners() { Id = 2, Category = 1,Name = "CFDP", Path = "https://www.ibseurope.com/images/suppliers/cfdp.jpg", Priority = 2, Website = "https://www.ibseurope.com/fr/index.cfm"},
            new Partners() { Id = 3, Category = 1,Name = "Allianz", Path = "https://www.ibseurope.com/images/suppliers/allianz.jpg", Priority = 3, Website = "https://www.ibseurope.com/fr/index.cfm"},
            new Partners() { Id = 4, Category = 1,Name = "EuroCaution", Path = "https://www.ibseurope.com/images/suppliers/eurocaution.jpg", Priority = 4, Website = "https://www.ibseurope.com/fr/index.cfm"},
            new Partners() { Id = 5, Category = 1,Name = "AXA", Path = "https://www.ibseurope.com/images/suppliers/axa.jpg", Priority = 5, Website = "https://www.ibseurope.com/fr/index.cfm"},
            new Partners() { Id = 6, Category = 1,Name = "Europ Assistance", Path = "https://www.ibseurope.com/images/suppliers/europ-assistance.jpg", Priority = 6, Website = "https://www.ibseurope.com/fr/index.cfm" },
            new Partners() { Id = 7, Category = 1,Name = "ACE Europe", Path = "https://www.ibseurope.com/images/suppliers/ace-europe.jpg", Priority = 7, Website = "https://www.ibseurope.com/fr/index.cfm"},
            new Partners() { Id = 8, Category = 1,Name = "JEAN VERHEYEN", Path = "https://www.ibseurope.com/images/suppliers/verheyen.jpg", Priority = 8, Website = "https://www.ibseurope.com/fr/index.cfm"},
            new Partners() { Id = 9, Category = 1,Name = "Generali Group", Path = "https://www.ibseurope.com/images/suppliers/generali.jpg", Priority = 9, Website = "https://www.ibseurope.com/fr/index.cfm"},
            new Partners() { Id = 10,Category = 1, Name = "Foyer", Path = "https://www.ibseurope.com/images/suppliers/foyer.jpg", Priority = 10, Website = "https://www.ibseurope.com/fr/index.cfm" },
            new Partners() { Id = 11, Category = 1,Name = "Lalux", Path = "https://www.ibseurope.com/images/suppliers/lalux.jpg", Priority = 11, Website = "https://www.ibseurope.com/fr/index.cfm" },
            new Partners() { Id = 12, Category = 1,Name = "Baloise", Path = "https://www.ibseurope.com/images/suppliers/labaloise.jpg", Priority = 12, Website = "https://www.ibseurope.com/fr/index.cfm"},
            new Partners() { Id = 13, Category = 1,Name = "Protegys", Path = "https://www.ibseurope.com/images/suppliers/protegys.jpg", Priority = 13, Website = "https://www.ibseurope.com/fr/index.cfm" },
            new Partners() { Id = 14, Category = 1,Name = "April", Path = "https://www.ibseurope.com/images/suppliers/aprilinternational.jpg", Priority = 14, Website = "https://www.ibseurope.com/fr/index.cfm" }
        );

        modelBuilder.Entity<Broker>().HasData(
            new Broker() {Id = 1, TranslatorId = 7, Name = "Maestria 2010", Path = "pdf/2010-contrat-conseiller-maestria.pdf", Category = 1, Priority = 1},
            new Broker() {Id = 2, TranslatorId = 8, Name = "Interim 2010", Path = "pdf/2010-convention-interim-assurances.pdf", Category = 2, Priority = 1},
            new Broker() {Id = 3, TranslatorId = 9, Name = "Interim 2011", Path = "pdf/2010-convention-interim-assurances.pdf", Category = 3, Priority = 1},
            new Broker() {Id = 4, TranslatorId = 10, Name = "Interim 2012", Path = "pdf/2010-convention-interim-assurances.pdf", Category = 3, Priority = 2},
            new Broker() {Id = 5, TranslatorId = 11, Name = "Interim 2013", Path = "pdf/2010-convention-interim-assurances.pdf", Category = 3, Priority = 3}
        );

        modelBuilder.Entity<Email>().HasData(
            new Email() { Id = 1, FirstTranslatorId = 12, SecondTranslatorId = 13, EmailAddress = "affaires@ibseurope.com", Name = "Information", Description = "Vous souhaitez des informations concernant les produits." },
            new Email() { Id = 2, FirstTranslatorId = 14, SecondTranslatorId = 15, EmailAddress = "sinistre@ibseurope.com", Name = "Sinistres", Description = "Vous souhaitez annoncer un sinistre." },
            new Email() { Id = 3, FirstTranslatorId = 16, SecondTranslatorId = 17, EmailAddress = "info@ibseurope.com", Name = "Autres", Description = "Vous souhaitez faire une demande particulière."}
        );
        
        modelBuilder.Entity<Informations>().HasData(
            new Informations() { Id = 1, FirstTranslatorId = 18, SecondTranslatorId = 19, Description = "Adresse", Text = "68 route de Luxembourg, L-4972 Dippach, Luxembourg", Type = 3 , Priority = 5},
            new Informations() { Id = 2, FirstTranslatorId = 20, SecondTranslatorId = 21,  Description = "BE", Text = "+32-4 259 76 72", Type = 1, Priority = 4},
            new Informations() { Id = 3, FirstTranslatorId = 22, SecondTranslatorId = 23,  Description = "LUX", Text = "+352-26 31 06 11-1", Type = 1, Priority = 3},
            new Informations() { Id = 4, FirstTranslatorId = 24, SecondTranslatorId = 25,  Description = "Email", Text = "info@ibseurope.com", Type = 2, Priority = 2},
            new Informations() { Id = 5, FirstTranslatorId = 26, SecondTranslatorId = 27,  Description = "Horaire", Text = "Lundi - Vendredi : 9h - 18h", Type = 4, Priority = 1}
        );
        
        modelBuilder.Entity<Translator>().HasData(
            new Translator { Id = 1, Text = "<h2>What is JuriPASS?</h2>\n\n<p><strong>Unique contract in Belgium!</strong><br> Your insured person, their spouse, and anyone living regularly under their roof, benefit from Legal Assistance and Protection: in the context of their private life;\nin case of a dispute with their employer;\n\nas a homeowner or tenant of their primary or secondary residence. Rental properties (including recovery of unpaid rents) as an option;\n\nin case of disputes concerning their health;\n\nfor their vehicle(s) (Option);\n\n<br> <br> <strong>Coverage applies to both extracontractual and contractual disputes.</strong><br> SOME EXAMPLES: <br> After surgery, complications arise…\nThe dream vacation apartment… is in the middle of a construction site.\n\nYour client is subject to wrongful termination.\n\nThe mechanic forgets to refill the oil in the vehicle’s engine.\n\nA tenant of your client damages the property or hasn't paid rent for three months.\n\n<br><br> <strong>If you strictly follow our procedure and take no initiative without prior consultation with us:</strong><br> NO DEDUCTIBLE,\n\nNO WAITING PERIOD OR QUALIFYING PERIOD!\n\nOur base premium: \u20ac131.96 (adjusted with the \"Abex\" index).\n\n</p>", IsChecked = true},
            new Translator { Id = 2, Text = "Chief Executive Officer", IsChecked = true},
            new Translator { Id = 3, Text = "President", IsChecked = true},
            new Translator { Id = 4, Text = "Product Underwriter", IsChecked = true},
            new Translator { Id = 5, Text = "Junior Underwriter", IsChecked = true},
            new Translator { Id = 6, Text = "Insurance Advisor", IsChecked = true},
            new Translator { Id = 7, Text = "Maestria 2010", IsChecked = true},
            new Translator { Id = 8, Text = "Interim 2010", IsChecked = true},
            new Translator { Id = 9, Text = "Interim 2011", IsChecked = true},
            new Translator { Id = 10, Text = "Interim 2012", IsChecked = true},
            new Translator { Id = 11, Text = "Interim 2013", IsChecked = true},
            new Translator { Id = 12, Text = "Information", IsChecked = true},
            new Translator { Id = 13, Text = "You would like information regarding the products.", IsChecked = true},
            new Translator { Id = 14, Text = "Claims", IsChecked = true},
            new Translator { Id = 15, Text = "You would like to report a claim.", IsChecked = true},
            new Translator { Id = 16, Text = "Others", IsChecked = true},
            new Translator { Id = 17, Text = "You would like to make a specific request.", IsChecked = true},
            new Translator { Id = 18, Text = "Adress", IsChecked = true},
            new Translator { Id = 19, Text = "68 route de Luxembourg, L-4972 Dippach, Luxembourg", IsChecked = true},
            new Translator { Id = 20, Text = "BE", IsChecked = true},
            new Translator { Id = 21, Text = "+32-4 259 76 72", IsChecked = true},
            new Translator { Id = 22, Text = "LUX", IsChecked = true},
            new Translator { Id = 23, Text = "+352-26 31 06 11-1", IsChecked = true},
            new Translator { Id = 24, Text = "Email", IsChecked = true},
            new Translator { Id = 25, Text = "info@ibseurope.com", IsChecked = true},
            new Translator { Id = 26, Text = "Schedule", IsChecked = true},
            new Translator { Id = 27, Text = "Monday - Friday: 9 AM - 6 PM", IsChecked = true},
            new Translator { Id = 28, Text = "Test", IsChecked = true}
        );



    }

}