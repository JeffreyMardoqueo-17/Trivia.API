using System.Collections.Generic;
using System.Threading.Tasks;
using TriviaGame.Api.Data;
using TriviaGame.Api.Models;
using TriviaGame.Api.Services.Interfaces;

namespace TriviaGame.Api.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly SpExecutor _spExecutor;

        public CategoriesService(SpExecutor spExecutor)
        {
            _spExecutor = spExecutor;
        }

        public async Task<IEnumerable<Categories>> GetAllCategoriesAsync()
        {
            return await _spExecutor.QueryAsync<Categories>("SP_GetCategories");
        }

        public async Task<Categories?> GetCategoryByIdAsync(int id)
        {
            return await _spExecutor.QuerySingleAsync<Categories>(
                "SP_GetCategoryById",
                new { Id = id }
            );
        }
    }
}
