using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCosmosNullGetRepro
{
    class Program
    {
        static void Main(string[] args)
        {
            //Load the config.. Note: Set environment variables: "cosmosauthkey" and "cosmosuri"
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddEnvironmentVariables();
            var config = configBuilder.Build();

            //Setup the Helper
            var helper = new CosmosDataHelper(config);

            //Generate a Model..
            var upsertModel = new ComplexModel
            {
                SubModel1 = new SubModel1
                {
                    Searchable1 = "SearchTerm1",
                    Searchable2 = "SearchTerm2",
                    Searchable3 = "SearchTerm3"
                },
                SubListedModel1s = new List<SubListedModel1>
                {
                    new SubListedModel1
                    {
                        Property1 = "Some Property Value 1",
                        Property2 = "Some Property Value 2",
                        SubModel2 = new SubModel2
                        {
                            Property1 = "Some Sub Property Value 1",
                            Property2 = "Some Sub Property Value 2"
                        },
                        SubModel2_2 = new SubModel2_2
                        {
                            Property1 = "Some Sub Property Value 1",
                            Property2 = "Some Sub Property Value 2"
                        },
                        SubListedModel2s = new List<SubListedModel2>
                        {
                            new SubListedModel2
                            {
                                Property1 = "Some Sub Listed Property 1",
                                Property2 = "Some Sub Listed Property 2"
                            }
                        },
                        SubListedModel2_2s = new List<SubListedModel2_2>
                        {
                            new SubListedModel2_2
                            {
                                Property1 = "Some Sub Listed Property 1",
                                Property2 = "Some Sub Listed Property 2"
                            }
                        }
                    }
                }
            };

            upsertModel.Id = $"{upsertModel.SubModel1.Searchable1}{upsertModel.SubModel1.Searchable2}{upsertModel.SubModel1.Searchable3}";
            upsertModel.PartitionKey = upsertModel.SubModel1.Searchable1;

            Console.WriteLine($"Model Pre-Upsert: upsertModel.SubListedModel1s.First().SubListedModel2s.Count(): {upsertModel.SubListedModel1s.First().SubListedModel2s.Count()}");
            Console.WriteLine($"Model Pre-Upsert: upsertModel.SubListedModel1s.First().SubListedModel2_2s.Count(): {upsertModel.SubListedModel1s.First().SubListedModel2_2s.Count()}\n");


            //Upsert the model
            Console.WriteLine("Upserting the model to Cosmos...\n");
            helper.UpsertComplexModelAsync(upsertModel).Wait();

            Console.WriteLine($"Model Post-Upsert: upsertModel.SubListedModel1s.First().SubListedModel2s.Count(): {upsertModel.SubListedModel1s.First().SubListedModel2s.Count()}");
            Console.WriteLine($"Model Post-Upsert: upsertModel.SubListedModel1s.First().SubListedModel2_2s.Count(): {upsertModel.SubListedModel1s.First().SubListedModel2_2s.Count()}\n");

            Console.WriteLine("Note: If you check the model using the Explorer in Cosmos, it is correct\n");

            //Get the model
            Console.WriteLine("Getting the model from Cosmos...\n");
            var getModel = helper.GetComplexModelsAsync("SearchTerm1", "SearchTerm2", "SearchTerm3").Result.First();

            //Compare the values in the sub listed mmodels
            Console.WriteLine("Compare Lists:");
            Console.WriteLine($"Model from Cosmos: getModel.SubListedModel1s.First().SubListedModel2s.Count(): {getModel.SubListedModel1s.First().SubListedModel2s?.Count().ToString() ?? "null"}");
            Console.WriteLine($"Model from Cosmos: getModel.SubListedModel1s.First().SubListedModel2_2s.Count(): {getModel.SubListedModel1s.First().SubListedModel2_2s?.Count().ToString() ?? "null"}\n");

            Console.WriteLine("Compare Sub models:");
            Console.WriteLine($"Model Post-Upsert: upsertModel.SubListedModel1s.First().SubModel2: {upsertModel.SubListedModel1s.First().SubModel2}");
            Console.WriteLine($"Model from Cosmos: getModel.SubListedModel1s.First().SubModel2: {getModel.SubListedModel1s.First().SubModel2?.ToString() ?? "null"}\n");

            Console.WriteLine($"Model Post-Upsert: upsertModel.SubListedModel1s.First().SubModel2_2: {upsertModel.SubListedModel1s.First().SubModel2_2}");
            Console.WriteLine($"Model from Cosmos: getModel.SubListedModel1s.First().SubModel2_2: {getModel.SubListedModel1s.First().SubModel2_2?.ToString() ?? "null"}");

            Console.ReadKey();
        }
    }
}
