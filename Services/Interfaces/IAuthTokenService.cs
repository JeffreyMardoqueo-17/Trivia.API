using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriviaGame.Api.Models;

namespace TriviaGame.Api.Services.Interfaces
{
    public interface IAuthTokenService
    {
        string GenerateToken(int userId, string gmail, bool isActive);
    }
}