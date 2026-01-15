using System.Net;
using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using BoligInfo.Tests.Integration.Setup;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BoligInfo.Tests.Integration.Validation;

public class ValidationTests(WebApplicationFactory<Program> factory) : IntegrationTestBase(factory)
{
    private async Task<long> CreateEquityAsync()
    {
        var createDto = new CreateEquityDto { Currency = "DKK" };
        var response = await _client.PostAsJsonAsync("/api/equities", createDto);
        var equity = await response.Content.ReadFromJsonAsync<EquityDto>();
        return equity!.Id;
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

        var response = await _client.PostAsJsonAsync("/api/loans", createDto);

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

        var response = await _client.PostAsJsonAsync("/api/loans", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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

        var response = await _client.PostAsJsonAsync("/api/loans", createDto);

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

        var response = await _client.PostAsJsonAsync("/api/loans", createDto);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Equity_AcceptsNullCurrency()
    {
        var createDto = new CreateEquityDto { Currency = null };

        var response = await _client.PostAsJsonAsync("/api/equities", createDto);

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

        var response = await _client.PostAsJsonAsync("/api/equities", createDto);

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
        var validResponse = await _client.PostAsJsonAsync("/api/allcash", validCash);
        validResponse.EnsureSuccessStatusCode();
    }
}