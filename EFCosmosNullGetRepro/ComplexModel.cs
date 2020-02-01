using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EFCosmosNullGetRepro
{
    public class ComplexModel
    {
        public string Id { get; set; }

        public string PartitionKey { get; set; }

        public SubModel1 SubModel1 { get; set; }

        public IEnumerable<SubListedModel1> SubListedModel1s { get; set; }
    }

    [Owned]
    public class SubModel1
    {
        public string Searchable1 { get; set; }

        public string Searchable2 { get; set; }

        public string Searchable3 { get; set; }
    }

    [Owned]
    public class SubModel2
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }

    [Owned]
    public class SubModel2_2
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }

    [Owned]
    public class SubListedModel1
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        
        public SubModel2 SubModel2 { get; set; }

        public SubModel2_2 SubModel2_2 { get; set; }

        public IEnumerable<SubListedModel2> SubListedModel2s { get; set; }

        public IEnumerable<SubListedModel2_2> SubListedModel2_2s { get; set; }
    }

    [Owned]
    public class SubListedModel2
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }

    [Owned]
    public class SubListedModel2_2
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }
}
