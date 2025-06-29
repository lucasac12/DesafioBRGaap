using DesafioBRGaap.Services;
using DesafioBRGaap.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = "Data Source=Data/tarefas.db";

var dataDirectory = "Data";
if (!Directory.Exists(dataDirectory))
{
    Directory.CreateDirectory(dataDirectory);
    Console.WriteLine($"Pasta de dados criada: {dataDirectory}");
}

builder.Services.AddDbContext<TarefaContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddHttpClient<ITarefaLocalService, TarefaLocalService>();

builder.Services.AddScoped<ITarefaService, TarefaLocalService>();
builder.Services.AddScoped<ITarefaLocalService, TarefaLocalService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TarefaContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();