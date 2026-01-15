using System.Net;
using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using BoligInfo.Tests.Integration.Setup;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BoligInfo.Tests.Integration.Integration;

public class ReferentialIntegrityTests(WebApplicationFactory<Program> factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Delete_Equity_CascadesDeleteToLoans()
    {
        var createEquityDto = new CreateEquityDto { Currency = "DKK" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", createEquityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var createLoanDto = new CreateLoanDto
        {
            EquityId = equity!.Id,
            LoanAmount = 100000.0,
            InterestRate = 3.0,
            LoanLifetime = 15
        };
        var loanResponse = await Client.PostAsJsonAsync("/api/loans", createLoanDto);
        var loan = await loanResponse.Content.ReadFromJsonAsync<LoanDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/equities/{equity.Id}");
        var loanGetResponse = await Client.GetAsync($"/api/loans/{loan!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, loanGetResponse.StatusCode);
    }

    [Fact]
    public async Task Equity_CanHaveOneToOneRelationshipWithCash()
    {
        var createEquityDto = new CreateEquityDto { Currency = "EUR" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", createEquityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var createCashDto = new CreateCashDto
        {
            EquityId = equity!.Id,
            CashAmount = 50000.0
        };
        var cashResponse = await Client.PostAsJsonAsync("/api/allcash", createCashDto);

        cashResponse.EnsureSuccessStatusCode();
        var cash = await cashResponse.Content.ReadFromJsonAsync<CashDto>();
        Assert.NotNull(cash);
        Assert.Equal(equity.Id, cash.EquityId);
    }

    [Fact]
    public async Task Equity_CanHaveMultipleLoans()
    {
        var createEquityDto = new CreateEquityDto { Currency = "DKK" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", createEquityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        await Client.PostAsJsonAsync("/api/loans", new CreateLoanDto
        {
            EquityId = equity!.Id,
            LoanType = "FIXED",
            LoanAmount = 100000.0,
            InterestRate = 2.5,
            LoanLifetime = 10
        });
        
        await Client.PostAsJsonAsync("/api/loans", new CreateLoanDto
        {
            EquityId = equity.Id,
            LoanType = "ADJUSTABLE",
            LoanAmount = 200000.0,
            InterestRate = 3.0,
            LoanLifetime = 20
        });

        var response = await Client.GetAsync($"/api/equities/{equity.Id}/with-loans");

        response.EnsureSuccessStatusCode();
        var equityWithLoans = await response.Content.ReadFromJsonAsync<EquityDto>();
        Assert.NotNull(equityWithLoans);
        Assert.NotNull(equityWithLoans.Loans);
        Assert.Equal(2, equityWithLoans.Loans.Count);
    }

    [Fact]
    public async Task ComplexScenario_EquityWithCashAndMultipleLoans()
    {
        var createEquityDto = new CreateEquityDto { Currency = "USD" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", createEquityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var cash1Response = await Client.PostAsJsonAsync("/api/allcash", new CreateCashDto
        {
            EquityId = equity!.Id,
            CashAmount = 15000.0
        });
        
        var cash2Response = await Client.PostAsJsonAsync("/api/allcash", new CreateCashDto
        {
            EquityId = equity.Id,
            CashAmount = 25000.0
        });

        var loan1Response = await Client.PostAsJsonAsync("/api/loans", new CreateLoanDto
        {
            EquityId = equity.Id,
            LoanType = "F5",
            LoanAmount = 500000.0,
            InterestRate = 3.5,
            LoanLifetime = 30
        });

        var loan2Response = await Client.PostAsJsonAsync("/api/loans", new CreateLoanDto
        {
            EquityId = equity.Id,
            LoanType = "FIXED",
            LoanAmount = 300000.0,
            InterestRate = 2.8,
            LoanLifetime = 20
        });

        cash1Response.EnsureSuccessStatusCode();
        cash2Response.EnsureSuccessStatusCode();
        loan1Response.EnsureSuccessStatusCode();
        loan2Response.EnsureSuccessStatusCode();

        var cashRecords = await Client.GetFromJsonAsync<IEnumerable<CashDto>>($"/api/allcash/equity/{equity.Id}");
        var loanRecords = await Client.GetFromJsonAsync<IEnumerable<LoanDto>>($"/api/loans/equity/{equity.Id}");
        
        Assert.Equal(2, cashRecords!.Count());
        Assert.Equal(2, loanRecords!.Count());
        Assert.Equal(40000.0, cashRecords.Sum(c => c.CashAmount));
        Assert.Equal(800000.0, loanRecords.Sum(l => l.LoanAmount));
    }

    [Fact]
    public async Task DatabaseConstraint_PreventsDuplicateEquitiesWithSameId()
    {
        var context = await GetDbContextAsync();
        
        var equity1 = new Core.Models.Equity { Currency = "DKK" };
        context.Equities.Add(equity1);
        await context.SaveChangesAsync();

        var equity2 = new Core.Models.Equity { Currency = "EUR" };
        context.Equities.Add(equity2);
        await context.SaveChangesAsync();

        Assert.NotEqual(equity1.Id, equity2.Id);
    }
}