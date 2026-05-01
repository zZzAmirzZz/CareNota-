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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Text.Json;
using System.Text.Json.Serialization;

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

// ❌ حذفنا JWT Authentication + Authorization بالكامل

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
// ── Build App ───────────────────────────────────────────────────────────────
var App = Builder.Build();

// ── Role Seeding + Migrations ───────────────────────────────────────────────
using (var Scope = App.Services.CreateScope())
{
    var Db = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Db.Database.Migrate();

    var RoleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(RoleManager);
}




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




