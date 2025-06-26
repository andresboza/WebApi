using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApi.DTOs;
using Xunit;

namespace WebApi.IntegrationTests
{
    public class PositionsEndpointsTests
        : IClassFixture<WebApplicationFactory<Program>>,
          IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private string _connectionString = "";

        public PositionsEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7023");
            _client.DefaultRequestHeaders.Remove("X-Api-Key");
        }

        public async Task InitializeAsync()
        {
            // 1. Obtén la IConfiguration que usa tu API
            var config = _factory.Services.GetRequiredService<IConfiguration>();
            _connectionString = config.GetConnectionString("OracleDb");

            // 2. Conéctate a Oracle y trunca la tabla
            await using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "TRUNCATE TABLE POSITIONS";
            await cmd.ExecuteNonQueryAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private void AddApiKey(string key)
        {
            if (_client.DefaultRequestHeaders.Contains("X-Api-Key"))
                _client.DefaultRequestHeaders.Remove("X-Api-Key");
            _client.DefaultRequestHeaders.Add("X-Api-Key", key);
        }

        [Fact]
        public async Task GetAll_InitiallyEmpty_Returns200AndEmptyList()
        {
            AddApiKey("123456SECRET");

            var resp = await _client.GetAsync("/api/Positions/GetAll");
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var list = await resp.Content.ReadFromJsonAsync<PositionDTO[]>();
            list.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task Crud_FullFlow_WorksAsExpected()
        {
            AddApiKey("123456SECRET");

            //crear
            var createDto = new PositionDTO
            {
                Title = "INT Dev",
                Description = "Integration test flow",
                Location = "CR",
                Status = "open",
                RecruiterId = 10,
                DepartmentId = 20,
                Budget = 1500.00m,
                ClosingDate = DateTime.UtcNow.Date.AddDays(30)
            };

            var createResp = await _client.PostAsJsonAsync("/api/Positions/Create", createDto);
            createResp.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await createResp.Content.ReadFromJsonAsync<PositionDTO>();
            created.Should().NotBeNull();
            created!.Id.Should().BeGreaterThan(0);

            //obtener por id
            var getResp = await _client.GetAsync($"/api/Positions/Get/{created.Id}");
            getResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var fetched = await getResp.Content.ReadFromJsonAsync<PositionDTO>();
            fetched.Should().NotBeNull().And.BeEquivalentTo(created);

            //actualizar
            var updateDto = new PositionDTO
            {
                Title = created.Title + " Updated",
                Description = created.Description,
                Location = created.Location,
                Status = "closed",
                RecruiterId = created.RecruiterId,
                DepartmentId = created.DepartmentId,
                Budget = created.Budget + 500,
                ClosingDate = created.ClosingDate
            };
            var updateResp = await _client.PutAsJsonAsync($"/api/Positions/Update/{created.Id}", updateDto);
            updateResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

            //obtener por id
            var getAfter = await _client.GetAsync($"/api/Positions/Get/{created.Id}");
            getAfter.StatusCode.Should().Be(HttpStatusCode.OK);
            var afterDto = await getAfter.Content.ReadFromJsonAsync<PositionDTO>();
            afterDto!.Title.Should().Be(updateDto.Title);

            //filtro
            var filterResp = await _client.GetAsync($"/api/Positions/Filter?status=closed&location={Uri.EscapeDataString(created.Location)}&page=1&pageSize=5");
            filterResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var filtered = await filterResp.Content.ReadFromJsonAsync<PositionDTO[]>();
            filtered.Should().ContainSingle(p => p.Id == created.Id);

            //eliminar
            var delResp = await _client.DeleteAsync($"/api/Positions/Delete/{created.Id}");
            delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

            //no encontrado
            var get404 = await _client.GetAsync($"/api/Positions/Get/{created.Id}");
            get404.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
