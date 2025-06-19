using System.Threading.Tasks;
using API_Project.Services;
using API_Project.Model;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Memory;

class Main_Console
{


    public static async Task Main()
    {
        var options = Options.Create(new UserServiceOptions
        {
            BaseUrl = "https://reqres.in/api/"
        });

        var httpClient = new HttpClient();

        var Cache = new MemoryCache(new MemoryCacheOptions());

        var service = new ExternalUserService(httpClient, options, Cache);

        User a;
        IEnumerable<User> b;

        Console.Write("Functions: 1. Get All Users, 2. Get User by ID, Enter 1 or 2: ");

        int i = Int32.Parse(Console.ReadLine());

        switch (i)
        {
            case 1:
                b = await service.GetAllUsersAsync();
                foreach (var m in b)
                {
                    Console.WriteLine("Email = {0}, Name = {1} {2}",m.Email,m.First_Name,m.Last_Name);
                }
                break;
            case 2:
                Console.Write("Enter User Id: ");
                int n = Int32.Parse(Console.ReadLine());
                a = await service.GetUserByIdAsync(n);
                Console.WriteLine("Id: {3}, Name: {0} {1}, Email: {2}",a.First_Name,a.Last_Name,a.Email,a.Id);
                break;

        }
    }
}
