
namespace API_Project.Model
{
    public class UsersResponse
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public int Total_Pages { get; set; }
        public List<User> Data { get; set; }
    }
}