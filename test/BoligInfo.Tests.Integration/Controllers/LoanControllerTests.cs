using System.Net;
using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using IntegrationTestBase = BoligInfo.Tests.Integration.Integration.IntegrationTestBase;

namespace BoligInfo.Tests.Integration.Controllers;

public class LoanControllerTests(WebApplicationFactory<Program> factory) : IntegrationTestBase(factory)
{
    private async Task<long> CreateEquityAsync(string currency = "DKK")
    {
        var createDto = new CreateEquityDto { Currency = currency };
        var response = await Client.PostAsJsonAsync("/api/equities", createDto);
        var equity = await response.Content.ReadFromJsonAsync<EquityDto>();
        return equity!.Id;
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoLoans()
    {
        await CleanDatabaseAsync();

        var response = await Client.GetAsync("/api/loans");

        response.EnsureSuccessStatusCode();
        var loans = await response.Content.ReadFromJsonAsync<IEnumerable<LoanDto>>();
        Assert.NotNull(loans);
        Assert.Empty(loans);
    }

    [Fact]
    public async Task Create_ReturnsCreatedLoan_WithValidData()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanType = "FIXED",
            LoanAmount = 500000.0,
            InterestRate = 3.5,
            LoanLifetime = 30
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var loan = await response.Content.ReadFromJsonAsync<LoanDto>();
        Assert.NotNull(loan);
        Assert.True(loan.Id > 0);
        Assert.Equal(equityId, loan.EquityId);
        Assert.Equal("FIXED", loan.LoanType);
        Assert.Equal(500000.0, loan.LoanAmount);
        Assert.Equal(3.5, loan.InterestRate);
        Assert.Equal(30, loan.LoanLifetime);
    }

