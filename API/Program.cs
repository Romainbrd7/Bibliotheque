using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 🔌 Configuration EF Core + SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BibliothequeContext>(options =>
    options.UseSqlite(connectionString));

// 🧠 Injection du Repository
builder.Services.AddScoped<IMediaRepository, MediaRepository>();

// 🚀 Ajout des contrôleurs et Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔄 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // 👈 Affiche les erreurs détaillées
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
