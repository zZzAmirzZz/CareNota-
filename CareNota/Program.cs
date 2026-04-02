
using CareNota.Data;
using CareNota.Models;
using CareNota.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var Builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
Builder.Services.AddDbContext<ApplicationDbContext>(Options =>
    Options.UseSqlServer(Builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions => sqlOptions.EnableRetryOnFailure(
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

Builder.Services.AddAuthorization();

// ── Services ──────────────────────────────────────────────────────────────────
Builder.Services.AddScoped<IAuthService, AuthService>();
Builder.Services.AddControllers();
Builder.Services.AddEndpointsApiExplorer();
Builder.Services.AddSwaggerGen();
Builder.Services.AddScoped<IRepository<Patient>, GenericRepository<Patient>>();
Builder.Services.AddScoped<IRepository<Doctor>, GenericRepository<Doctor>>();
Builder.Services.AddScoped<IRepository<Appointment>, GenericRepository<Appointment>>();
Builder.Services.AddScoped<IPatientService, PatientService>();
Builder.Services.AddScoped<IPatientRepository, PatientRepository>();
Builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
Builder.Services.AddScoped<IDoctorService, DoctorService>();
Builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
Builder.Services.AddScoped<IAppointmentService, AppointmentService>();
Builder.Services.AddAutoMapper(typeof(Program));

var App = Builder.Build();

// ── Role Seeding ──────────────────────────────────────────────────────────────
using (var Scope = App.Services.CreateScope())
{
    // Step 1 — make sure DB exists and all migrations are applied
    var Db = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Db.Database.Migrate(); // ← THIS is what was missing

    // Step 2 — then seed roles
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
App.UseAuthentication(); // ← must be before Authorization
App.UseAuthorization();
App.MapControllers();
App.Run();