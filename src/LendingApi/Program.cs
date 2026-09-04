using LendingApi.Data;
using LendingApi.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Config-driven provider switch:
// - Connection string "LendingDb" present  -> Azure SQL / SQL Server
// - Absent                                 -> EF Core InMemory (zero-setup local run)
var connectionString = builder.Configuration.GetConnectionString("LendingDb");

builder.Services.AddDbContext<LendingContext>(options =>
{
    if (string.IsNullOrWhiteSpace(connectionString))
        options.UseInMemoryDatabase("LendingDb");
    else
        options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<LoanValidationService>();

var app = builder.Build();

// Seed demo data (InMemory always; SQL only when empty)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LendingContext>();
    if (!string.IsNullOrWhiteSpace(connectionString))
        db.Database.EnsureCreated();
    DataSeeder.Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
