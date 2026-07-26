using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Project_3.IntegrationTests
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
    }
}
