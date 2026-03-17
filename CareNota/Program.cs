using System.Text;
using CareNota.Data;
using CareNota.Models;
using CareNota.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var Builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
Builder.Services.AddDbContext<ApplicationDbContext>(Options =>
    Options.UseSqlServer(Builder.Configuration.GetConnectionString("DefaultConnection")));

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

var App = Builder.Build();

// ── Role Seeding ──────────────────────────────────────────────────────────────
using (var Scope = App.Services.CreateScope())
{
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