using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Factory.Abstract;
using DogrudanTeminParadiseAPI.Factory.Concrete;
using DogrudanTeminParadiseAPI.Factory.Main;
using DogrudanTeminParadiseAPI.Filter;
using DogrudanTeminParadiseAPI.Helpers.Options;
using DogrudanTeminParadiseAPI.Mapping;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using DogrudanTeminParadiseAPI.Service.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;


builder.Services.AddSingleton(new HttpClient(new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
}));

cfg.AddJsonFile("superadminsettings.json", optional: false, reloadOnChange: true);
builder.Services.Configure<SuperAdminSettings>(cfg.GetSection("SuperAdminCredentials"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<SuperAdminSettings>>().Value);

builder.Services.Configure<LoggerApiOptions>(
    cfg.GetSection("LoggerApi"));

// Register HttpClient for logger
builder.Services.AddHttpClient("LoggerApi", client =>
{
    client.BaseAddress = new Uri(cfg["LoggerApi:BaseUrl"]);
});

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyClient", policy =>
    {
        policy
          .WithOrigins(cfg["ConnectionStrings:CORS"].ToString()) 
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

// MongoDBRepository kayıtları
builder.Services.AddScoped(sp => new MongoDBRepository<AdminUser>(cfg["MongoAPI"], cfg["MongoDBName"], "AdminUsers"));
builder.Services.AddScoped(sp => new MongoDBRepository<SuperAdminUser>(cfg["MongoAPI"], cfg["MongoDBName"], "SuperAdmin"));
builder.Services.AddScoped(sp => new MongoDBRepository<User>(cfg["MongoAPI"], cfg["MongoDBName"], "Users"));
builder.Services.AddScoped(sp => new MongoDBRepository<Entreprise>(cfg["MongoAPI"], cfg["MongoDBName"], "Entreprises"));
builder.Services.AddScoped(sp => new MongoDBRepository<ProductItem>(cfg["MongoAPI"], cfg["MongoDBName"], "ProductItems"));
builder.Services.AddScoped(sp => new MongoDBRepository<Product>(cfg["MongoAPI"], cfg["MongoDBName"], "Products"));
builder.Services.AddScoped(sp => new MongoDBRepository<AdministrationUnit>(cfg["MongoAPI"], cfg["MongoDBName"], "AdministrationUnits"));
builder.Services.AddScoped(sp => new MongoDBRepository<SubAdministrationUnit>(cfg["MongoAPI"], cfg["MongoDBName"], "SubAdministrationUnits"));
builder.Services.AddScoped(sp => new MongoDBRepository<ThreeSubAdministrationUnit>(cfg["MongoAPI"], cfg["MongoDBName"], "ThreeSubAdministrationUnits"));
builder.Services.AddScoped(sp => new MongoDBRepository<ProcurementEntry>(cfg["MongoAPI"], cfg["MongoDBName"], "ProcurementEntries"));
builder.Services.AddScoped(sp => new MongoDBRepository<Title>(cfg["MongoAPI"], cfg["MongoDBName"], "Titles"));
builder.Services.AddScoped(sp => new MongoDBRepository<ProcurementListItem>(cfg["MongoAPI"], cfg["MongoDBName"], "ProcurementListItems"));
builder.Services.AddScoped(sp => new MongoDBRepository<Unit>(cfg["MongoAPI"], cfg["MongoDBName"], "Units"));
builder.Services.AddScoped(sp => new MongoDBRepository<OfferLetter>(cfg["MongoAPI"], cfg["MongoDBName"], "OfferLetters"));
builder.Services.AddScoped(sp => new MongoDBRepository<BackupOfferLetter>(cfg["MongoAPI"], cfg["MongoBackupDBName"], "BackupOfferLetters"));
builder.Services.AddScoped(sp => new MongoDBRepository<Category>(cfg["MongoAPI"], cfg["MongoDBName"], "Categories"));
builder.Services.AddScoped(sp => new MongoDBRepository<BudgetItem>(cfg["MongoAPI"], cfg["MongoDBName"], "BudgetItems"));
builder.Services.AddScoped(sp => new MongoDBRepository<MarketResearchJury>(cfg["MongoAPI"], cfg["MongoDBName"], "MarketResearchJuries"));
builder.Services.AddScoped(sp => new MongoDBRepository<InspectionAcceptanceJury>(cfg["MongoAPI"], cfg["MongoDBName"], "InspectionAcceptanceJuries"));
builder.Services.AddScoped(sp => new MongoDBRepository<InspectionAcceptanceCertificate>(cfg["MongoAPI"], cfg["MongoDBName"], "InspectionAcceptanceCertificates"));
builder.Services.AddScoped(sp => new MongoDBRepository<AdditionalInspectionAcceptanceCertificate>(cfg["MongoAPI"], cfg["MongoDBName"], "AdditionalInspectionAcceptanceCertificates"));
builder.Services.AddScoped(sp => new MongoDBRepository<BackupInspectionAcceptanceJury>(cfg["MongoAPI"], cfg["MongoBackupDBName"], "BackupInspectionAcceptanceJuries"));
builder.Services.AddScoped(sp => new MongoDBRepository<BackupInspectionAcceptanceCertificate>(cfg["MongoAPI"], cfg["MongoBackupDBName"], "InspectionAcceptanceCertificates"));
builder.Services.AddScoped(sp => new MongoDBRepository<BackupAdditionalInspectionAcceptanceCertificate>(cfg["MongoAPI"], cfg["MongoBackupDBName"], "AdditionalInspectionAcceptanceCertificates"));
builder.Services.AddScoped(sp => new MongoDBRepository<ApproximateCostJury>(cfg["MongoAPI"], cfg["MongoDBName"], "ApproximateCostJuries"));
builder.Services.AddScoped(sp => new MongoDBRepository<ProcurementEntryEditor>(cfg["MongoAPI"], cfg["MongoDBName"], "ProcurementEntryEditors"));
builder.Services.AddScoped(sp => new MongoDBRepository<BackupProcurementEntry>(cfg["MongoAPI"], cfg["MongoBackupDBName"], "BackupProcurementEntries"));
builder.Services.AddScoped(sp => new MongoDBRepository<BackupProcurementEntryEditor>(cfg["MongoAPI"], cfg["MongoBackupDBName"], "BackupProcurementEntryEditors"));
builder.Services.AddScoped(sp => new MongoDBRepository<InspectionAcceptanceNote>(cfg["MongoAPI"], cfg["MongoDBName"], "InspectionAcceptanceNotes"));
builder.Services.AddScoped(sp => new MongoDBRepository<UserOwnFeaturesList>(cfg["MongoAPI"], cfg["MongoDBName"], "UserOwnFeaturesLists"));

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(cfg["MongoAPI"])
);

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(cfg["MongoDBName"]);
});

