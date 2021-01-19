using EuroCollection.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EuroCollection.Services
{
    public class CollectionService
    {
        private readonly IMongoCollection<Collection> _collections;
        private readonly IMongoCollection<Coin> _coins;

        public CollectionService(IEuroCollectionDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _collections = database.GetCollection<Collection>(settings.CollectionName);
            _coins = database.GetCollection<Coin>(settings.CoinCollectionName);
        }
        public List<Collection> Get() =>
          _collections.Find(collection => true).ToList();

        public Collection Get(string id) =>
            _collections.Find<Collection>(collection => collection.Id == id).FirstOrDefault();

        public void AddToCollection(string coinId, string userId)
        {
            var collection = _collections.Find <Collection>(collection => collection.CollectorId.Equals(userId)).FirstOrDefault();
            if (collection.CoinCollection==null)
            {
                collection.CoinCollection = new List<string>();

            }
            collection.CoinCollection.Add(coinId);
            _collections.ReplaceOne(col => col.CollectorId.Equals(userId), collection);
        }
        public void RemoveFromCollection(string coinId, string userId)
        {
            var collection = _collections.Find<Collection>(collection => collection.CollectorId.Equals(userId)).FirstOrDefault();
            collection.CoinCollection.Remove(coinId);
            _collections.ReplaceOne(col => col.CollectorId.Equals(userId), collection);
        }

        public CollectedStruct[] GetStatus(string userId)
        {
            List<Coin> coins = _coins.Find(coin => true).ToList();
            List<string> allCountries = coins.Select(item => item.Country).ToList();
            List<string> sortedCountries = allCountries.Distinct().ToList();
            sortedCountries.Sort();
            var userCollection = _collections.Find(col => col.CollectorId.Equals(userId)).FirstOrDefault();
            var userCoins = _coins.Find(coin => userCollection.CoinCollection.Contains(coin.Id)).ToList();
            CollectedStruct[] collectedStructs = new CollectedStruct[sortedCountries.Count];
            for (int i = 0; i < sortedCountries.Count; i++)
            {
                collectedStructs[i].CountryName = sortedCountries[i];
                List<Coin> countryCoins = _coins.Find(coin => coin.Country== sortedCountries[i]).ToList();
                collectedStructs[i].TotalCount = countryCoins.Count;
                collectedStructs[i].CollectedCount = userCoins.Where(coin => coin.Country == sortedCountries[i]).ToList().Count;
                collectedStructs[i].CountPercentage = (double)collectedStructs[i].CollectedCount / (double)collectedStructs[i].TotalCount*100;

            }
            return collectedStructs;
        }

        public TotalCountStruct[] GetTotalCoins()
        {
            List<Coin> coins = _coins.Find(coin => true).ToList();
            List<string> allCountries = coins.Select(item => item.Country).ToList();
            List<string> sortedCountries = allCountries.Distinct().ToList();
            sortedCountries.Sort();
            TotalCountStruct[] totalStruct = new TotalCountStruct[sortedCountries.Count];
            for (int i = 0; i < sortedCountries.Count; i++)
            {
                totalStruct[i].CountryName = sortedCountries[i];
                List<Coin> countryCoins = _coins.Find(coin => coin.Country == sortedCountries[i]).ToList();
                totalStruct[i].TotalCount = countryCoins.Count;

            }
            return totalStruct;
        }

    }
}
