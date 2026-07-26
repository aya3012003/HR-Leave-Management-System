using Microsoft.AspNetCore.Mvc.Testing;
using Project_3.src.Application.DTOs.AuthDto;
using Project_3.src.Application.DTOs.EmployeeLeaveBalanceDtos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace IntegrationTests.Testing
{
    public class EmployeeLeaveBalancesControllerTests
      : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EmployeeLeaveBalancesControllerTests(
            WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }



        [Fact]
        public async Task GetAll_WithAdmin_ReturnsOk()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var response =
                await _client.GetAsync(
                    "/api/EmployeeLeaveBalances");


            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }





        [Fact]
        public async Task GetById_WithAdmin_ReturnsOk()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);



            var response =
                await _client.GetAsync(
                    "/api/EmployeeLeaveBalances/1");


            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }





        [Fact]
        public async Task Update_WithAdmin_ReturnsOk()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);



            var dto = new UpdateEmployeeLeaveBalanceDto
            {
                RemainingDays = 10
            };


            var response =
                await _client.PutAsJsonAsync(
                    "/api/EmployeeLeaveBalances/1",
                    dto);



            var content =
                await response.Content.ReadAsStringAsync();


            Assert.True(
                response.StatusCode == HttpStatusCode.OK,
                $"Status: {response.StatusCode}\nResponse: {content}"
            );
        }





        [Fact]
        public async Task Delete_WithAdmin_ReturnsNoContent()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);



            var response =
                await _client.DeleteAsync(
                    "/api/EmployeeLeaveBalances/1");



            var content =
                await response.Content.ReadAsStringAsync();


            Assert.True(
                response.StatusCode == HttpStatusCode.NoContent,
                $"Status: {response.StatusCode}\nResponse: {content}"
            );
        }





        [Fact]
        public async Task GetAll_WithoutLogin_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Clear();


            var response =
                await _client.GetAsync(
                    "/api/EmployeeLeaveBalances");


            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }





        [Fact]
        public async Task GetAll_WithEmployee_ReturnsForbidden()
        {
            var token = await GetEmployeeToken();


            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);



            var response =
                await _client.GetAsync(
                    "/api/EmployeeLeaveBalances");


            Assert.Equal(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }





        private async Task<string> GetAdminToken()
        {
            var dto = new LoginDto
            {
                Email = "john@gmail.com",
                Password = "Password@123"
            };


            var response =
                await _client.PostAsJsonAsync(
                    "/api/Auth/login",
                    dto);


            response.EnsureSuccessStatusCode();


            var result =
                await response.Content
                .ReadFromJsonAsync<AuthResponseDto>();


            return result!.AccessToken;
        }





        private async Task<string> GetEmployeeToken()
        {
            var dto = new LoginDto
            {
                Email = "bob@gmail.com",
                Password = "Password@123"
            };


            var response =
                await _client.PostAsJsonAsync(
                    "/api/Auth/login",
                    dto);


            response.EnsureSuccessStatusCode();


            var result =
                await response.Content
                .ReadFromJsonAsync<AuthResponseDto>();


            return result!.AccessToken;
        }
    }
}
