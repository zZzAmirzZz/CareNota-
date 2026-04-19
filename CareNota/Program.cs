using CareNota.Data;
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
//using Microsoft.OpenApi.Models;
using System.Text;

var Builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
Builder.Services.AddDbContext<ApplicationDbContext>(Options =>
    Options.UseSqlServer(
        Builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

// ── Identity ──────────────────────────────────────────────────────────────────
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

// ── JWT Authentication ────────────────────────────────────────────────────────
var JwtSettings = Builder.Configuration.GetSection("Jwt");
var Key = Encoding.UTF8.GetBytes(JwtSettings["Key"]!);

Builder.Services.AddAuthentication(Options =>
{
    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(Options =>
{
    Options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtSettings["Issuer"],
        ValidAudience = JwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Key),
        ClockSkew = TimeSpan.Zero
    };
});


//Builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CareNota API", Version = "v1" });

//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Enter: Bearer {your token}"
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});
// ── Controllers + Swagger ─────────────────────────────────────────────────────
Builder.Services.AddControllers();
Builder.Services.AddEndpointsApiExplorer();

// ── Repositories ──────────────────────────────────────────────────────────────
Builder.Services.AddScoped<IVisitRepository, VisitRepository>();
Builder.Services.AddScoped<IDiagnosisRepository, DiagnosisRepository>();
Builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
Builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
Builder.Services.AddScoped<ILabTestRepository, LabTestRepository>();
Builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// ── Services ──────────────────────────────────────────────────────────────────
Builder.Services.AddScoped<IAuthService, AuthService>();
Builder.Services.AddScoped<IVisitService, VisitService>();
Builder.Services.AddScoped<IDiagnosisService, DiagnosisService>();
Builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
Builder.Services.AddScoped<IMedicationService, MedicationService>();
Builder.Services.AddScoped<ILabTestService, LabTestService>();

// ── FluentValidation ───────────────────────────────────────────────────────────
Builder.Services.AddFluentValidationAutoValidation();
Builder.Services.AddValidatorsFromAssemblyContaining<CreateAppointmentValidator>();
// ── File Upload Config ────────────────────────────────────────────────────────
Builder.Services.Configure<FormOptions>(Options =>
{
    Options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});
Builder.Services.AddAutoMapper(typeof(Program));
// ── Build App ─────────────────────────────────────────────────────────────────
var App = Builder.Build();

// ── Role Seeding + Migrations ─────────────────────────────────────────────────
using (var Scope = App.Services.CreateScope())
{
    var Db = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Db.Database.Migrate();

    var RoleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(RoleManager);
}

// ── Middleware Pipeline ───────────────────────────────────────────────────────
if (App.Environment.IsDevelopment())
{
    App.UseSwagger();
    App.UseSwaggerUI();
}

App.UseHttpsRedirection();

//App.UseAuthentication(); // must come before Authorization
//App.UseAuthorization();


App.MapControllers();

App.Run();