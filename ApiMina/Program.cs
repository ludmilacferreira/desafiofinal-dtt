using Microsoft.EntityFrameworkCore;
using DesafioFinal.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Registra os serviços
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    options
        .UseNpgsql(cs)
        .UseSnakeCaseNamingConvention(); 
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI(); 
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();