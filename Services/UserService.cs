using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Interfaces
{
    public class UserService : IUserService
    {
        private readonly UrlContext _urlContext;
        public UserService(UrlContext urlContext)
        {
            _urlContext = urlContext;
            _urlContext.Database.EnsureCreated();
        }

        public async Task<bool> CreateUser(User user)
        {
            if(await CheckIfUserExists(user) == true) 
                return false;

            _urlContext.Users.Add(user);
            _urlContext.SaveChanges();
            return true;
        }
        public async Task<bool> CheckIfUserExists(User user)
        {
            var emailRes = await _urlContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if(emailRes != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Login(User user)
        {
            var userFromEmail = await _urlContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (userFromEmail == null)
                return false;
            if (userFromEmail.Password != user.Password)
                return false;
            return true;
        }
/*        public async Task<string> GetUsername(string email)
        {
           var user = await _urlContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return "";
            return user.Username;
        }*/
        public async Task<User> GetUserByEmail(string email)
        {
            return await _urlContext.Users.Include(l => l.Links).FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task UpdateUser(string originalUserEmail, string name, string lastName, string email, string password)
        {
			var originalUser = await GetUserByEmail(originalUserEmail);
			originalUser.Name = name;
            originalUser.LastName = lastName;
			originalUser.Email = email;
			originalUser.Password = password;
            await _urlContext.SaveChangesAsync();
		}
        //if validation passes, return true
		public async Task<bool> ApiCheckLogin(string email)
        {
			var emailRes = await _urlContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(emailRes == null || email != emailRes.Email)
            {
                return false;
            }

                return true;

		}
	}
}
