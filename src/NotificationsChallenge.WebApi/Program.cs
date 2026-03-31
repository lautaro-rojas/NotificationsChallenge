using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using NotificationsChallenge.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
builder.Services.AddAuthorization();
var secretKey = builder.Configuration["JwtSecretKey"]!;
if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
{
    Console.ForegroundColor = ConsoleColor.Red;
    
    Console.WriteLine("\n=======================================================================");
    Console.WriteLine(" FATAL ERROR: The server cannot start.");
    Console.WriteLine(" 'JwtSecretKey' is missing in environment variables or is invalid.");
    Console.WriteLine("=======================================================================\n");
    
    Console.ResetColor();

    // Turn off the application forcefully. The '1' tells Docker/Coolify that the app failed and should not be deployed.
    Environment.Exit(1); 
}
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
    {
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
    });

// BD
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<NotificationsChallenge.Application.Services.UserService>();
builder.Services.AddScoped<NotificationsChallenge.Application.Services.AuthService>();
builder.Services.AddScoped<NotificationsChallenge.Application.Services.JwtService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Configure swagger to shwow the ui in the root of the application
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationsChallenge v1");
        options.RoutePrefix = string.Empty;
    });
}

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();