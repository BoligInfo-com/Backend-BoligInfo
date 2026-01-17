using System.Reflection;
using BoligInfo.CashFlowRepository;
using BoligInfo.CashFlowService;
using Boliginfo.CashRepository;
using BoligInfo.CashService;
using BoligInfo.Core.Enums;
using BoligInfo.Database;
using BoligInfo.LoanRepository;
using BoligInfo.EquityRepository;
using BoligInfo.EquityService;
using BoligInfo.HouseRepository;
using BoligInfo.HouseService;
using BoligInfo.LoanService;

using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.NameTranslation;


var builder = WebApplication.CreateBuilder(args);


// configure PostgresSQL if not in test environment
if (!builder.Environment.EnvironmentName.Equals("Test", StringComparison.OrdinalIgnoreCase))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    
    // Map C# Enums to PostgresSQL Enum types
    dataSourceBuilder.MapEnum<LoanType>("LoanType", new NpgsqlNullNameTranslator());
    dataSourceBuilder.MapEnum<EnergyLabel>("EnergyLabel", new NpgsqlNullNameTranslator());
    dataSourceBuilder.MapEnum<Frequency>("Frequency", new NpgsqlNullNameTranslator());
    dataSourceBuilder.MapEnum<CashFlowType>("CashFlowType", new NpgsqlNullNameTranslator());
    
    var dataSource = dataSourceBuilder.Build();
    
    // Add DbContext
    builder.Services.AddDbContext<BoligInfoDbContext>(options =>
        options.UseNpgsql(dataSource, o =>
        {
            o.MapEnum<LoanType>("LoanType");
            o.MapEnum<EnergyLabel>("EnergyLabel");
            o.MapEnum<Frequency>("Frequency");
            o.MapEnum<CashFlowType>("CashFlowType");
        }));
}
    


// Add services to the container
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddAuthorization();
builder.Services.AddLogging();


// Register repositories for the scope of a request
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IEquityRepository, EquityRepository>();
builder.Services.AddScoped<ICashRepository, CashRepository>();
builder.Services.AddScoped<IHouseRepository, HouseRepository>();
builder.Services.AddScoped<ICashFlowRepository, CashFlowRepository>();

// Register services for the scope of a request
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IEquityService, EquityService>();
builder.Services.AddScoped<ICashService, CashService>();
builder.Services.AddScoped<IHouseService, HouseService>();
builder.Services.AddScoped<ICashFlowService, CashFlowService>();


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

public partial class Program { }