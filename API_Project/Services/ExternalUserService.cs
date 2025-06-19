using System.Text.Json;
using Microsoft.Extensions.Options;
using API_Project.Interfaces;
using API_Project.Model;
using API_Project.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace API_Project.Services
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        private readonly string _baseURL;


        public ExternalUserService(HttpClient httpClient, IOptions<UserServiceOptions> options, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _baseURL = options.Value.BaseUrl;
            _cache = cache;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {

                var response = await _httpClient.GetAsync($"{_baseURL}users/{id}");

                if (!response.IsSuccessStatusCode)
                    throw new ExternalApiException($"Failed to fetch user: {response.StatusCode}");


                var content = await response.Content.ReadAsStringAsync();

                using var jsonDoc = JsonDocument.Parse(content);
                if (jsonDoc.RootElement.TryGetProperty("data", out var data))
                {
                    return JsonSerializer.Deserialize<User>(data.GetRawText(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                return null;
            }
            catch (HttpRequestException ex)
            {
                throw new NetworkException("Network error while fetching user", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TimeoutException("The request timed out", ex);
            }
            catch (JsonException ex)
            {
                throw new DeserializationException("Error deserializing user response", ex);
            }
            catch (Exception ex)
            {
                throw new UnexpectedException("Unexpected Exception bro: ", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            string cacheKey = "all-users";
            if (_cache.TryGetValue(cacheKey, out List<User> cachedUsers))
            {
                return cachedUsers; // ✅ Return from cache
            }
            var allUsers = new List<User>();
            int currentPage = 1;
            int totalPages;

            try
            {

                do
                {
                    var response = await _httpClient.GetAsync($"{_baseURL}users?page={currentPage}");
                    if (!response.IsSuccessStatusCode)
                        throw new ExternalApiException($"Failed to fetch user: {response.StatusCode}");

                    var content = await response.Content.ReadAsStringAsync();
                    var userResponse = JsonSerializer.Deserialize<UsersResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (userResponse == null || userResponse.Data == null)
                        break;

                    allUsers.AddRange(userResponse.Data);
                    totalPages = userResponse.Total_Pages;
                    currentPage++;

                } while (currentPage <= totalPages);

                _cache.Set(cacheKey, allUsers, TimeSpan.FromMinutes(5));

                return allUsers;
            }
            catch (HttpRequestException ex)
            {
                throw new NetworkException("Network error while fetching user", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TimeoutException("The request timed out", ex);
            }
            catch (JsonException ex)
            {
                throw new DeserializationException("Error deserializing user response", ex);
            }
            catch (Exception ex)
            {
                throw new UnexpectedException("Unexpected Exception bro: ", ex);
            }
        }
    }
}