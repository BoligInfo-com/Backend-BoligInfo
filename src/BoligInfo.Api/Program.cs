using Boliginfo.CashRepository;
using BoligInfo.CashService;
using BoligInfo.Core.Enums;
using BoligInfo.Database;
using BoligInfo.LoanRepository;
using BoligInfo.EquityRepository;
using BoligInfo.EquityService;
using BoligInfo.LoanService;

using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.NameTranslation;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<LoanType>("LoanType", new NpgsqlNullNameTranslator());
var dataSource = dataSourceBuilder.Build();

// Add services to the container
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();


// Add DbContext
builder.Services.AddDbContext<BoligInfoDbContext>(options =>
    options.UseNpgsql(dataSource, o => o.MapEnum<LoanType>("LoanType")));

// Register repositories & services for the scope of a request
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IEquityRepository, EquityRepository>();
builder.Services.AddScoped<ICashRepository, CashRepository>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IEquityService, EquityService>();
builder.Services.AddScoped<ICashService, CashService>();



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

public partial class Program;