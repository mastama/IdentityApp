using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Tambahkan layanan Swagger
builder.Services.AddEndpointsApiExplorer(); // Untuk eksplorasi API endpoints
builder.Services.AddSwaggerGen(); // Untuk menghasilkan dokumentasi Swagger

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

// untuk redirect ke https
app.UseHttpsRedirection();

// Endpoint Hello World untuk pengecekan
app.MapGet("/hello", () => Results.Ok("Hello, World!"))
    .WithName("GetHelloWorld");

// Endpoint WeatherForecast
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}