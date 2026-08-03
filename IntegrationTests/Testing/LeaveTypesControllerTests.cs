using Microsoft.AspNetCore.Mvc.Testing;
using Project_3.src.Application.DTOs.LeaveTypeDto;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace IntegrationTests.Testing
{
    public class LeaveTypesControllerTests
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public LeaveTypesControllerTests(
            WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task GetLeaveTypes_ReturnsOk()
        {
            var response =
                await _client.GetAsync("/api/LeaveTypes");


            response.EnsureSuccessStatusCode();


            var content =
                await response.Content.ReadAsStringAsync();


            Assert.NotEmpty(content);
        }



        [Fact]
        public async Task GetLeaveTypeById_ReturnsOk()
        {
            var response =
                await _client.GetAsync("/api/LeaveTypes/1");


            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }



        [Fact]
        public async Task CreateLeaveType_ReturnsCreated()
        {
            var dto = new CreateLeaveTypeDto
            {
                Name = $"Test Leave {Guid.NewGuid().ToString("N")[..8]}",
                DefaultDays = 10
            };


            var response =
                await _client.PostAsJsonAsync(
                    "/api/LeaveTypes",
                    dto);


            var content =
                await response.Content.ReadAsStringAsync();


            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Status: {response.StatusCode}\nResponse: {content}"
            );


            var leaveType =
                await response.Content.ReadFromJsonAsync<LeaveTypeDto>();


            Assert.NotNull(leaveType);
        }




        [Fact]
        public async Task UpdateLeaveType_ReturnsOk()
        {
            var dto = new UpdateLeaveTypeDto
            {
                Name = "Updated Leave Type",
                DefaultDays = 20
            };


            var response =
                await _client.PutAsJsonAsync(
                    "/api/LeaveTypes/1",
                    dto);


            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }




        [Fact]
        public async Task DeleteLeaveType_ReturnsNoContent()
        {
            // Create first to avoid deleting seeded data

            var createDto = new CreateLeaveTypeDto
            {
                Name = $"Delete Leave {Guid.NewGuid().ToString("N")[..8]}",
                DefaultDays = 5
            };


            var createResponse =
                await _client.PostAsJsonAsync(
                    "/api/LeaveTypes",
                    createDto);


            var leaveType =
                await createResponse.Content
                .ReadFromJsonAsync<LeaveTypeDto>();


            var deleteResponse =
                await _client.DeleteAsync(
                    $"/api/LeaveTypes/{leaveType!.Id}");


            Assert.Equal(
                HttpStatusCode.NoContent,
                deleteResponse.StatusCode);
        }




        [Fact]
        public async Task GetLeaveType_InvalidId_ReturnsNotFound()
        {
            var response =
                await _client.GetAsync(
                    "/api/LeaveTypes/999999");


            Assert.Equal(
                HttpStatusCode.NotFound,
                response.StatusCode);
        }
    }
}
