using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCosmosNullGetRepro
{
    /// <summary>
    /// Implements a database context for storing order data in Cosmos
    /// </summary>
    public class CosmosDbContext : DbContext
    {
        /// <summary>
        /// The configuration to use for the Cosmos DB Context
        /// </summary>
        private IConfiguration _config;

        /// <summary>
        /// Initializes the CosmosDBContext using the speficied configuration
        /// </summary>
        /// <param name="config">The configuration to use</param>
        public CosmosDbContext(IConfiguration config)
        {
            _config = config;
        }
        /// <summary>
        /// Gets or sets the master orders
        /// </summary>
        public DbSet<ComplexModel> ComplexModels { get; set; }

        /// <summary>
        /// Configures the context to use Cosmos
        /// </summary>
        /// <param name="optionsBuilder">The builder options</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(_config["cosmosuri"],
                                     _config["cosmosauthkey"],
                                     _config["dbname"]);
        }

        /// <summary>
        /// Creates the models
        /// </summary>
        /// <param name="modelBuilder">The model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Set up regions
            modelBuilder.Entity<ComplexModel>()
                        .ToContainer(nameof(ComplexModels));

            //Set keys
            modelBuilder.Entity<ComplexModel>()
                        .HasKey(o => o.Id);

            //Set up discriminator
            modelBuilder.Entity<ComplexModel>()
                        .HasNoDiscriminator();

            //Set up partition keys
            modelBuilder.Entity<ComplexModel>()
                        .HasPartitionKey(o => o.PartitionKey);
        }
    }
}
