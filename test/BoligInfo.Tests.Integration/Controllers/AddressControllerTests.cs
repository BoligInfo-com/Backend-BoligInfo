using System.Net;
using System.Net.Http.Json;
using BoligInfo.Core.DTO;
using BoligInfo.Tests.Integration.Setup;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BoligInfo.Tests.Integration.Controllers;

public class AddressControllerTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
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
    public async Task GetAll_ReturnsEmptyList_WhenNoAddresses()
    {
        await CleanDatabaseAsync();

        var response = await Client.GetAsync("/api/addresses");

        response.EnsureSuccessStatusCode();
        var addresses = await response.Content.ReadFromJsonAsync<IEnumerable<AddressDto>>();
        Assert.NotNull(addresses);
        Assert.Empty(addresses);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAddress_WithValidData()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            Country = "Denmark",
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Nørrebrogade",
            Number = "45",
            Suite = "2A",
            Floor = 3
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var address = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(address);
        Assert.True(address.Id > 0);
        Assert.Equal(houseId, address.HouseId);
        Assert.Equal("Denmark", address.Country);
        Assert.Equal("Copenhagen", address.City);
        Assert.Equal("2100", address.Zipcode);
        Assert.Equal("Nørrebrogade", address.Street);
        Assert.Equal("45", address.Number);
        Assert.Equal("2A", address.Suite);
        Assert.Equal(3, address.Floor);
    }

    [Fact]
    public async Task Create_UsesDefaultCountry_WhenNotProvided()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Aarhus",
            Zipcode = "8000",
            Street = "Vestergade"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
        var address = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(address);
        Assert.Equal("Denmark", address.Country);
    }

    [Fact]
    public async Task Create_AllowsNullOptionalFields()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Odense",
            Zipcode = "5000",
            Street = "Kongensgade",
            Number = null,
            Suite = null,
            Floor = null
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
        var address = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(address);
        Assert.Null(address.Number);
        Assert.Null(address.Suite);
        Assert.Null(address.Floor);
    }

    [Fact]
    public async Task Create_ReturnsError_WhenHouseDoesNotExist()
    {
        var createDto = new CreateAddressDto
        {
            HouseId = 99999,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_RejectsDuplicateAddress_WhenHouseAlreadyHasAddress()
    {
        var houseId = await CreateHouseAsync();
        
        // Create first address - should succeed
        var firstAddressDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = "Street 1"
        };
        var firstResponse = await Client.PostAsJsonAsync("/api/addresses", firstAddressDto);
        firstResponse.EnsureSuccessStatusCode();
        
        // Try to create second address for same house - should fail
        var secondAddressDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Aarhus",
            Zipcode = "8000",
            Street = "Street 2"
        };
        var secondResponse = await Client.PostAsJsonAsync("/api/addresses", secondAddressDto);

        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsAddress_WhenExists()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Aalborg",
            Zipcode = "9000",
            Street = "Boulevarden",
            Number = "12"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/addresses", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<AddressDto>();

        var response = await Client.GetAsync($"/api/addresses/{created!.Id}");

        response.EnsureSuccessStatusCode();
        var address = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(address);
        Assert.Equal(created.Id, address.Id);
        Assert.Equal("Aalborg", address.City);
        Assert.Equal("9000", address.Zipcode);
        Assert.Equal("Boulevarden", address.Street);
        Assert.Equal("12", address.Number);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await Client.GetAsync("/api/addresses/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByHouseId_ReturnsAddress_WhenExists()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Esbjerg",
            Zipcode = "6700",
            Street = "Torvegade"
        };
        await Client.PostAsJsonAsync("/api/addresses", createDto);

        var response = await Client.GetAsync($"/api/addresses/house/{houseId}");

        response.EnsureSuccessStatusCode();
        var address = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(address);
        Assert.Equal(houseId, address.HouseId);
        Assert.Equal("Esbjerg", address.City);
    }

    [Fact]
    public async Task GetByHouseId_ReturnsNotFound_WhenNoAddressForHouse()
    {
        var houseId = await CreateHouseAsync();

        var response = await Client.GetAsync($"/api/addresses/house/{houseId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ModifiesAddress_WhenExists()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            Country = "Denmark",
            City = "Roskilde",
            Zipcode = "4000",
            Street = "Algade",
            Number = "10",
            Floor = 2
        };
        var createResponse = await Client.PostAsJsonAsync("/api/addresses", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<AddressDto>();
        
        var updateDto = new UpdateAddressDto
        {
            Country = "Sweden",
            City = "Malmö",
            Zipcode = "211 22",
            Street = "Stortorget",
            Number = "15",
            Floor = 3
        };

        var response = await Client.PutAsJsonAsync($"/api/addresses/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Sweden", updated.Country);
        Assert.Equal("Malmö", updated.City);
        Assert.Equal("211 22", updated.Zipcode);
        Assert.Equal("Stortorget", updated.Street);
        Assert.Equal("15", updated.Number);
        Assert.Equal(3, updated.Floor);
    }

    [Fact]
    public async Task Update_PartialUpdate_OnlyModifiesProvidedFields()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Kolding",
            Zipcode = "6000",
            Street = "Jernbanegade",
            Number = "5",
            Floor = 1
        };
        var createResponse = await Client.PostAsJsonAsync("/api/addresses", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<AddressDto>();
        
        var updateDto = new UpdateAddressDto { City = "Vejle" };

        var response = await Client.PutAsJsonAsync($"/api/addresses/{created!.Id}", updateDto);

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(updated);
        Assert.Equal("Vejle", updated.City);
        Assert.Equal("6000", updated.Zipcode);
        Assert.Equal("Jernbanegade", updated.Street);
        Assert.Equal("5", updated.Number);
        Assert.Equal(1, updated.Floor);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenDoesNotExist()
    {
        var updateDto = new UpdateAddressDto { City = "Copenhagen" };

        var response = await Client.PutAsJsonAsync("/api/addresses/99999", updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesAddress_WhenExists()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Horsens",
            Zipcode = "8700",
            Street = "Søndergade"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/addresses", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<AddressDto>();

        var deleteResponse = await Client.DeleteAsync($"/api/addresses/{created!.Id}");
        var getResponse = await Client.GetAsync($"/api/addresses/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await Client.DeleteAsync("/api/addresses/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_AcceptsLongCityName()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = new string('A', 340),
            Zipcode = "1234",
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Create_RejectsCityNameTooLong()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = new string('A', 341),
            Zipcode = "1234",
            Street = "Main Street"
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_AcceptsLongStreetName()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = new string('B', 340)
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Create_RejectsStreetNameTooLong()
    {
        var houseId = await CreateHouseAsync();
        var createDto = new CreateAddressDto
        {
            HouseId = houseId,
            City = "Copenhagen",
            Zipcode = "2100",
            Street = new string('B', 341)
        };

        var response = await Client.PostAsJsonAsync("/api/addresses", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}