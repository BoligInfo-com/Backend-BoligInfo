using System.Net;
using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using BoligInfo.Tests.Integration.Setup;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BoligInfo.Tests.Integration.Controllers;

public class CashFlowControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
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

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoCashFlows()
    {
        await CleanDatabaseAsync();

        var response = await Client.GetAsync("/api/cashflows");

        response.EnsureSuccessStatusCode();
        var cashFlows = await response.Content.ReadFromJsonAsync<IEnumerable<CashFlowDto>>();
        Assert.NotNull(cashFlows);
        Assert.Empty(cashFlows);
    }

    [Fact]
    public async Task Create_ReturnsCreatedCashFlow_WithValidData()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 12000.0,
            Name = "Property Tax",
            Description = "Annual property tax payment"
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var cashFlow = await response.Content.ReadFromJsonAsync<CashFlowDto>();
        Assert.NotNull(cashFlow);
        Assert.True(cashFlow.Id > 0);
        Assert.Equal(houseId, cashFlow.HouseId);
        Assert.Equal("EXPENSE", cashFlow.Type);
        Assert.Equal("MONTHLY", cashFlow.Frequency);
        Assert.Equal(12000.0, cashFlow.Amount);
        Assert.Equal("Property Tax", cashFlow.Name);
        Assert.Equal("Annual property tax payment", cashFlow.Description);
    }

    [Theory]
    [InlineData("INCOME")]
    [InlineData("EXPENSE")]
    public async Task Create_AcceptsAllCashFlowTypes(string type)
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = type,
            Frequency = "MONTHLY",
            Amount = 5000.0,
            Name = "Test Cash Flow"
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        response.EnsureSuccessStatusCode();
        var cashFlow = await response.Content.ReadFromJsonAsync<CashFlowDto>();
        Assert.NotNull(cashFlow);
        Assert.Equal(type, cashFlow.Type);
    }

    [Theory]
    [InlineData("DAILY")]
    [InlineData("WEEKLY")]
    [InlineData("BI_WEEKLY")]
    [InlineData("MONTHLY")]
    [InlineData("QUARTERLY")]
    [InlineData("YEARLY")]
    [InlineData("ONE_TIME")]
    public async Task Create_AcceptsAllFrequencies(string frequency)
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = frequency,
            Amount = 1000.0,
            Name = "Test Payment"
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        response.EnsureSuccessStatusCode();
        var cashFlow = await response.Content.ReadFromJsonAsync<CashFlowDto>();
        Assert.NotNull(cashFlow);
        Assert.Equal(frequency, cashFlow.Frequency);
    }

    [Fact]
    public async Task Create_AllowsNullDescription()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "INCOME",
            Frequency = "MONTHLY",
            Amount = 8000.0,
            Name = "Rental Income",
            Description = null
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        response.EnsureSuccessStatusCode();
        var cashFlow = await response.Content.ReadFromJsonAsync<CashFlowDto>();
        Assert.NotNull(cashFlow);
        Assert.Null(cashFlow.Description);
    }

    [Fact]
    public async Task Create_ReturnsError_WhenHouseDoesNotExist()
    {
        var createDto = new CreateCashFlowDto
        {
            HouseId = 99999,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 5000.0,
            Name = "Test"
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_RejectsNegativeAmount()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = -1000.0,
            Name = "Invalid"
        };

        var response = await Client.PostAsJsonAsync("/api/cashflows", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsCashFlow_WhenExists()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "QUARTERLY",
            Amount = 3000.0,
            Name = "Insurance"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/cashflows", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<CashFlowDto>();

        var response = await Client.GetAsync($"/api/cashflows/{created!.Id}");

        response.EnsureSuccessStatusCode();
        var cashFlow = await response.Content.ReadFromJsonAsync<CashFlowDto>();
        Assert.NotNull(cashFlow);
        Assert.Equal(created.Id, cashFlow.Id);
        Assert.Equal("EXPENSE", cashFlow.Type);
        Assert.Equal("QUARTERLY", cashFlow.Frequency);
        Assert.Equal(3000.0, cashFlow.Amount);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await Client.GetAsync("/api/cashflows/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByHouseId_ReturnsAllCashFlowsForHouse()
    {
        var houseId = await CreateHouseAsync();
        
        await Client.PostAsJsonAsync("/api/cashflows", new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 10000.0,
            Name = "Mortgage"
        });
        
        await Client.PostAsJsonAsync("/api/cashflows", new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "INCOME",
            Frequency = "MONTHLY",
            Amount = 15000.0,
            Name = "Rent"
        });

        var response = await Client.GetAsync($"/api/cashflows/house/{houseId}");

        response.EnsureSuccessStatusCode();
        var cashFlows = await response.Content.ReadFromJsonAsync<IEnumerable<CashFlowDto>>();
        Assert.NotNull(cashFlows);
        Assert.Equal(2, cashFlows.Count());
        Assert.All(cashFlows, cf => Assert.Equal(houseId, cf.HouseId));
    }

    [Fact]
    public async Task GetByHouseId_ReturnsEmptyList_WhenNoCashFlowsForHouse()
    {
        var houseId = await CreateHouseAsync();

        var response = await Client.GetAsync($"/api/cashflows/house/{houseId}");

        response.EnsureSuccessStatusCode();
        var cashFlows = await response.Content.ReadFromJsonAsync<IEnumerable<CashFlowDto>>();
        Assert.NotNull(cashFlows);
        Assert.Empty(cashFlows);
    }

    [Fact]
    public async Task Update_ModifiesCashFlow_WhenExists()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 5000.0,
            Name = "Utilities"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/cashflows", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<CashFlowDto>();
        
        var updateDto = new UpdateCashFlowDto
        {
            Type = "EXPENSE",
            Frequency = "BI_WEEKLY",
            Amount = 5500.0,
            Name = "Updated Utilities"
        };

        var response = await Client.PutAsJsonAsync($"/api/cashflows/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<CashFlowDto>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("EXPENSE", updated.Type);
        Assert.Equal("BI_WEEKLY", updated.Frequency);
        Assert.Equal(5500.0, updated.Amount);
        Assert.Equal("Updated Utilities", updated.Name);
    }

    [Fact]
    public async Task Update_PartialUpdate_OnlyModifiesProvidedFields()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "INCOME",
            Frequency = "MONTHLY",
            Amount = 12000.0,
            Name = "Rental Income"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/cashflows", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<CashFlowDto>();
        
        var updateDto = new UpdateCashFlowDto { Amount = 13000.0 };

        var response = await Client.PutAsJsonAsync($"/api/cashflows/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<CashFlowDto>();
        Assert.NotNull(updated);
        Assert.Equal(13000.0, updated.Amount);
        Assert.Equal("INCOME", updated.Type);
        Assert.Equal("MONTHLY", updated.Frequency);
        Assert.Equal("Rental Income", updated.Name);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenDoesNotExist()
    {
        var updateDto = new UpdateCashFlowDto { Amount = 5000.0 };

        var response = await Client.PutAsJsonAsync("/api/cashflows/99999", updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesCashFlow_WhenExists()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateCashFlowDto
        {
            HouseId = houseId,
            Type = "EXPENSE",
            Frequency = "YEARLY",
            Amount = 2000.0,
            Name = "Maintenance"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/cashflows", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<CashFlowDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/cashflows/{created!.Id}");
        var getResponse = await Client.GetAsync($"/api/cashflows/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await Client.DeleteAsync("/api/cashflows/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}