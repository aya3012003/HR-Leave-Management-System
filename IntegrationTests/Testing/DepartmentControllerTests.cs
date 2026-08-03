using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project_3.src.Application.DTOs.AuthDto;
using Project_3.src.Application.DTOs.DepartmentDTOs;
using Project_3.src.Application.Models;
using Project_3.src.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Testing
{
    public class DepartmentControllerTests
      : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DepartmentControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {

                });

            }).CreateClient();
        }
        [Fact]
        public async Task GetDepartments_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/Department");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.NotNull(content);
            Assert.NotEmpty(content);
        }
        [Fact]
        public async Task GetDepartmentById_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/Department/1");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task CreateDepartment_ReturnsCreated()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var dto = new CreateDepartmentDto
            {
                Name = $"Test Dept {Guid.NewGuid().ToString("N")[..8]}"
            };

            var response =
                await _client.PostAsJsonAsync("/api/Department", dto);

            var content = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Status: {response.StatusCode}, Response: {content}"
            );

            var department =
                await response.Content.ReadFromJsonAsync<DepartmentDto>();

            Assert.NotNull(department);
        }
        [Fact]
        public async Task UpdateDepartment_ReturnsOk()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            var dto = new UpdateDepartmentDto
            {
                Name = "Updated Department"
            };

            var response = await _client.PutAsJsonAsync("/api/Department/1", dto);

            response.EnsureSuccessStatusCode();

            var department =
                await response.Content.ReadFromJsonAsync<DepartmentDto>();

            Assert.Equal("Updated Department", department.Name);
        }
        [Fact]
        public async Task DeleteDepartment_ReturnsNoContent()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync("/api/Department/1");

            var content = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode,
                $"Status: {response.StatusCode}\nResponse: {content}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        [Fact]
        public async Task GetDepartment_InvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/Department/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task CreateDepartment_WithoutLogin_ReturnsUnauthorized()
        {
           _client.DefaultRequestHeaders.Clear();
            var dto = new CreateDepartmentDto
            {
                Name = "Test"
            };

            var response = await _client.PostAsJsonAsync("/api/Department", dto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        private async Task<string> GetAdminToken()
        {
            var dto = new LoginDto
            {
                Email = "john@gmail.com",
                Password = "Password@123"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/login", dto);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

            return result!.AccessToken;
        }
    }
}
