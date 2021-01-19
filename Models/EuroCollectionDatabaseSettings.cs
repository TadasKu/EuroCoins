using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EuroCollection.Models
{
    public class EuroCollectionDatabaseSettings : IEuroCollectionDatabaseSettings
    {
        public string CoinCollectionName { get; set; }
        public string UserCollectionName { get; set; }
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Secret { get; set; }

    }
    public interface IEuroCollectionDatabaseSettings
    {
       public string CoinCollectionName { get; set; }
        public string UserCollectionName { get; set; }
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Secret { get; set; }
    }
}
