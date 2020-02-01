using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EFCosmosNullGetRepro
{
    /// <summary>
    /// Implements an data source helper backed by Cosmos DB
    /// </summary>
    public class CosmosDataHelper
    {
        private readonly IConfiguration _config;
        private static readonly SemaphoreSlim _locker = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Default constructor
        /// </summary>
        public CosmosDataHelper(IConfiguration config)
        {
            _config = config;
            using var context = new CosmosDbContext(_config);
            context.Database.EnsureCreated();
        }

        /// <summary>
        /// Gets a list of complex models based on the searchables properties
        /// </summary>
        /// <param name="searchable1">The 1st searchable</param>
        /// <param name="searchable2">The 2nd searchable</param>
        /// <param name="searchable3">The 3rd searchable</param>
        /// <returns>A list of orders</returns>
        public async Task<IEnumerable<ComplexModel>> GetComplexModelsAsync(string searchable1, string searchable2, string searchable3)
        {
            using var context = new CosmosDbContext(_config);
            return await context.ComplexModels
                                .Where(m => (string.IsNullOrWhiteSpace(searchable1) || m.SubModel1.Searchable1 == searchable1) &&
                                            (string.IsNullOrWhiteSpace(searchable2) || m.SubModel1.Searchable2 == searchable2) &&
                                            (string.IsNullOrWhiteSpace(searchable3) || m.SubModel1.Searchable3 == searchable3))
                                .ToListAsync();
        }

        /// <summary>
        /// Upserts a complex model into Cosmos
        /// </summary>
        /// <param name="model">The model to upsert</param>
        public async Task UpsertComplexModelAsync(ComplexModel model)
        {
            await _locker.WaitAsync();
            using var context = new CosmosDbContext(_config);
            var current = context.ComplexModels.FirstOrDefault(m => m.Id == model.Id);
            if (current != null)
            {
                context.ComplexModels.Remove(current);
            }
            await context.ComplexModels.AddAsync(model);
            await context.SaveChangesAsync();
            _locker.Release();
        }
    }
}