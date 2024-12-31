using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ServerApp.Data;
using ServerApp.Models;
using ServerApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Tambahkan layanan Swagger
builder.Services.AddEndpointsApiExplorer(); // Untuk eksplorasi API endpoints
builder.Services.AddSwaggerGen(); // Untuk menghasilkan dokumentasi Swagger

// Konfigurasi db
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ??
                      throw new InvalidOperationException("Could not find a your connection string!"));
});

// Jwt
builder.Services.AddScoped<JwtService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // validasi key
            ValidateIssuerSigningKey = true,
            // menentukan key untuk memvalidasi sign token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
            // validasi token di keluarkan oleh server yang valid
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            // validasi token digunakan audience yang valid
            ValidAudience = builder.Configuration["JWT:Audience"],
            // validasi token di keluarkan oleh server yang valid
            ValidateIssuer = true,
            // validasi token digunakan audience yang valid
            ValidateAudience = false,
        };
    });

// Defining our IdentityCore Service
builder.Services.AddIdentityCore<User>(options =>
{
    // password configuration
    options.Password.RequiredLength = 8; // min length 8
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    
    // for email confirmation
    options.User.RequireUniqueEmail = true; // Pastikan email unik
    options.SignIn.RequireConfirmedEmail = true; // Wajib konfirmasi email setelah register
    
    // for Lockout user
    options.Lockout.MaxFailedAccessAttempts = 3; // Pengguna terkunci setelah 3 kali gagal login
})
.AddRoles<IdentityRole>() // Menambahkan manajemen roles (admin, user)
.AddRoleManager<RoleManager<IdentityRole>>() // Menambahkan RoleManager
.AddEntityFrameworkStores<ApplicationDbContext>() // Menyimpan data di EF Core
.AddSignInManager<SignInManager<User>>() // Menambahkan SigInManager (login & logout)
.AddUserManager<UserManager<User>>() // Menambahkan UserManager untuk membuat user
.AddDefaultTokenProviders(); // Menambahkan token default, konfirmasi email

// Konfigurasi Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Kirim log ke konsol
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Kirim log ke file
    .CreateLogger();

var app = builder.Build();

// Konfigurasi Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Menghasilkan swagger.json
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // Menyediakan UI Swagger di /swagger
        options.RoutePrefix = string.Empty; // Agar Swagger UI tersedia di root (http://localhost:5000)
    });
}



// Endpoint Hello World untuk pengecekan
app.MapGet("/hello", () => Results.Ok("Hello, World!"))
    .WithName("GetHelloWorld");

// untuk redirect ke https
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();