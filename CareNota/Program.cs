using CareNota.Data;
using CareNota.Mappings;
using CareNota.Models;
using CareNota.Repositories;
using CareNota.Repositories.Interfaces;
using CareNota.Services;
using CareNota.Services.Interfaces;
using CareNota.Validators.Appointment;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;

using Azure.Storage.Blobs;                                             
      


var Builder = WebApplication.CreateBuilder(args);

// ── Database ────────────────────────────────────────────────────────────────
Builder.Services.AddDbContext<ApplicationDbContext>(Options =>
    Options.UseSqlServer(
        Builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

// ── Identity ────────────────────────────────────────────────────────────────
Builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Options =>
{
    Options.Password.RequireDigit = true;
    Options.Password.RequiredLength = 8;
    Options.Password.RequireUppercase = true;
    Options.Password.RequireNonAlphanumeric = false;
    Options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();




////   AZURE BLOB
//Builder.Services.AddSingleton(
//   new BlobServiceClient(
//       Builder.Configuration["AzureBlob:ConnectionString"]));

////  HTTPCLIENT → PYTHON FASTAPI                                   
//Builder.Services.AddHttpClient("PythonFastAPI", Client =>
//{
//    Client.BaseAddress = new Uri(Builder.Configuration["PythonAI:BaseUrl"]!);
//    Client.Timeout = TimeSpan.FromMinutes(5);
//});


// ── Controllers + Swagger ───────────────────────────────────────────────────
//Builder.Services.AddControllers();

Builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
Builder.Services.AddEndpointsApiExplorer();
Builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CareNota API", Version = "v1" });

options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Enter your JWT token below."
});

options.AddSecurityRequirement(document => new()
{
    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
});
});

// ── Repositories ────────────────────────────────────────────────────────────
Builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
Builder.Services.AddScoped<IPatientRepository, PatientRepository>();
Builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
Builder.Services.AddScoped<IVisitRepository, VisitRepository>();
Builder.Services.AddScoped<IDiagnosisRepository, DiagnosisRepository>();
Builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
Builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
Builder.Services.AddScoped<ILabTestRepository, LabTestRepository>();
// DI registrations
Builder.Services.AddScoped<IAdminRepository, AdminRepository>();


//Builder.Services.AddScoped<IAudioRepository, AudioRepository>();

// ── Services ────────────────────────────────────────────────────────────────

Builder.Services.AddScoped<IAuthService, AuthService>();
Builder.Services.AddScoped<IPatientService, PatientService>();
Builder.Services.AddScoped<IDoctorService, DoctorService>();
Builder.Services.AddScoped<IVisitService, VisitService>();
Builder.Services.AddScoped<IDiagnosisService, DiagnosisService>();
Builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
Builder.Services.AddScoped<IMedicationService, MedicationService>();
Builder.Services.AddScoped<ILabTestService, LabTestService>();
Builder.Services.AddScoped<IAppointmentService, AppointmentService>();
//Builder.Services.AddScoped<IAudioService, AudioService>();
Builder.Services.AddScoped<IAdminService, AdminService>();

Builder.Services.AddAutoMapper(typeof(MappingProfile));

// ── FluentValidation ─────────────────────────────────────────────────────────
Builder.Services.AddFluentValidationAutoValidation();
Builder.Services.AddValidatorsFromAssemblyContaining<CreateAppointmentValidator>();

// ── File Upload Config ──────────────────────────────────────────────────────
Builder.Services.Configure<FormOptions>(Options =>
{
    Options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});

Builder.Services.AddAutoMapper(typeof(Program));


// CORS

Builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
// ── JWT Authentication ───────────────────────────────────────────────────────
Builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// ── Build App ───────────────────────────────────────────────────────────────
var App = Builder.Build();
// admin 
// ====================== DATA SEEDING ======================
// Program.cs — seeding block
using (var scope = App.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var context = services.GetRequiredService<ApplicationDbContext>(); 
        var logger = services.GetRequiredService<ILogger<Program>>();

        await DataSeeder.SeedAsync(userManager, roleManager, context, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ An error occurred while seeding the database.");
    }
}
//// Seed Roles + Admin
//using (var scope = App.Services.CreateScope())
//{
//    // Run migrations
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    db.Database.Migrate();

//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//    // Create roles
//    string[] roles = { "admin", "doctor", "patient", "receptionist" };
//    foreach (var role in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//            await roleManager.CreateAsync(new IdentityRole(role));
//    }



///

//        var result = await userManager.CreateAsync(admin, "Admin@123456");
//        if (result.Succeeded)
//            await userManager.AddToRoleAsync(admin, "admin");
//    }
//}
// ── Middleware Pipeline ─────────────────────────────────────────────────────
if (App.Environment.IsDevelopment())
{
    App.UseSwagger();
    App.UseSwaggerUI();
}

App.UseCors("AllowAll");

App.UseHttpsRedirection();

App.MapControllers();

App.Run();




