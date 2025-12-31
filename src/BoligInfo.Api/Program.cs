using BoligInfo.Database;
using BoligInfo.LoanRepository;
using BoligInfo.EquityRepository;
using BoligInfo.Services;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

// Add DbContext
builder.Services.AddDbContext<BoligInfoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IEquityRepository, EquityRepository>();

// Register services
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IEquityService, EquityService>();

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