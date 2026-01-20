using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriviaGame.Api.Models;

namespace TriviaGame.Api.Services.Interfaces
{
    public interface ICategoriesService
    {
        Task <IEnumerable<Categories>> GetAllCategoriesAsync();
        Task<Categories?> GetCategoryByIdAsync(int id);
    }
}