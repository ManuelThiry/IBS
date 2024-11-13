using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBS_Europe.Infrastructures.Migrations
{
    public partial class firstInserts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Website = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translator",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    IsChecked = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translator", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Brokers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    TranslatorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brokers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brokers_Translator_TranslatorId",
                        column: x => x.TranslatorId,
                        principalTable: "Translator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Email",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmailAddress = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    FirstTranslatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondTranslatorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Email_Translator_FirstTranslatorId",
                        column: x => x.FirstTranslatorId,
                        principalTable: "Translator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Email_Translator_SecondTranslatorId",
                        column: x => x.SecondTranslatorId,
                        principalTable: "Translator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Informations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstTranslatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondTranslatorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Informations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Informations_Translator_FirstTranslatorId",
                        column: x => x.FirstTranslatorId,
                        principalTable: "Translator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Informations_Translator_SecondTranslatorId",
                        column: x => x.SecondTranslatorId,
                        principalTable: "Translator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    TranslatorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.ForeignKey(
                        name: "FK_People_Translator_TranslatorId",
                        column: x => x.TranslatorId,
                        principalTable: "Translator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    TranslatorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Translator_TranslatorId",
                        column: x => x.TranslatorId,
                        principalTable: "Translator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 1, "AG Insurance", "https://www.ibseurope.com/images/suppliers/AG.jpg", 1, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 2, "CFDP", "https://www.ibseurope.com/images/suppliers/cfdp.jpg", 2, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 3, "Allianz", "https://www.ibseurope.com/images/suppliers/allianz.jpg", 3, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 4, "EuroCaution", "https://www.ibseurope.com/images/suppliers/eurocaution.jpg", 4, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 5, "AXA", "https://www.ibseurope.com/images/suppliers/axa.jpg", 5, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 6, "Europ Assistance", "https://www.ibseurope.com/images/suppliers/europ-assistance.jpg", 6, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 7, "ACE Europe", "https://www.ibseurope.com/images/suppliers/ace-europe.jpg", 7, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 8, "JEAN VERHEYEN", "https://www.ibseurope.com/images/suppliers/verheyen.jpg", 8, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 9, "Generali Group", "https://www.ibseurope.com/images/suppliers/generali.jpg", 9, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 10, "Foyer", "https://www.ibseurope.com/images/suppliers/foyer.jpg", 10, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 11, "Lalux", "https://www.ibseurope.com/images/suppliers/lalux.jpg", 11, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 12, "Baloise", "https://www.ibseurope.com/images/suppliers/labaloise.jpg", 12, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 13, "Protegys", "https://www.ibseurope.com/images/suppliers/protegys.jpg", 13, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Partners",
                columns: new[] { "Id", "Name", "Path", "Priority", "Website" },
                values: new object[] { 14, "April", "https://www.ibseurope.com/images/suppliers/aprilinternational.jpg", 14, "https://www.ibseurope.com/fr/index.cfm" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 1, true, "<h2>What is JuriPASS?</h2>\n\n<p><strong>Unique contract in Belgium!</strong><br> Your insured person, their spouse, and anyone living regularly under their roof, benefit from Legal Assistance and Protection: in the context of their private life;\nin case of a dispute with their employer;\n\nas a homeowner or tenant of their primary or secondary residence. Rental properties (including recovery of unpaid rents) as an option;\n\nin case of disputes concerning their health;\n\nfor their vehicle(s) (Option);\n\n<br> <br> <strong>Coverage applies to both extracontractual and contractual disputes.</strong><br> SOME EXAMPLES: <br> After surgery, complications arise…\nThe dream vacation apartment… is in the middle of a construction site.\n\nYour client is subject to wrongful termination.\n\nThe mechanic forgets to refill the oil in the vehicle’s engine.\n\nA tenant of your client damages the property or hasn't paid rent for three months.\n\n<br><br> <strong>If you strictly follow our procedure and take no initiative without prior consultation with us:</strong><br> NO DEDUCTIBLE,\n\nNO WAITING PERIOD OR QUALIFYING PERIOD!\n\nOur base premium: €131.96 (adjusted with the \"Abex\" index).\n\n</p>" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 2, true, "Chief Executive Officer" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 3, true, "President" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 4, true, "Product Underwriter" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 5, true, "Junior Underwriter" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 6, true, "Insurance Advisor" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 7, true, "Maestria 2010" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 8, true, "Interim 2010" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 9, true, "Interim 2011" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 10, true, "Interim 2012" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 11, true, "Interim 2013" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 12, true, "Information" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 13, true, "You would like information regarding the products." });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 14, true, "Claims" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 15, true, "You would like to report a claim." });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 16, true, "Others" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 17, true, "You would like to make a specific request." });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 18, true, "Adress" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 19, true, "68 route de Luxembourg, L-4972 Dippach, Luxembourg" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 20, true, "BE" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 21, true, "+32-4 259 76 72" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 22, true, "LUX" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 23, true, "+352-26 31 06 11-1" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 24, true, "Email" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 25, true, "info@ibseurope.com" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 26, true, "Schedule" });

            migrationBuilder.InsertData(
                table: "Translator",
                columns: new[] { "Id", "IsChecked", "Text" },
                values: new object[] { 27, true, "Monday - Friday: 9 AM - 6 PM" });

            migrationBuilder.InsertData(
                table: "Brokers",
                columns: new[] { "Id", "Category", "Name", "Path", "Priority", "TranslatorId" },
                values: new object[] { 1, 1, "Maestria 2010", "pdf/2010-contrat-conseiller-maestria.pdf", 1, 7 });

            migrationBuilder.InsertData(
                table: "Brokers",
                columns: new[] { "Id", "Category", "Name", "Path", "Priority", "TranslatorId" },
                values: new object[] { 2, 2, "Interim 2010", "pdf/2010-convention-interim-assurances.pdf", 1, 8 });

            migrationBuilder.InsertData(
                table: "Brokers",
                columns: new[] { "Id", "Category", "Name", "Path", "Priority", "TranslatorId" },
                values: new object[] { 3, 3, "Interim 2011", "pdf/2010-convention-interim-assurances.pdf", 1, 9 });

            migrationBuilder.InsertData(
                table: "Brokers",
                columns: new[] { "Id", "Category", "Name", "Path", "Priority", "TranslatorId" },
                values: new object[] { 4, 3, "Interim 2012", "pdf/2010-convention-interim-assurances.pdf", 2, 10 });

            migrationBuilder.InsertData(
                table: "Brokers",
                columns: new[] { "Id", "Category", "Name", "Path", "Priority", "TranslatorId" },
                values: new object[] { 5, 3, "Interim 2013", "pdf/2010-convention-interim-assurances.pdf", 3, 11 });

            migrationBuilder.InsertData(
                table: "Email",
                columns: new[] { "Id", "Description", "EmailAddress", "FirstTranslatorId", "Name", "SecondTranslatorId" },
                values: new object[] { 1, "Vous souhaitez des informations concernant les produits.", "affaires@ibseurope.com", 12, "Information", 13 });

            migrationBuilder.InsertData(
                table: "Email",
                columns: new[] { "Id", "Description", "EmailAddress", "FirstTranslatorId", "Name", "SecondTranslatorId" },
                values: new object[] { 2, "Vous souhaitez annoncer un sinistre.", "sinistre@ibseurope.com", 14, "Sinistres", 15 });

            migrationBuilder.InsertData(
                table: "Email",
                columns: new[] { "Id", "Description", "EmailAddress", "FirstTranslatorId", "Name", "SecondTranslatorId" },
                values: new object[] { 3, "Vous souhaitez faire une demande particulière.", "info@ibseurope.com", 16, "Autres", 17 });

            migrationBuilder.InsertData(
                table: "Informations",
                columns: new[] { "Id", "Description", "FirstTranslatorId", "Priority", "SecondTranslatorId", "Text", "Type" },
                values: new object[] { 1, "Adresse", 18, 5, 19, "68 route de Luxembourg, L-4972 Dippach, Luxembourg", 3 });

            migrationBuilder.InsertData(
                table: "Informations",
                columns: new[] { "Id", "Description", "FirstTranslatorId", "Priority", "SecondTranslatorId", "Text", "Type" },
                values: new object[] { 2, "BE", 20, 4, 21, "+32-4 259 76 72", 1 });

            migrationBuilder.InsertData(
                table: "Informations",
                columns: new[] { "Id", "Description", "FirstTranslatorId", "Priority", "SecondTranslatorId", "Text", "Type" },
                values: new object[] { 3, "LUX", 22, 3, 23, "+352-26 31 06 11-1", 1 });

            migrationBuilder.InsertData(
                table: "Informations",
                columns: new[] { "Id", "Description", "FirstTranslatorId", "Priority", "SecondTranslatorId", "Text", "Type" },
                values: new object[] { 4, "Email", 24, 2, 25, "info@ibseurope.com", 2 });

            migrationBuilder.InsertData(
                table: "Informations",
                columns: new[] { "Id", "Description", "FirstTranslatorId", "Priority", "SecondTranslatorId", "Text", "Type" },
                values: new object[] { 5, "Horaire", 26, 1, 27, "Lundi - Vendredi : 9h - 18h", 4 });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Path", "Phone", "Priority", "Role", "TranslatorId" },
                values: new object[] { 1, "gdm@ibseurope.com", "Gael", "de Miomandre", "https://i.pinimg.com/564x/53/76/31/53763136436d736e99c915f41f0ce25d.jpg", "0412345678", 1, "Administrateur Délégué", 2 });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Path", "Phone", "Priority", "Role", "TranslatorId" },
                values: new object[] { 2, "adm@ibseurope.com", "Alain", "de Miomandre", "https://avatarfiles.alphacoders.com/326/thumb-1920-326625.jpg", "0412345678", 2, "Président", 3 });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Path", "Phone", "Priority", "Role", "TranslatorId" },
                values: new object[] { 3, "pap@ibseurope.com", "Patrice", "Penders", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSkM7w_sYTtWDdtd18g--vJQXR4RxexU_pxlw&s", "0412345678", 3, "Souscripteur de produits", 4 });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Path", "Phone", "Priority", "Role", "TranslatorId" },
                values: new object[] { 4, "sat@ibseurope.com", "Salvatore", "Tomasello", "https://i.pinimg.com/564x/9b/ed/ac/9bedac5d6b820b0ead1810bc3551aa5e.jpg", "0412345678", 4, "Souscripteur junior", 5 });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Path", "Phone", "Priority", "Role", "TranslatorId" },
                values: new object[] { 5, "mac@ibseurope.com", "Mathieu", "Clicq", "https://www.cartoonize.net/wp-content/uploads/2024/05/avatar-maker-photo-to-cartoon.png", "0412345678", 5, "Conseiller assurance", 6 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Path", "Text", "TranslatorId" },
                values: new object[] { 1, "JuriPASS", "images/IBS-logo-bleu-2_HD.JPG", "<h2>Qu'est ce que JuriPASS ?</h2>\n            <p><strong>Contrat unique en Belgique !</strong><br>\n                Votre assuré, son conjoint et toute personne\n                vivant habituellement sous son toit,\n                bénéficient d'une Assistance et d'une Protection Juridique :\n                dans le cadre de leur vie privée ;\n               \n                en cas de litige avec leur employeur ;\n               \n                en leur qualité de propriétaire ou de locataire\n                de leur résidence principale ou secondaire.\n                Les biens donnés en location (y compris le recouvrement\n                des loyers impayés) en option.\n               \n                dans le cas de litiges qui concernent leur santé ;\n               \n                pour leur(s) véhicule(s) (Option) ;\n               \n                <br>\n                <br>\n                <strong>Les garanties sont acquises pour les litiges extra contractuels ET contractuels. </strong><br>\n                QUELQUES EXEMPLES : <br>\n                Après une opération, des complications surviennent…\n               \n                L'appartement de rêve des vacances… est au milieu d'un chantier.\n               \n                Votre client fait l'objet d’un licenciement abusif.\n               \n                Le garagiste oublie de remettre de l'huile dans le carter de son véhicule.\n               \n                Un locataire de votre client dégrade le bien ou n’a pas payé son loyer depuis trois mois.\n               \n                <br><br>\n                <strong>Si vous respectez scrupuleusement notre procédure et ne prenez aucune initiative sans concertation préalable avec nous : </strong><br>\n                PAS DE FRANCHISE,\n               \n                PAS DE DELAI D'ATTENTE OU DE CARENCE !\n               \n                Notre prime de base : 131.96 € (évolue avec l'indice « Abex »)\n            </p>", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brokers_TranslatorId",
                table: "Brokers",
                column: "TranslatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Email_FirstTranslatorId",
                table: "Email",
                column: "FirstTranslatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Email_SecondTranslatorId",
                table: "Email",
                column: "SecondTranslatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Informations_FirstTranslatorId",
                table: "Informations",
                column: "FirstTranslatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Informations_SecondTranslatorId",
                table: "Informations",
                column: "SecondTranslatorId");

            migrationBuilder.CreateIndex(
                name: "IX_People_TranslatorId",
                table: "People",
                column: "TranslatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TranslatorId",
                table: "Products",
                column: "TranslatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Brokers");

            migrationBuilder.DropTable(
                name: "Email");

            migrationBuilder.DropTable(
                name: "Informations");

            migrationBuilder.DropTable(
                name: "Partners");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Translator");
        }
    }
}
