using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using API_Project.Exceptions;
using API_Project.Model;

namespace ExternalUserService.Tests
{
    public class ExternalUserServiceTests
    {
        private API_Project.Services.ExternalUserService CreateService(string responseJson, HttpStatusCode statusCode)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseJson)
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var options = Options.Create(new UserServiceOptions
            {
                BaseUrl = "https://reqres.in/api/"
            });

            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            return new API_Project.Services.ExternalUserService(httpClient, options, memoryCache);
        }

        // ✅ Test 1: Valid user ID returns user
        [Fact]
        public async Task GetUserByIdAsync_ValidId_ReturnsUser()
        {
            // Arrange
            string json = "{\"data\":{\"id\":1,\"first_name\":\"George\",\"last_name\":\"Bluth\",\"email\":\"george@example.com\"}}";
            var service = CreateService(json, HttpStatusCode.OK);

            // Act
            var user = await service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
            Assert.Equal("George", user.First_Name);
        }

        // ❌ Test 2: Invalid ID throws exception
        [Fact]
        public async Task GetUserByIdAsync_InvalidId_ThrowsExternalApiException()
        {
            // Arrange
            var service = CreateService("{}", HttpStatusCode.NotFound);

            // Act & Assert
            await Assert.ThrowsAsync<ExternalApiException>(() => service.GetUserByIdAsync(999));
        }

        // ✅ Test 3: Get all users returns user list
        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsers()
        {
            // Arrange
            string jsonPage1 = @"{
              ""page"": 1,
              ""per_page"": 2,
              ""total"": 2,
              ""total_pages"": 1,
              ""data"": [
                { ""id"": 1, ""first_name"": ""George"", ""last_name"": ""Bluth"", ""email"": ""george@example.com"" },
                { ""id"": 2, ""first_name"": ""Janet"", ""last_name"": ""Weaver"", ""email"": ""janet@example.com"" }
              ]
            }";

            var service = CreateService(jsonPage1, HttpStatusCode.OK);

            // Act
            var users = await service.GetAllUsersAsync();
            var userList = users.ToList();

            // Assert
            Assert.NotNull(users);
            Assert.Equal(2, userList.Count);
            Assert.Equal("George", userList[0].First_Name);
        }

        // ❌ Test 4: API error while getting users throws exception
        [Fact]
        public async Task GetAllUsersAsync_ApiFails_ThrowsException()
        {
            // Arrange
            var service = CreateService("Internal Server Error", HttpStatusCode.InternalServerError);

            // Act & Assert
            await Assert.ThrowsAsync<ExternalApiException>(() => service.GetAllUsersAsync());
        }
    }
}
