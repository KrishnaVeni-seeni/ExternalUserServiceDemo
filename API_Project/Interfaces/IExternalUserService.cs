using API_Project.Model;

namespace API_Project.Interfaces

{
    public interface IExternalUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();

    }
}