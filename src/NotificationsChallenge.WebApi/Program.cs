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
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
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