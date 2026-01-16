using System.Net;
using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using BoligInfo.Tests.Integration.Setup;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BoligInfo.Tests.Integration.Controllers;

public class EquityControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoEquities()
    {
        await CleanDatabaseAsync();

        var response = await Client.GetAsync("/api/equities");

        response.EnsureSuccessStatusCode();
        var equities = await response.Content.ReadFromJsonAsync<IEnumerable<EquityDto>>();
        Assert.NotNull(equities);
        Assert.Empty(equities);
    }

    [Fact]
    public async Task Create_ReturnsCreatedEquity_WithValidData()
    {
        var createDto = new CreateEquityDto { Currency = "USD" };

        var response = await Client.PostAsJsonAsync("/api/equities", createDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var equity = await response.Content.ReadFromJsonAsync<EquityDto>();
        Assert.NotNull(equity);
        Assert.True(equity.Id > 0);
        Assert.Equal("USD", equity.Currency);
    }

    [Fact]
    public async Task Create_UsesDefaultCurrency_WhenNotProvided()
    {
        var createDto = new CreateEquityDto();

        var response = await Client.PostAsJsonAsync("/api/equities", createDto);

        response.EnsureSuccessStatusCode();
        var equity = await response.Content.ReadFromJsonAsync<EquityDto>();
        Assert.NotNull(equity);
        Assert.Equal("DKK", equity.Currency);
    }

    [Fact]
    public async Task GetById_ReturnsEquity_WhenExists()
    {
        var createDto = new CreateEquityDto { Currency = "EUR" };
        var createResponse = await Client.PostAsJsonAsync("/api/equities", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<EquityDto>();

        var response = await Client.GetAsync($"/api/equities/{created!.Id}");

        response.EnsureSuccessStatusCode();
        var equity = await response.Content.ReadFromJsonAsync<EquityDto>();
        Assert.NotNull(equity);
        Assert.Equal(created.Id, equity.Id);
        Assert.Equal("EUR", equity.Currency);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await Client.GetAsync("/api/equities/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ModifiesEquity_WhenExists()
    {
        var createDto = new CreateEquityDto { Currency = "DKK" };
        var createResponse = await Client.PostAsJsonAsync("/api/equities", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<EquityDto>();
        
        var updateDto = new UpdateEquityDto { Currency = "SEK" };

        var response = await Client.PutAsJsonAsync($"/api/equities/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<EquityDto>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("SEK", updated.Currency);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenDoesNotExist()
    {
        var updateDto = new UpdateEquityDto { Currency = "NOK" };

        var response = await Client.PutAsJsonAsync("/api/equities/99999", updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesEquity_WhenExistsAndHasNoDependencies()
    {
        var createDto = new CreateEquityDto { Currency = "DKK" };
        var createResponse = await Client.PostAsJsonAsync("/api/equities", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<EquityDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/equities/{created!.Id}");
        var getResponse = await Client.GetAsync($"/api/equities/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await Client.DeleteAsync("/api/equities/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByIdWithLoans_ReturnsEquityWithLoans_WhenLoansExist()
    {
        var createEquityDto = new CreateEquityDto { Currency = "DKK" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", createEquityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var createLoanDto = new CreateLoanDto
        {
            EquityId = equity!.Id,
            LoanType = "FIXED",
            LoanAmount = 500000.0,
            InterestRate = 3.5,
            LoanLifetime = 30
        };
        await Client.PostAsJsonAsync("/api/loans", createLoanDto);

        var response = await Client.GetAsync($"/api/equities/{equity.Id}/with-loans");

        response.EnsureSuccessStatusCode();
        var equityWithLoans = await response.Content.ReadFromJsonAsync<EquityDto>();
        Assert.NotNull(equityWithLoans);
        Assert.NotNull(equityWithLoans.Loans);
        Assert.Single(equityWithLoans.Loans);
        Assert.Equal(500000.0, equityWithLoans.Loans.First().LoanAmount);
    }

    [Fact]
    public async Task GetByIdWithLoans_ReturnsNotFound_WhenEquityDoesNotExist()
    {
        var response = await Client.GetAsync("/api/equities/99999/with-loans");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}