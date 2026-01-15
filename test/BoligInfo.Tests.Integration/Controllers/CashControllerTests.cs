using System.Net;
using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using BoligInfo.Tests.Integration.Setup;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BoligInfo.Tests.Integration.Controllers;

public class CashControllerTests(WebApplicationFactory<Program> factory) : IntegrationTestBase(factory)
{
    private async Task<long> CreateEquityAsync(string currency = "DKK")
    {
        var createDto = new CreateEquityDto { Currency = currency };
        var response = await Client.PostAsJsonAsync("/api/equities", createDto);
        var equity = await response.Content.ReadFromJsonAsync<EquityDto>();
        return equity!.Id;
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoCash()
    {
        await CleanDatabaseAsync();

        var response = await Client.GetAsync("/api/allcash");

        response.EnsureSuccessStatusCode();
        var cashRecords = await response.Content.ReadFromJsonAsync<IEnumerable<CashDto>>();
        Assert.NotNull(cashRecords);
        Assert.Empty(cashRecords);
    }

    [Fact]
    public async Task Create_ReturnsCreatedCash_WithValidData()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateCashDto
        {
            EquityId = equityId,
            CashAmount = 50000.0
        };

        var response = await Client.PostAsJsonAsync("/api/allcash", createDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var cash = await response.Content.ReadFromJsonAsync<CashDto>();
        Assert.NotNull(cash);
        Assert.True(cash.Id > 0);
        Assert.Equal(equityId, cash.EquityId);
        Assert.Equal(50000.0, cash.CashAmount);
    }

    [Fact]
    public async Task Create_ReturnsError_WhenEquityDoesNotExist()
    {
        var createDto = new CreateCashDto
        {
            EquityId = 99999,
            CashAmount = 10000.0
        };

        var response = await Client.PostAsJsonAsync("/api/allcash", createDto);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsCash_WhenExists()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateCashDto
        {
            EquityId = equityId,
            CashAmount = 75000.0
        };
        var createResponse = await Client.PostAsJsonAsync("/api/allcash", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<CashDto>();

        var response = await Client.GetAsync($"/api/allcash/{created!.Id}");

        response.EnsureSuccessStatusCode();
        var cash = await response.Content.ReadFromJsonAsync<CashDto>();
        Assert.NotNull(cash);
        Assert.Equal(created.Id, cash.Id);
        Assert.Equal(75000.0, cash.CashAmount);
        Assert.Equal(equityId, cash.EquityId);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await Client.GetAsync("/api/allcash/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByEquityId_ReturnsAllCashForEquity()
    {
        var equityId = await CreateEquityAsync();
        
        await Client.PostAsJsonAsync("/api/allcash", new CreateCashDto
        {
            EquityId = equityId,
            CashAmount = 25000.0
        });
        
        await Client.PostAsJsonAsync("/api/allcash", new CreateCashDto
        {
            EquityId = equityId,
            CashAmount = 35000.0
        });

        var response = await Client.GetAsync($"/api/allcash/equity/{equityId}");

        response.EnsureSuccessStatusCode();
        var cashRecords = await response.Content.ReadFromJsonAsync<IEnumerable<CashDto>>();
        Assert.NotNull(cashRecords);
        Assert.Equal(2, cashRecords.Count());
        Assert.All(cashRecords, c => Assert.Equal(equityId, c.EquityId));
    }

    [Fact]
    public async Task GetByEquityId_ReturnsEmptyList_WhenNoCashForEquity()
    {
        var equityId = await CreateEquityAsync();

        var response = await Client.GetAsync($"/api/allcash/equity/{equityId}");

        response.EnsureSuccessStatusCode();
        var cashRecords = await response.Content.ReadFromJsonAsync<IEnumerable<CashDto>>();
        Assert.NotNull(cashRecords);
        Assert.Empty(cashRecords);
    }

    [Fact]
    public async Task Update_ModifiesCash_WhenExists()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateCashDto
        {
            EquityId = equityId,
            CashAmount = 40000.0
        };
        var createResponse = await Client.PostAsJsonAsync("/api/allcash", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<CashDto>();
        
        var updateDto = new UpdateCashDto { CashAmount = 45000.0 };

        var response = await Client.PutAsJsonAsync($"/api/allcash/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<CashDto>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal(45000.0, updated.CashAmount);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenDoesNotExist()
    {
        var updateDto = new UpdateCashDto { CashAmount = 50000.0 };

        var response = await Client.PutAsJsonAsync("/api/allcash/99999", updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesCash_WhenExists()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateCashDto
        {
            EquityId = equityId,
            CashAmount = 30000.0
        };
        var createResponse = await Client.PostAsJsonAsync("/api/allcash", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<CashDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/allcash/{created!.Id}");
        var getResponse = await Client.GetAsync($"/api/allcash/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDoesNotExist()
    {
        var response = await Client.DeleteAsync("/api/allcash/99999");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}