// Servisler

builder.Services.AddScoped<IEntrepriseService, EntrepriseService>();
builder.Services.AddScoped<IProductItemService, ProductItemService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAdministrationUnitService, AdministrationUnitService>();
builder.Services.AddScoped<ISubAdministrationUnitService, SubAdministrationUnitService>();
builder.Services.AddScoped<IThreeSubAdministrationUnitService, ThreeSubAdministrationUnitService>();
builder.Services.AddScoped<IProcurementEntryService, ProcurementEntryService>();
builder.Services.AddScoped<ITitleService, TitleService>();
builder.Services.AddScoped<IProcurementListItemService, ProcurementListItemService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IOfferLetterService, OfferLetterService>();
builder.Services.AddScoped<IBackupOfferLetterService, BackupOfferLetterService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBudgetItemService, BudgetItemService>();
builder.Services.AddScoped<IMarketResearchJuryService, MarketResearchJuryService>();
builder.Services.AddScoped<IInspectionAcceptanceJuryService, InspectionAcceptanceJuryService>();
builder.Services.AddScoped<IInspectionAcceptanceCertificateService, InspectionAcceptanceCertificateService>();
builder.Services.AddScoped<IAdditionalInspectionAcceptanceService, AdditionalInspectionAcceptanceService>();
builder.Services.AddScoped<IBackupInspectionAcceptanceJuryService, BackupInspectionAcceptanceJuryService>();
builder.Services.AddScoped<IBackupInspectionAcceptanceCertificateService, BackupInspectionAcceptanceCertificateService>();
builder.Services.AddScoped<IBackupAdditionalInspectionAcceptanceService, BackupAdditionalInspectionAcceptanceService>();
builder.Services.AddScoped<IApproximateCostJuryService, ApproximateCostJuryService>();
builder.Services.AddScoped<IProcurementEntryEditorService, ProcurementEntryEditorService>();
builder.Services.AddScoped<IBackupProcurementEntryService, BackupProcurementEntryService>();
builder.Services.AddScoped<IBackupProcurementEntryEditorService, BackupProcurementEntryEditorService>();
builder.Services.AddScoped<ISuperAdminService, SuperAdminService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IInspectionAcceptanceNoteService, InspectionAcceptanceNoteService>();
builder.Services.AddScoped<IUserOwnFeaturesListService, UserOwnFeaturesListService>();
// Factoryler
builder.Services.AddSingleton<ITeminApiExceptionFactory, TeminApiExceptionFactory>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = cfg["Jwt:Issuer"],
        ValidAudience = cfg["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]))
    };
});

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register LogActionFilter for DI
builder.Services.AddScoped<LogActionFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<TeminApiExceptionFilter>();
}).AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new TurkeyDateTimeConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Doğrudan Temin API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Bearer şeması. “Bearer {token}” olarak ekleyin.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var wizard = new DbInstallationWizard(scope.ServiceProvider);
    await wizard.RunAsync();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowMyClient");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();
