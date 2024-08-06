using Microsoft.EntityFrameworkCore;
using MovieApp.Dto;
using MovieApp.Interfaces;
using MovieApp.Models;
using System.Text.RegularExpressions;

namespace MovieApp.DataAccess
{
    public class UserDataAccessLayer : IUser
    {
        readonly MovieDBContext _dbContext;

        public UserDataAccessLayer(MovieDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AuthenticatedUser AuthenticateUser(UserLogin loginCredentials)
        {
            AuthenticatedUser authenticatedUser = new();

            // Validación de correo electrónico
            if (!IsValidEmail(loginCredentials.Username))
            {
                return null;
            }

            var userDetails = _dbContext.UserMasters
                .FirstOrDefault(u =>
                u.Username == loginCredentials.Username &&
                u.Password == loginCredentials.Password);

            if (userDetails != null)
            {
                authenticatedUser = new AuthenticatedUser
                {
                    Username = userDetails.Username,
                    UserId = userDetails.UserId,
                    UserTypeName = userDetails.UserTypeName
                };
            }
            return authenticatedUser;
        }

        public async Task<bool> IsUserExists(int userId)
        {
            UserMaster? user = await _dbContext.UserMasters.FirstOrDefaultAsync(x => x.UserId == userId);

            return user != null;
        }

        public async Task<bool> RegisterUser(UserMaster userData)
        {
            bool isUserNameAvailable = CheckUserNameAvailability(userData.Username);

            // Validación de correo electrónico
            if (!IsValidEmail(userData.Username))
            {
                return false;
            }

            try
            {
                if (isUserNameAvailable)
                {
                    await _dbContext.UserMasters.AddAsync(userData);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckUserNameAvailability(string userName)
        {
            string? user = _dbContext.UserMasters.FirstOrDefault(x => x.Username == userName)?.ToString();

            return user == null;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Usar Regex para validar el formato de correo electrónico
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}
