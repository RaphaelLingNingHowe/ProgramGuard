using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProgramGuard.Config;
using ProgramGuard.Data;
using ProgramGuard.Identity;
using ProgramGuard.Interfaces.Repository;
using ProgramGuard.Interfaces.Service;
using ProgramGuard.Models;
using ProgramGuard.Repositories;
using ProgramGuard.Repository;
using ProgramGuard.Services;
using System.Text;

//WebApplicationOptions webApplicationOptions = new()
//{
//    ContentRootPath = AppContext.BaseDirectory,
//    Args = args,
//    ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName
//};

//WebApplicationBuilder builder = WebApplication.CreateBuilder(webApplicationOptions);
//builder.Host.UseWindowsService();

///* set log */
//builder.Services.AddLogging(loggingBuilder =>
//{
//    loggingBuilder.ClearProviders();
//    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
//    loggingBuilder.AddNLog();
//});
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContextFactory<ApplicationDBContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IFileDetectionService, FileDetectionService>();
builder.Services.AddScoped<IChangeLogRepository, ChangeLogRepository>();
builder.Services.AddScoped<IFileListRepository, FileListRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPathProcessService, PathProcessService>();
builder.Services.AddScoped<UserManager<AppUser>, CustomUserManager<AppUser>>();
builder.Services.AddScoped<CustomUserManager<AppUser>>();
builder.Services.AddScoped<SignInManager<AppUser>, CustomSignInManager<AppUser>>();

builder.Services.AddHostedService<Worker>();
var lockoutTimeSpan = builder.Configuration.GetSection("IdentityOptions:LockoutTimeSpan").Get<int>();
var signingKey = builder.Configuration["JWT:SigningKey"];
if (string.IsNullOrEmpty(signingKey))
{
    throw new InvalidOperationException("JWT:SigningKey is not configured or is empty.");
}
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutTimeSpan);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)

        )
    };
});
builder.Services.AddAuthorization();

// Initialize constants
AppSettings.Initialize(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
