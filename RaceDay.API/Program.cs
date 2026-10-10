using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Token Service
builder.Services.AddScoped<TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
// Seed Roles
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new RaceDayAPI.Models.Role { RoleName = "Admin", Description = "System Administrator" },
            new RaceDayAPI.Models.Role { RoleName = "Organiser", Description = "Event Organiser" },
            new RaceDayAPI.Models.Role { RoleName = "Participant", Description = "Regular Participant" }
        );
        context.SaveChanges();
    }
}