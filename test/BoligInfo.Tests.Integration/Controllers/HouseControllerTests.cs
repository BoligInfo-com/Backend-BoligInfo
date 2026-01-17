using System.Net;
using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using BoligInfo.Tests.Integration.Setup;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BoligInfo.Tests.Integration.Controllers;

public class HouseControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private async Task<long> CreateEquityAsync(string currency = "DKK")
    {
        var createDto = new CreateEquityDto { Currency = currency };
        var response = await Client.PostAsJsonAsync("/api/equities", createDto);
        var equity = await response.Content.ReadFromJsonAsync<EquityDto>();
        return equity!.Id;
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoHouses()
    {
        await CleanDatabaseAsync();

        var response = await Client.GetAsync("/api/houses");

        response.EnsureSuccessStatusCode();
        var houses = await response.Content.ReadFromJsonAsync<IEnumerable<HouseDto>>();
        Assert.NotNull(houses);
        Assert.Empty(houses);
    }

    [Fact]
    public async Task Create_ReturnsCreatedHouse_WithValidData()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2500000.0,
            NumberOfRooms = 4,
            SquareMeters = 120,
            EnergyLabel = "B",
            PurchaseDate = new DateOnly(2024, 1, 15)
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var house = await response.Content.ReadFromJsonAsync<HouseDto>();
        Assert.NotNull(house);
        Assert.True(house.Id > 0);
        Assert.Equal(equityId, house.EquityId);
        Assert.Equal(2500000.0, house.Price);
        Assert.Equal(4, house.NumberOfRooms);
        Assert.Equal(120, house.SquareMeters);
        Assert.Equal("B", house.EnergyLabel);
        Assert.Equal(new DateOnly(2024, 1, 15), house.PurchaseDate);
    }

    [Theory]
    [InlineData("A2020")]
    [InlineData("A2015")]
    [InlineData("A2010")]
    [InlineData("B")]
    [InlineData("C")]
    [InlineData("D")]
    [InlineData("E")]
    [InlineData("F")]
    [InlineData("G")]
    [InlineData("NOT_AVAILABLE")]
    public async Task Create_AcceptsAllEnergyLabels(string energyLabel)
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 1500000.0,
            EnergyLabel = energyLabel
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        response.EnsureSuccessStatusCode();
        var house = await response.Content.ReadFromJsonAsync<HouseDto>();
        Assert.NotNull(house);
        Assert.Equal(energyLabel, house.EnergyLabel);
    }

    [Fact]
    public async Task Create_AllowsNullOptionalFields()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 1800000.0,
            NumberOfRooms = null,
            SquareMeters = null,
            EnergyLabel = null,
            PurchaseDate = null
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        response.EnsureSuccessStatusCode();
        var house = await response.Content.ReadFromJsonAsync<HouseDto>();
        Assert.NotNull(house);
        Assert.Equal(1800000.0, house.Price);
        Assert.Null(house.NumberOfRooms);
        Assert.Null(house.SquareMeters);
        Assert.Null(house.EnergyLabel);
        Assert.Null(house.PurchaseDate);
    }

    [Fact]
    public async Task Create_ReturnsError_WhenEquityDoesNotExist()
    {
        var createDto = new CreateHouseDto
        {
            EquityId = 99999,
            Price = 2000000.0
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_RejectsNegativePrice()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = -100000.0
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_RejectsNegativeNumberOfRooms()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2000000.0,
            NumberOfRooms = -1
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_RejectsNegativeSquareMeters()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2000000.0,
            SquareMeters = -50
        };

        var response = await Client.PostAsJsonAsync("/api/houses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsHouse_WhenExists()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 3000000.0,
            NumberOfRooms = 5,
            SquareMeters = 150
        };
        var createResponse = await Client.PostAsJsonAsync("/api/houses", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<HouseDto>();

        var response = await Client.GetAsync($"/api/houses/{created!.Id}");

        response.EnsureSuccessStatusCode();
        var house = await response.Content.ReadFromJsonAsync<HouseDto>();
        Assert.NotNull(house);
        Assert.Equal(created.Id, house.Id);
        Assert.Equal(3000000.0, house.Price);
        Assert.Equal(5, house.NumberOfRooms);
        Assert.Equal(150, house.SquareMeters);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await Client.GetAsync("/api/houses/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByEquityId_ReturnsAllHousesForEquity()
    {
        var equityId = await CreateEquityAsync();
        
        await Client.PostAsJsonAsync("/api/houses", new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2000000.0,
            NumberOfRooms = 3
        });
        
        await Client.PostAsJsonAsync("/api/houses", new CreateHouseDto
        {
            EquityId = equityId,
            Price = 3500000.0,
            NumberOfRooms = 5
        });

        var response = await Client.GetAsync($"/api/houses/equity/{equityId}");

        response.EnsureSuccessStatusCode();
        var houses = await response.Content.ReadFromJsonAsync<IEnumerable<HouseDto>>();
        Assert.NotNull(houses);
        Assert.Equal(2, houses.Count());
        Assert.All(houses, h => Assert.Equal(equityId, h.EquityId));
    }

    [Fact]
    public async Task GetByEquityId_ReturnsEmptyList_WhenNoHousesForEquity()
    {
        var equityId = await CreateEquityAsync();

        var response = await Client.GetAsync($"/api/houses/equity/{equityId}");

        response.EnsureSuccessStatusCode();
        var houses = await response.Content.ReadFromJsonAsync<IEnumerable<HouseDto>>();
        Assert.NotNull(houses);
        Assert.Empty(houses);
    }

    [Fact]
    public async Task Update_ModifiesHouse_WhenExists()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2000000.0,
            NumberOfRooms = 3,
            SquareMeters = 100,
            EnergyLabel = "C"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/houses", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<HouseDto>();
        
        var updateDto = new UpdateHouseDto
        {
            Price = 2200000.0,
            NumberOfRooms = 4,
            SquareMeters = 110,
            EnergyLabel = "B"
        };

        var response = await Client.PutAsJsonAsync($"/api/houses/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<HouseDto>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal(2200000.0, updated.Price);
        Assert.Equal(4, updated.NumberOfRooms);
        Assert.Equal(110, updated.SquareMeters);
        Assert.Equal("B", updated.EnergyLabel);
    }

    [Fact]
    public async Task Update_PartialUpdate_OnlyModifiesProvidedFields()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2000000.0,
            NumberOfRooms = 3,
            SquareMeters = 100,
            EnergyLabel = "C"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/houses", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<HouseDto>();
        
        var updateDto = new UpdateHouseDto { Price = 2100000.0 };

        var response = await Client.PutAsJsonAsync($"/api/houses/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<HouseDto>();
        Assert.NotNull(updated);
        Assert.Equal(2100000.0, updated.Price);
        Assert.Equal(3, updated.NumberOfRooms);
        Assert.Equal(100, updated.SquareMeters);
        Assert.Equal("C", updated.EnergyLabel);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenDoesNotExist()
    {
        var updateDto = new UpdateHouseDto { Price = 2000000.0 };

        var response = await Client.PutAsJsonAsync("/api/houses/99999", updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesHouse_WhenExists()
    {
        var equityId = await CreateEquityAsync();
        var createDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 1500000.0
        };
        var createResponse = await Client.PostAsJsonAsync("/api/houses", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<HouseDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/houses/{created!.Id}");
        var getResponse = await Client.GetAsync($"/api/houses/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await Client.DeleteAsync("/api/houses/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByIdWithCashFlows_ReturnsHouseWithCashFlows_WhenCashFlowsExist()
    {
        var equityId = await CreateEquityAsync();
        var createHouseDto = new CreateHouseDto
        {
            EquityId = equityId,
            Price = 2500000.0
        };
        var houseResponse = await Client.PostAsJsonAsync("/api/houses", createHouseDto);
        var house = await houseResponse.Content.ReadFromJsonAsync<HouseDto>();

        var createCashFlowDto = new CreateCashFlowDto
        {
            HouseId = house!.Id,
            Type = "EXPENSE",
            Frequency = "MONTHLY",
            Amount = 15000.0,
            Name = "Mortgage Payment"
        };
        await Client.PostAsJsonAsync("/api/cashflows", createCashFlowDto);

        var response = await Client.GetAsync($"/api/houses/{house.Id}/with-cashflows");

        response.EnsureSuccessStatusCode();
        var houseWithCashFlows = await response.Content.ReadFromJsonAsync<HouseDto>();
        Assert.NotNull(houseWithCashFlows);
        Assert.NotNull(houseWithCashFlows.CashFlows);
        Assert.Single(houseWithCashFlows.CashFlows);
        Assert.Equal(15000.0, houseWithCashFlows.CashFlows.First().Amount);
    }

    [Fact]
    public async Task GetByIdWithCashFlows_ReturnsNotFound_WhenHouseDoesNotExist()
    {
        var response = await Client.GetAsync("/api/houses/99999/with-cashflows");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}