using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using BoligInfo.Tests.Integration.Setup;

namespace BoligInfo.Tests.Integration.Integration;

public class EdgeCaseTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetAll_ReturnsMultipleEquities_InCorrectFormat()
    {
        await Client.PostAsJsonAsync("/api/equities", new CreateEquityDto { Currency = "DKK" });
        await Client.PostAsJsonAsync("/api/equities", new CreateEquityDto { Currency = "USD" });
        await Client.PostAsJsonAsync("/api/equities", new CreateEquityDto { Currency = "EUR" });

        var response = await Client.GetAsync("/api/equities");

        response.EnsureSuccessStatusCode();
        var equities = await response.Content.ReadFromJsonAsync<IEnumerable<EquityDto>>();
        Assert.NotNull(equities);
        Assert.True(equities.Count() >= 3);
    }

    [Fact]
    public async Task Update_WithNoChanges_ReturnsSuccessfully()
    {
        var createDto = new CreateEquityDto { Currency = "DKK" };
        var createResponse = await Client.PostAsJsonAsync("/api/equities", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<EquityDto>();

        var updateDto = new UpdateEquityDto { Currency = "DKK" };
        var response = await Client.PutAsJsonAsync($"/api/equities/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Loan_WithVeryLargeValues_HandlesCorrectly()
    {
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", new CreateEquityDto());
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var createDto = new CreateLoanDto
        {
            EquityId = equity!.Id,
            LoanAmount = 999999999.99,
            InterestRate = 99.99,
            LoanLifetime = int.MaxValue
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        response.EnsureSuccessStatusCode();
        var loan = await response.Content.ReadFromJsonAsync<LoanDto>();
        Assert.Equal(999999999.99, loan!.LoanAmount);
        Assert.Equal(99.99, loan.InterestRate);
    }

    [Fact]
    public async Task MultipleOperations_OnSameEntity_MaintainConsistency()
    {
        var createResponse = await Client.PostAsJsonAsync("/api/equities", new CreateEquityDto { Currency = "DKK" });
        var equity = await createResponse.Content.ReadFromJsonAsync<EquityDto>();

        await Client.PutAsJsonAsync($"/api/equities/{equity!.Id}", new UpdateEquityDto { Currency = "USD" });
        await Client.PutAsJsonAsync($"/api/equities/{equity.Id}", new UpdateEquityDto { Currency = "EUR" });
        
        var getResponse = await Client.GetAsync($"/api/equities/{equity.Id}");
        var final = await getResponse.Content.ReadFromJsonAsync<EquityDto>();

        Assert.Equal("EUR", final!.Currency);
    }

    [Fact]
    public async Task ConcurrentCreation_GeneratesUniqueIds()
    {
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", new CreateEquityDto());
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var tasks = Enumerable.Range(0, 10).Select(i => 
            Client.PostAsJsonAsync("/api/loans", new CreateLoanDto
            {
                EquityId = equity!.Id,
                LoanAmount = 100000.0 * (i + 1),
                InterestRate = 2.5 + i * 0.1,
                LoanLifetime = 10 + i
            })
        ).ToArray();

        await Task.WhenAll(tasks);

        var loansResponse = await Client.GetAsync($"/api/loans/equity/{equity!.Id}");
        var loans = await loansResponse.Content.ReadFromJsonAsync<IEnumerable<LoanDto>>();

        var loanDtos = loans!.ToList();
        Assert.Equal(10, loanDtos.Count);
        Assert.Equal(10, loanDtos.Select(l => l.Id).Distinct().Count());
    }
}