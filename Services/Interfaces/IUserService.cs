using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface IUserService
    {
        public Task<bool> CreateUser(User user);
        public Task<bool> Login(User user);
        public Task<string> GetUsername(string email);
        public Task<User> GetUserByEmail(string email);
    }
}
