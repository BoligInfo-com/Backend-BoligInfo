using System.Net;
using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using BoligInfo.Tests.Integration.Setup;

namespace BoligInfo.Tests.Integration.Integration;

public class ReferentialIntegrityTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    
    [Fact]
    public async Task Delete_House_CascadesDeleteToAddress()
    {
        var equityDto = new CreateEquityDto { Currency = "DKK" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", equityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var houseDto = new CreateHouseDto
        {
            EquityId = equity!.Id,
            Price = 2500000.0
        };
        var houseResponse = await Client.PostAsJsonAsync("/api/houses", houseDto);
        var house = await houseResponse.Content.ReadFromJsonAsync<HouseDto>();

        var addressDto = new CreateAddressDto
        {
            HouseId = house!.Id,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Nørrebrogade"
        };
        var addressResponse = await Client.PostAsJsonAsync("/api/addresses", addressDto);
        var address = await addressResponse.Content.ReadFromJsonAsync<AddressDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/houses/{house.Id}");
        var addressGetResponse = await Client.GetAsync($"/api/addresses/{address!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, addressGetResponse.StatusCode);
    }
    
    [Fact]
    public async Task Delete_Equity_CascadesDeleteToHouseAndAddress()
    {
        var equityDto = new CreateEquityDto { Currency = "EUR" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", equityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var houseDto = new CreateHouseDto
        {
            EquityId = equity!.Id,
            Price = 3000000.0
        };
        var houseResponse = await Client.PostAsJsonAsync("/api/houses", houseDto);
        var house = await houseResponse.Content.ReadFromJsonAsync<HouseDto>();

        var addressDto = new CreateAddressDto
        {
            HouseId = house!.Id,
            City = "Aarhus",
            Zipcode = "8000",
            Street = "Vestergade"
        };
        var addressResponse = await Client.PostAsJsonAsync("/api/addresses", addressDto);
        var address = await addressResponse.Content.ReadFromJsonAsync<AddressDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/equities/{equity.Id}");
        var houseGetResponse = await Client.GetAsync($"/api/houses/{house.Id}");
        var addressGetResponse = await Client.GetAsync($"/api/addresses/{address!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, houseGetResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, addressGetResponse.StatusCode);
    }
    
    [Fact]
    public async Task House_CanHaveOnlyOneAddress()
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

        // First address should succeed
        var address1Dto = new CreateAddressDto
        {
            HouseId = house!.Id,
            City = "Odense",
            Zipcode = "5000",
            Street = "Kongensgade"
        };
        var address1Response = await Client.PostAsJsonAsync("/api/addresses", address1Dto);
        address1Response.EnsureSuccessStatusCode();

        // Second address should fail (one-to-one constraint)
        var address2Dto = new CreateAddressDto
        {
            HouseId = house.Id,
            City = "Esbjerg",
            Zipcode = "6700",
            Street = "Torvegade"
        };
        var address2Response = await Client.PostAsJsonAsync("/api/addresses", address2Dto);

        Assert.Equal(HttpStatusCode.BadRequest, address2Response.StatusCode);
    }
    
    [Fact]
    public async Task ComplexScenario_EquityWithHouseAddressAndCashFlows()
    {
        var equityDto = new CreateEquityDto { Currency = "USD" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", equityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var houseDto = new CreateHouseDto
        {
            EquityId = equity!.Id,
            Price = 3500000.0,
            NumberOfRooms = 5,
            SquareMeters = 150,
            EnergyLabel = "B"
        };
        var houseResponse = await Client.PostAsJsonAsync("/api/houses", houseDto);
        var house = await houseResponse.Content.ReadFromJsonAsync<HouseDto>();

        var addressDto = new CreateAddressDto
        {
            HouseId = house!.Id,
            Country = "Denmark",
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Nørrebrogade",
            Number = "45",
            Floor = 3
        };
        var addressResponse = await Client.PostAsJsonAsync("/api/addresses", addressDto);

        var cashFlow1Dto = new CreateCashFlowDto
        {
            HouseId = house.Id,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 15000.0,
            Name = "Mortgage"
        };
        var cashFlow1Response = await Client.PostAsJsonAsync("/api/cashflows", cashFlow1Dto);

        var cashFlow2Dto = new CreateCashFlowDto
        {
            HouseId = house.Id,
            Type = "INCOME",
            Frequency = "MONTHLY",
            Amount = 20000.0,
            Name = "Rent"
        };
        var cashFlow2Response = await Client.PostAsJsonAsync("/api/cashflows", cashFlow2Dto);

        // Verify all were created successfully
        addressResponse.EnsureSuccessStatusCode();
        cashFlow1Response.EnsureSuccessStatusCode();
        cashFlow2Response.EnsureSuccessStatusCode();

        // Verify address exists for house
        var addressGetResponse = await Client.GetAsync($"/api/addresses/house/{house.Id}");
        addressGetResponse.EnsureSuccessStatusCode();
        var addressResult = await addressGetResponse.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(addressResult);
        Assert.Equal("Copenhagen", addressResult.City);

        // Verify cash flows exist for house
        var cashFlowsResponse = await Client.GetAsync($"/api/cashflows/house/{house.Id}");
        var cashFlows = await cashFlowsResponse.Content.ReadFromJsonAsync<IEnumerable<CashFlowDto>>();
        Assert.NotNull(cashFlows);
        Assert.Equal(2, cashFlows.Count());
    }
    
    [Fact]
    public async Task Delete_Address_AllowsNewAddressForSameHouse()
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

        // Create first address
        var address1Dto = new CreateAddressDto
        {
            HouseId = house!.Id,
            City = "Aalborg",
            Zipcode = "9000",
            Street = "Boulevarden"
        };
        var address1Response = await Client.PostAsJsonAsync("/api/addresses", address1Dto);
        var address1 = await address1Response.Content.ReadFromJsonAsync<AddressDto>();

        // Delete first address
        await Client.DeleteAsync($"/api/addresses/{address1!.Id}");

        // Create new address for same house - should succeed
        var address2Dto = new CreateAddressDto
        {
            HouseId = house.Id,
            City = "Randers",
            Zipcode = "8900",
            Street = "Storegade"
        };
        var address2Response = await Client.PostAsJsonAsync("/api/addresses", address2Dto);

        address2Response.EnsureSuccessStatusCode();
        var address2 = await address2Response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(address2);
        Assert.Equal("Randers", address2.City);
    }
    
    [Fact]
    public async Task Delete_House_CascadesDeleteToCashFlows()
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

        var cashFlowDto = new CreateCashFlowDto
        {
            HouseId = house!.Id,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 10000.0,
            Name = "Mortgage"
        };
        var cashFlowResponse = await Client.PostAsJsonAsync("/api/cashflows", cashFlowDto);
        var cashFlow = await cashFlowResponse.Content.ReadFromJsonAsync<CashFlowDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/houses/{house.Id}");
        var cashFlowGetResponse = await Client.GetAsync($"/api/cashflows/{cashFlow!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, cashFlowGetResponse.StatusCode);
    }
    
    [Fact]
    public async Task Delete_Equity_CascadesDeleteToHousesAndCashFlows()
    {
        var equityDto = new CreateEquityDto { Currency = "EUR" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", equityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var houseDto = new CreateHouseDto
        {
            EquityId = equity!.Id,
            Price = 3000000.0
        };
        var houseResponse = await Client.PostAsJsonAsync("/api/houses", houseDto);
        var house = await houseResponse.Content.ReadFromJsonAsync<HouseDto>();

        var cashFlowDto = new CreateCashFlowDto
        {
            HouseId = house!.Id,
            Type = "INCOME",
            Frequency = "MONTHLY",
            Amount = 15000.0,
            Name = "Rent"
        };
        var cashFlowResponse = await Client.PostAsJsonAsync("/api/cashflows", cashFlowDto);
        var cashFlow = await cashFlowResponse.Content.ReadFromJsonAsync<CashFlowDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/equities/{equity.Id}");
        var houseGetResponse = await Client.GetAsync($"/api/houses/{house.Id}");
        var cashFlowGetResponse = await Client.GetAsync($"/api/cashflows/{cashFlow!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, houseGetResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, cashFlowGetResponse.StatusCode);
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
    public async Task Equity_CanHaveMultipleHouses()
    {
        var equityDto = new CreateEquityDto { Currency = "DKK" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", equityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        await Client.PostAsJsonAsync("/api/houses", new CreateHouseDto
        {
            EquityId = equity!.Id,
            Price = 2000000.0,
            NumberOfRooms = 3
        });

        await Client.PostAsJsonAsync("/api/houses", new CreateHouseDto
        {
            EquityId = equity.Id,
            Price = 3500000.0,
            NumberOfRooms = 5
        });

        var response = await Client.GetAsync($"/api/houses/equity/{equity.Id}");

        response.EnsureSuccessStatusCode();
        var houses = await response.Content.ReadFromJsonAsync<IEnumerable<HouseDto>>();
        Assert.NotNull(houses);
        Assert.Equal(2, houses.Count());
    }
    
    [Fact]
    public async Task House_CanHaveMultipleCashFlows()
    {
        var equityDto = new CreateEquityDto { Currency = "USD" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", equityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        var houseDto = new CreateHouseDto
        {
            EquityId = equity!.Id,
            Price = 2500000.0
        };
        var houseResponse = await Client.PostAsJsonAsync("/api/houses", houseDto);
        var house = await houseResponse.Content.ReadFromJsonAsync<HouseDto>();

        await Client.PostAsJsonAsync("/api/cashflows", new CreateCashFlowDto
        {
            HouseId = house!.Id,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 12000.0,
            Name = "Mortgage"
        });

        await Client.PostAsJsonAsync("/api/cashflows", new CreateCashFlowDto
        {
            HouseId = house.Id,
            Type = "INCOME",
            Frequency = "MONTHLY",
            Amount = 18000.0,
            Name = "Rent"
        });

        await Client.PostAsJsonAsync("/api/cashflows", new CreateCashFlowDto
        {
            HouseId = house.Id,
            Type = "EXPENSE",
            Frequency = "YEARLY",
            Amount = 5000.0,
            Name = "Property Tax"
        });

        var response = await Client.GetAsync($"/api/houses/{house.Id}/with-cashflows");

        response.EnsureSuccessStatusCode();
        var houseWithCashFlows = await response.Content.ReadFromJsonAsync<HouseDto>();
        Assert.NotNull(houseWithCashFlows);
        Assert.NotNull(houseWithCashFlows.CashFlows);
        Assert.Equal(3, houseWithCashFlows.CashFlows.Count);
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

        // Create single cash for this equity (one-to-one relationship)
        var cashResponse = await Client.PostAsJsonAsync("/api/allcash", new CreateCashDto
        {
            EquityId = equity!.Id,
            CashAmount = 40000.0
        });

        // Create multiple loans for this equity (one-to-many relationship)
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

        // Verify all operations succeeded
        cashResponse.EnsureSuccessStatusCode();
        loan1Response.EnsureSuccessStatusCode();
        loan2Response.EnsureSuccessStatusCode();

        // Verify the data was created correctly
        var cashRecords = await Client.GetFromJsonAsync<IEnumerable<CashDto>>($"/api/allcash/equity/{equity.Id}");
        var loanRecords = await Client.GetFromJsonAsync<IEnumerable<LoanDto>>($"/api/loans/equity/{equity.Id}");
    
        var cashDtos = cashRecords!.ToList();
        var loanDtos = loanRecords!.ToList();
        
        Assert.NotNull(cashRecords);
        Assert.NotNull(loanRecords);
        Assert.Single(cashDtos); // One-to-one: only one cash per equity
        Assert.Equal(2, loanDtos.Count); // One-to-many: multiple loans per equity
        Assert.Equal(40000.0, cashDtos.First().CashAmount);
        Assert.Equal(800000.0, loanDtos.Sum(l => l.LoanAmount));
    }

    [Fact]
    public async Task ComplexScenario_EquityWithMultipleHousesAndCashFlows()
    {
        var equityDto = new CreateEquityDto { Currency = "DKK" };
        var equityResponse = await Client.PostAsJsonAsync("/api/equities", equityDto);
        var equity = await equityResponse.Content.ReadFromJsonAsync<EquityDto>();

        // Create first house with cash flows
        var house1Dto = new CreateHouseDto
        {
            EquityId = equity!.Id,
            Price = 2000000.0,
            NumberOfRooms = 3
        };
        var house1Response = await Client.PostAsJsonAsync("/api/houses", house1Dto);
        var house1 = await house1Response.Content.ReadFromJsonAsync<HouseDto>();

        await Client.PostAsJsonAsync("/api/cashflows", new CreateCashFlowDto
        {
            HouseId = house1!.Id,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 10000.0,
            Name = "House 1 Mortgage"
        });

        await Client.PostAsJsonAsync("/api/cashflows", new CreateCashFlowDto
        {
            HouseId = house1.Id,
            Type = "INCOME",
            Frequency = "MONTHLY",
            Amount = 15000.0,
            Name = "House 1 Rent"
        });

        // Create second house with cash flows
        var house2Dto = new CreateHouseDto
        {
            EquityId = equity.Id,
            Price = 3000000.0,
            NumberOfRooms = 5
        };
        var house2Response = await Client.PostAsJsonAsync("/api/houses", house2Dto);
        var house2 = await house2Response.Content.ReadFromJsonAsync<HouseDto>();

        await Client.PostAsJsonAsync("/api/cashflows", new CreateCashFlowDto
        {
            HouseId = house2!.Id,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 15000.0,
            Name = "House 2 Mortgage"
        });

        // Verify data
        var housesResponse = await Client.GetAsync($"/api/houses/equity/{equity.Id}");
        var houses = await housesResponse.Content.ReadFromJsonAsync<IEnumerable<HouseDto>>();

        var house1CashFlowsResponse = await Client.GetAsync($"/api/cashflows/house/{house1.Id}");
        var house1CashFlows = await house1CashFlowsResponse.Content.ReadFromJsonAsync<IEnumerable<CashFlowDto>>();

        var house2CashFlowsResponse = await Client.GetAsync($"/api/cashflows/house/{house2.Id}");
        var house2CashFlows = await house2CashFlowsResponse.Content.ReadFromJsonAsync<IEnumerable<CashFlowDto>>();

        
        var houseDtos = houses!.ToList();
        
        Assert.NotNull(houses);
        Assert.NotNull(house1CashFlows);
        Assert.NotNull(house2CashFlows);
        Assert.Equal(2, houseDtos.Count);
        Assert.Equal(2, house1CashFlows.Count());
        Assert.Single(house2CashFlows);
        Assert.Equal(5000000.0, houseDtos.Sum(h => h.Price));
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