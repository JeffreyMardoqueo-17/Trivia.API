using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriviaGame.Api.Models;

namespace TriviaGame.Api.Services.Interfaces
{
    public interface IUserService
    {
        
        Task<(bool Exists, string Message)> CreateUserAsync(string gmail, string passwordHash);
        Task<(bool Success, string Message)> UpdatePasswordAsync(string gmail, string newHash, string newSalt);
        Task<User?> LoginAsync(string gmail, string password);
        Task<(bool Success, string Message)> DeactivateUserAsync(int userId);
    }
}