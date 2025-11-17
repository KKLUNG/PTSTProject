using Microsoft.EntityFrameworkCore;
using PTSDProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Add Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PTSDContext")));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVite", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];
var key = jwtSection["Key"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key ?? string.Empty))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 開發環境不使用 HTTPS 重定向
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseDefaultFiles(); // 支持 index.html

app.UseRouting();

app.UseCors("AllowVite");

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowVite");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.Run();
