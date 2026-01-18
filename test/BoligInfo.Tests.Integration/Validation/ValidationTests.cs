using System.Net;
using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using BoligInfo.Tests.Integration.Setup;

namespace BoligInfo.Tests.Integration.Validation;

public class ValidationTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private async Task<long> CreateHouseAsync()
    {
        var equityDto = new CreateEquityDto { Currency = "DKK" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", equityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var houseDto = new CreateHouseDto
        {
            EquityId = equity!.Id,
            Price = 2000000.0
        };
        var houseResponse = await Client.PostAsJsonAsync("/api/houses", houseDto);
        var house = await houseResponse.Content.ReadFromJsonAsync<HouseDto>();
        return house!.Id;
    }
    
    private async Task<long> CreateEquityAsync()
    {
        var createDto = new CreateEquityDto { Currency = "DKK" };
        var response = await Client.PostAsJsonAsync("/api/equities", createDto);
        var equity = await response.Content.ReadFromJsonAsync<EquityDto>();
        return equity!.Id;
    }

    private async Task<long> CreateHouseAsync(long equityId)
    {
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2000000.0
        };
        var response = await Client.PostAsJsonAsync("/api/houses", createDto);
        var house = await response.Content.ReadFromJsonAsync<HouseDto>();
        return house!.Id;
    }
    
    [Fact]
    public async Task Address_RejectsCountryLongerThan180Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            Country = new string('A', 181),
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Address_AcceptsCountryUpTo180Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            Country = new string('A', 180),
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task Address_RejectsCityLongerThan340Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = new string('B', 341),
            Zipcode = "2100",
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Address_AcceptsCityUpTo340Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = new string('B', 340),
            Zipcode = "2100",
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task Address_RejectsZipcodeLongerThan80Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = new string('C', 81),
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Address_AcceptsZipcodeUpTo80Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = new string('C', 80),
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task Address_RejectsStreetLongerThan340Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = new string('D', 341)
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Address_AcceptsStreetUpTo340Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = new string('D', 340)
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task Address_RejectsNumberLongerThan20Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Main Street",
            Number = new string('E', 21)
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Address_AcceptsNumberUpTo20Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Main Street",
            Number = new string('E', 20)
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task Address_RejectsSuiteLongerThan20Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Main Street",
            Suite = new string('F', 21)
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Address_AcceptsSuiteUpTo20Characters()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Main Street",
            Suite = new string('F', 20)
        };
        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(int.MaxValue)]
    public async Task Address_AcceptsValidFloorNumbers(int floor)
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Main Street",
            Floor = floor
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task Address_RequiresCityField()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = null!,
            Zipcode = "2100",
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Address_RequiresZipcodeField()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = null!,
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Address_RequiresStreetField()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = null!
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    
    [Theory]
    [InlineData(-1000000.0)]
    [InlineData(-0.01)]
    public async Task House_RejectsNegativePrice(double price)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = price
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Theory]
    [InlineData(0.0)]
    [InlineData(1000000.0)]
    [InlineData(999999999.99)]
    public async Task House_AcceptsValidPrices(double price)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = price
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task House_RejectsNegativeNumberOfRooms(int rooms)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2000000.0,
            NumberOfRooms = rooms
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task House_AcceptsValidNumberOfRooms(int rooms)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2000000.0,
            NumberOfRooms = rooms
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(-500)]
    public async Task House_RejectsNegativeSquareMeters(int sqm)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2000000.0,
            SquareMeters = sqm
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(500)]
    public async Task House_AcceptsValidSquareMeters(int sqm)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2000000.0,
            SquareMeters = sqm
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData(-1000.0)]
    [InlineData(-0.01)]
    public async Task CashFlow_RejectsNegativeAmount(double amount)
    {
        var equityId = await CreateEquityAsync();
        var houseId = await CreateHouseAsync(equityId);
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = amount,
            Name = "Test"
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Theory]
    [InlineData(-1000.0)]
    [InlineData(-0.01)]
    public async Task Loan_RejectsNegativeLoanAmount(double amount)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanAmount = amount,
            InterestRate = 3.0,
            LoanLifetime = 15
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Theory]
    [InlineData(0.0)]
    [InlineData(1000.0)]
    [InlineData(999999999.99)]
    public async Task CashFlow_AcceptsValidAmounts(double amount)
    {
        var equityId = await CreateEquityAsync();
        var houseId = await CreateHouseAsync(equityId);
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = amount,
            Name = "Test"
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task CashFlow_RejectsNameLongerThan60Characters()
    {
        var equityId = await CreateEquityAsync();
        var houseId = await CreateHouseAsync(equityId);
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 5000.0,
            Name = new string('A', 61)
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CashFlow_AcceptsNameUpTo60Characters()
    {
        var equityId = await CreateEquityAsync();
        var houseId = await CreateHouseAsync(equityId);
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 5000.0,
            Name = new string('A', 60)
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CashFlow_RejectsDescriptionLongerThan1400Characters()
    {
        var equityId = await CreateEquityAsync();
        var houseId = await CreateHouseAsync(equityId);
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 5000.0,
            Name = "Test",
            Description = new string('B', 1401)
        };
        
        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Loan_RejectsInvalidLoanLifetime(int lifetime)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanAmount = 100000.0,
            InterestRate = 3.0,
            LoanLifetime = lifetime
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CashFlow_AcceptsDescriptionUpTo1400Characters()
    {
        var equityId = await CreateEquityAsync();
        var houseId = await CreateHouseAsync(equityId);
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 5000.0,
            Name = "Test",
            Description = new string('B', 1400)
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData(0.0)]
    [InlineData(100000.0)]
    [InlineData(999999999.99)]
    public async Task Loan_AcceptsValidLoanAmounts(double amount)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanAmount = amount,
            InterestRate = 3.0,
            LoanLifetime = 15
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(30)]
    [InlineData(50)]
    public async Task Loan_AcceptsValidLoanLifetimes(int lifetime)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanAmount = 100000.0,
            InterestRate = 3.0,
            LoanLifetime = lifetime
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Equity_AcceptsNullCurrency()
    {
        var createDto = new CreateEquityDto { Currency = null };

        var response = await Client.PostAsJsonAsync("/api/equities", createDto);

        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("DKK")]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("SEK")]
    [InlineData("NOK")]
    public async Task Equity_AcceptsVariousCurrencies(string currency)
    {
        var createDto = new CreateEquityDto { Currency = currency };

        var response = await Client.PostAsJsonAsync("/api/equities", createDto);

        response.EnsureSuccessStatusCode();
        var equity = await response.Content.ReadFromJsonAsync<EquityDto>();
        Assert.Equal(currency, equity!.Currency);
    }

    [Fact]
    public async Task Cash_RequiresNonNegativeAmount()
    {
        var equityId = await CreateEquityAsync();
        
        var validCash = new CreateCashDto
        {
            EquityId = equityId,
            CashAmount = 0.0
        };
        var validResponse = await Client.PostAsJsonAsync("/api/allcash", validCash);
        validResponse.EnsureSuccessStatusCode();
    }
}