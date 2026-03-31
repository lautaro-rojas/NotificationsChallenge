using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using NotificationsChallenge.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

#region OpenAPI + Scalar
// dotnet add package Microsoft.OpenApi
// dotnet add package Scalar.AspNetCore
builder.Services.AddOpenApi(options =>
{
    // 1. Registramos el esquema "Bearer" en el diccionario del documento global
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            BearerFormat = "JWT"
        };

        document.Components ??= new OpenApiComponents();
        document.AddComponent("Bearer", bearerScheme);

        return Task.CompletedTask;
    });

    // 2. Filtramos endpoint por endpoint (Operación por Operación)
    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        // Obtenemos los atributos que le pusiste al endpoint (ej: [Authorize], [AllowAnonymous])
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        
        var hasAuthorize = metadata.OfType<Microsoft.AspNetCore.Authorization.IAuthorizeData>().Any();
        var hasAllowAnonymous = metadata.OfType<Microsoft.AspNetCore.Authorization.IAllowAnonymous>().Any();

        // Si el endpoint tiene [Authorize] y NO tiene [AllowAnonymous]...
        if (hasAuthorize && !hasAllowAnonymous)
        {
            // ...entonces sí le agregamos el requisito de seguridad (el candado)
            var securityRequirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
            };

            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(securityRequirement);
        }

        return Task.CompletedTask;
    });
});
#endregion

#region JWT
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
#endregion

#region Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

// Services
builder.Services.AddScoped<NotificationsChallenge.Application.Services.UserService>();
builder.Services.AddScoped<NotificationsChallenge.Application.Services.AuthService>();
builder.Services.AddScoped<NotificationsChallenge.Application.Services.JwtService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.MapGet("/", () => Results.Redirect("/scalar/v1"))
       .ExcludeFromDescription();
}

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();