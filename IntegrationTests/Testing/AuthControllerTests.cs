using Microsoft.AspNetCore.Mvc.Testing;
using Project_3.src.Application.DTOs.AuthDto;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace IntegrationTests.Testing
{
    public class AuthControllerTests
         : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidAdminCredentials_ReturnsToken()
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = "john@gmail.com",
                Password = "Password@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/login", dto);

            var content = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode,
                $"Status: {response.StatusCode}\nResponse: {content}");

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
            Assert.Contains("Admin", result.Roles);
            Assert.Equal("john@gmail.com", result.UserName);
        }
        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var dto = new LoginDto
            {
                Email = "wrong@gmail.com",
                Password = "WrongPassword"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/login", dto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
     
    }
}

