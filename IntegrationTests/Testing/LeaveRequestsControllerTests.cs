using Microsoft.AspNetCore.Mvc.Testing;
using Project_3.src.Application.DTOs.AuthDto;
using Project_3.src.Application.DTOs.LeaveRequestDTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace IntegrationTests.Testing
{
    public class LeaveRequestsControllerTests
         : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public LeaveRequestsControllerTests(
            WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {

                });

            }).CreateClient();
        }



        [Fact]
        public async Task GetAll_WithAdmin_ReturnsOk()
        {
            var token = await GetAdminToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var response =
                await _client.GetAsync(
                    "/api/v1/LeaveRequests");


            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }



        [Fact]
        public async Task GetMyRequests_WithEmployee_ReturnsOk()
        {
            var token = await GetEmployeeToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var response =
                await _client.GetAsync(
                    "/api/v1/LeaveRequests/my");


            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }




        [Fact]
        public async Task CreateLeaveRequest_WithEmployee_ReturnsCreated()
        {
            var token = await GetEmployeeToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);



            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                Reason = "Integration test request"
            };


            var response =
                await _client.PostAsJsonAsync(
                    "/api/v1/LeaveRequests",
                    dto);



            var content =
                await response.Content.ReadAsStringAsync();


            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Status: {response.StatusCode}\nResponse: {content}"
            );
        }




        [Fact]
        public async Task GetLeaveRequestById_WithEmployee_ReturnsOk()
        {
            var token = await GetEmployeeToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);



            var response =
                await _client.GetAsync(
                    "/api/v1/LeaveRequests/1");


            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

       

        [Fact]
        public async Task GetAll_WithoutLogin_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Clear();


            var response =
                await _client.GetAsync(
                    "/api/v1/LeaveRequests");


            Assert.Equal(
                HttpStatusCode.Unauthorized,
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
                await _client.PostAsJsonAsync("/api/Auth/login",dto);


            response.EnsureSuccessStatusCode();


            var result =
                await response.Content
                .ReadFromJsonAsync<AuthResponseDto>();


            return result!.AccessToken;
        }
    }
}
