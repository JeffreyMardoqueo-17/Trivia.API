using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriviaGame.Api.Models;

namespace TriviaGame.Api.Services.Interfaces
{

    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}