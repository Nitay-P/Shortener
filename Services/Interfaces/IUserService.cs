using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface IUserService
    {
        public Task<bool> CreateUser(User user);
        public Task<bool> Login(User user);
        public Task<string> GetUsername(string email);
        public Task<User> GetUserByEmail(string email);
        public Task UpdateUser(string originalUserEmail, string name, string lastName, string username, string email, string password);
		public Task<bool> CheckIfUserExists(User user);
	}
}