    [Theory]
    [InlineData("FIXED")]
    [InlineData("ADJUSTABLE")]
    [InlineData("F5")]
    [InlineData("F4")]
    [InlineData("F3")]
    [InlineData("F2")]
    [InlineData("F1")]
    [InlineData("F_SHORT")]
    [InlineData("OTHER")]
    public async Task Create_AcceptsAllLoanTypes(string loanType)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanType = loanType,
            LoanAmount = 100000.0,
            InterestRate = 2.5,
            LoanLifetime = 20
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        response.EnsureSuccessStatusCode();
        var loan = await response.Content.ReadFromJsonAsync<LoanDto>();
        Assert.NotNull(loan);
        Assert.Equal(loanType, loan.LoanType);
    }

    [Fact]
    public async Task Create_AllowsNullLoanType()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanType = null,
            LoanAmount = 200000.0,
            InterestRate = 3.0,
            LoanLifetime = 25
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        response.EnsureSuccessStatusCode();
        var loan = await response.Content.ReadFromJsonAsync<LoanDto>();
        Assert.NotNull(loan);
        Assert.True(string.IsNullOrEmpty(loan.LoanType));
    }

    [Fact]
    public async Task Create_ReturnsError_WhenEquityDoesNotExist()
    {
        var createDto = new CreateLoanDto
        {
            EquityId = 99999,
            LoanAmount = 100000.0,
            InterestRate = 2.5,
            LoanLifetime = 20
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Create_RejectsNegativeLoanAmount()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanAmount = -50000.0,
            InterestRate = 3.0,
            LoanLifetime = 15
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_RejectsZeroOrNegativeLoanLifetime()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanAmount = 100000.0,
            InterestRate = 2.5,
            LoanLifetime = 0
        };

        var response = await Client.PostAsJsonAsync("/api/loans", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsLoan_WhenExists()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanType = "ADJUSTABLE",
            LoanAmount = 250000.0,
            InterestRate = 2.8,
            LoanLifetime = 15
        };
        var createResponse = await Client.PostAsJsonAsync("/api/loans", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<LoanDto>();

        var response = await Client.GetAsync($"/api/loans/{created!.Id}");

        response.EnsureSuccessStatusCode();
        var loan = await response.Content.ReadFromJsonAsync<LoanDto>();
        Assert.NotNull(loan);
        Assert.Equal(created.Id, loan.Id);
        Assert.Equal("ADJUSTABLE", loan.LoanType);
        Assert.Equal(250000.0, loan.LoanAmount);
        Assert.Equal(2.8, loan.InterestRate);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await Client.GetAsync("/api/loans/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByEquityId_ReturnsAllLoansForEquity()
    {
        var equityId = await CreateEquityAsync();
        
        await Client.PostAsJsonAsync("/api/loans", new CreateLoanDto
        {
            EquityId = equityId,
            LoanType = "FIXED",
            LoanAmount = 100000.0,
            InterestRate = 2.5,
            LoanLifetime = 10
        });
        
        await Client.PostAsJsonAsync("/api/loans", new CreateLoanDto
        {
            EquityId = equityId,
            LoanType = "ADJUSTABLE",
            LoanAmount = 200000.0,
            InterestRate = 3.0,
            LoanLifetime = 20
        });

        var response = await Client.GetAsync($"/api/loans/equity/{equityId}");

        response.EnsureSuccessStatusCode();
        var loans = await response.Content.ReadFromJsonAsync<IEnumerable<LoanDto>>();
        Assert.NotNull(loans);
        Assert.Equal(2, loans.Count());
        Assert.All(loans, l => Assert.Equal(equityId, l.EquityId));
    }

    [Fact]
    public async Task GetByEquityId_ReturnsEmptyList_WhenNoLoansForEquity()
    {
        var equityId = await CreateEquityAsync();

        var response = await Client.GetAsync($"/api/loans/equity/{equityId}");

        response.EnsureSuccessStatusCode();
        var loans = await response.Content.ReadFromJsonAsync<IEnumerable<LoanDto>>();
        Assert.NotNull(loans);
        Assert.Empty(loans);
    }

    [Fact]
    public async Task Update_ModifiesLoan_WhenExists()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanType = "FIXED",
            LoanAmount = 300000.0,
            InterestRate = 3.2,
            LoanLifetime = 25
        };
        var createResponse = await Client.PostAsJsonAsync("/api/loans", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<LoanDto>();
        
        var updateDto = new UpdateLoanDto
        {
            LoanType = "ADJUSTABLE",
            LoanAmount = 350000.0,
            InterestRate = 2.9,
            LoanLifetime = 30
        };

        var response = await Client.PutAsJsonAsync($"/api/loans/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<LoanDto>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("ADJUSTABLE", updated.LoanType);
        Assert.Equal(350000.0, updated.LoanAmount);
        Assert.Equal(2.9, updated.InterestRate);
        Assert.Equal(30, updated.LoanLifetime);
    }

    [Fact]
    public async Task Update_PartialUpdate_OnlyModifiesProvidedFields()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanType = "FIXED",
            LoanAmount = 200000.0,
            InterestRate = 3.5,
            LoanLifetime = 20
        };
        var createResponse = await Client.PostAsJsonAsync("/api/loans", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<LoanDto>();
        
        var updateDto = new UpdateLoanDto { LoanAmount = 225000.0 };

        var response = await Client.PutAsJsonAsync($"/api/loans/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<LoanDto>();
        Assert.NotNull(updated);
        Assert.Equal(225000.0, updated.LoanAmount);
        Assert.Equal("FIXED", updated.LoanType);
        Assert.Equal(3.5, updated.InterestRate);
        Assert.Equal(20, updated.LoanLifetime);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenDoesNotExist()
    {
        var updateDto = new UpdateLoanDto { LoanAmount = 100000.0 };

        var response = await Client.PutAsJsonAsync("/api/loans/99999", updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesLoan_WhenExists()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateLoanDto
        {
            EquityId = equityId,
            LoanAmount = 150000.0,
            InterestRate = 2.7,
            LoanLifetime = 15
        };
        var createResponse = await Client.PostAsJsonAsync("/api/loans", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<LoanDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/loans/{created!.Id}");
        var getResponse = await Client.GetAsync($"/api/loans/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDoesNotExist()
    {
        var response = await Client.DeleteAsync("/api/loans/99999");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}