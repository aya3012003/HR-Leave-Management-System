using Microsoft.AspNetCore.Mvc.Testing;
using Project_3.src.Application.DTOs.AuthDto;
using Project_3.src.Application.DTOs.EmployeeDTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace IntegrationTests.Testing
{
    [Collection("Sequential Integration Tests")]
    public class EmployeeControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private static string? _cachedAdminToken;
        private static string? _cachedEmployeeToken;

        public EmployeeControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetEmployees_WithAdmin_ReturnsOk()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/v1/Employees");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task GetEmployeeById_WithAdmin_ReturnsOk()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var createDto = new CreateEmployeeDto
            {
                FirstName = "Get",
                LastName = "Test",
                Email = $"get{Guid.NewGuid()}@gmail.com",
                Password = "Password@123",
                DepartmentId = 1,
                Role = "Employee"
            };


            var createResponse =
                await _client.PostAsJsonAsync("/api/v1/Employees", createDto);


            var employee =
                await createResponse.Content.ReadFromJsonAsync<EmployeeDto>();


            var response =
                await _client.GetAsync(
                    $"/api/v1/Employees/{employee!.Id}");


            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetMyProfile_ReturnsOk()
        {
            var token = await GetEmployeeToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/v1/Employees/me");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateEmployee_WithAdmin_ReturnsCreated()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var dto = new CreateEmployeeDto
            {
                FirstName = "Test",
                LastName = "Employee",
                Email = $"testemployee{Guid.NewGuid()}@gmail.com",
                Password = "Password@123",
                DepartmentId = 1,
                Role = "Employee"
            };

            var response = await _client.PostAsJsonAsync("/api/v1/Employees", dto);

            var content = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Status: {response.StatusCode}, Response: {content}"
            );
        }

        [Fact]
        public async Task UpdateEmployee_WithAdmin_ReturnsOk()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var createDto = new CreateEmployeeDto
            {
                FirstName = "Before",
                LastName = "Update",
                Email = $"update{Guid.NewGuid()}@gmail.com",
                Password = "Password@123",
                DepartmentId = 1,
                Role = "Employee"
            };


            var createResponse =
                await _client.PostAsJsonAsync("/api/v1/Employees", createDto);


            var employee =
                await createResponse.Content.ReadFromJsonAsync<EmployeeDto>();


            var updateDto = new UpdateEmployeeDto
            {
                FirstName = "Updated Name",
                LastName = "Employee"
            };


            var response =
                await _client.PutAsJsonAsync(
                    $"/api/v1/Employees/{employee!.Id}",
                    updateDto);


            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task UpdateMyProfile_ReturnsOk()
        {
            var token = await GetEmployeeToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var dto = new UpdateEmployeeDto
            {
                FirstName = "My Updated Name"
            };

            var response = await _client.PutAsJsonAsync("/api/v1/Employees/me", dto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteEmployee_WithAdmin_ReturnsNoContent()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var createDto = new CreateEmployeeDto
            {
                FirstName = "Delete",
                LastName = "Test",
                Email = $"delete{Guid.NewGuid()}@gmail.com",
                Password = "Password@123",
                DepartmentId = 1,
                Role = "Employee"
            };


            var createResponse =
                await _client.PostAsJsonAsync("/api/v1/Employees", createDto);


            var created =
                await createResponse.Content.ReadFromJsonAsync<EmployeeDto>();


            var deleteResponse =
                await _client.DeleteAsync(
                    $"/api/v1/Employees/{created!.Id}");


            Assert.Equal(
                HttpStatusCode.NoContent,
                deleteResponse.StatusCode);
        }


        [Fact]
        public async Task CreateEmployee_WithoutLogin_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Clear();

            var dto = new CreateEmployeeDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"testemployee{Guid.NewGuid()}@gmail.com",
                Password = "Password@123",
                DepartmentId = 1,
                Role = "Employee"
            };

            var response = await _client.PostAsJsonAsync("/api/v1/Employees", dto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateEmployee_WithEmployee_ReturnsForbidden()
        {
            var token = await GetEmployeeToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var dto = new CreateEmployeeDto
            {
                FirstName = "No",
                LastName = "Permission",
                Email = "nopermission@gmail.com",
                Password = "Password@123",
                DepartmentId = 1,
                Role = "Employee"
            };

            var response = await _client.PostAsJsonAsync("/api/v1/Employees", dto);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

       
        private async Task<string> GetAdminToken()
        {
            if (!string.IsNullOrEmpty(_cachedAdminToken))
                return _cachedAdminToken;

            var dto = new LoginDto
            {
                Email = "john@gmail.com",
                Password = "Password@123"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/login", dto);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            _cachedAdminToken = result!.AccessToken;

            return _cachedAdminToken;
        }

        private async Task<string> GetEmployeeToken()
        {
            if (!string.IsNullOrEmpty(_cachedEmployeeToken))
                return _cachedEmployeeToken;

            var dto = new LoginDto
            {
                Email = "bob@gmail.com",
                Password = "Password@123"
            };


            var response = await _client.PostAsJsonAsync("/api/Auth/login", dto);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            _cachedEmployeeToken = result!.AccessToken;

            return _cachedEmployeeToken;
        }
    }
}
