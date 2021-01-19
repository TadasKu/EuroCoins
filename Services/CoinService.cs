using EuroCollection.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EuroCollection.Services
{
    public class CoinService
    {
        private readonly IMongoCollection<Coin> _coins;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Collection> _collections;

        public CoinService(IEuroCollectionDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            var loggerf = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            ILogger logger = loggerf.CreateLogger<CoinService>();
            logger.LogInformation("CoinService");
            logger.LogInformation(settings.CoinCollectionName);

            _coins = database.GetCollection<Coin>(settings.CoinCollectionName);

            _users = database.GetCollection<User>(settings.UserCollectionName);
            _collections = database.GetCollection<Collection>(settings.CollectionName);
        }

        public List<Coin> Get() =>
            _coins.Find(coin => true).ToList();

        public Coin Get(string id) =>
            _coins.Find<Coin>(coin => coin.Id == id).FirstOrDefault();
        public List<CollectedCoin> GetByCountry(string id, string country)
        {
            List<CollectedCoin> collectionCoins = new List<CollectedCoin>();
            var coins = _coins.Find(coin => coin.Country.ToLower() == country.ToLower()).ToList();
            if (id != "null")
            {
                var selectedUser = _users.Find(user => user.Id.Equals(id)).FirstOrDefault();

                
                var collection = _collections.Find(collection => collection.CollectorId.Equals(selectedUser.Id)).FirstOrDefault();

                
                if (collection.CoinCollection != null)
                {
                    foreach (var coin in coins)
                    {
                        CollectedCoin collectionCoin = new CollectedCoin();
                        if (collection.CoinCollection.Contains(coin.Id))
                        {

                            collectionCoin.Coin = coin;
                            collectionCoin.isCollectedCoinColor = "#4caf50";
                        }
                        else
                        {
                            collectionCoin.Coin = coin;
                            collectionCoin.isCollectedCoinColor = "#f1f1f1";
                        }
                        collectionCoins.Add(collectionCoin);
                    }
                }
                else
                {
                    foreach (var coin in coins)
                    {
                        CollectedCoin collectionCoin = new CollectedCoin();
                        collectionCoin.Coin = coin;
                        collectionCoin.isCollectedCoinColor = "#dddddd";
                        collectionCoins.Add(collectionCoin);
                    }
                }
            }
            else
            {
                foreach (var coin in coins)
                {
                    CollectedCoin collectionCoin = new CollectedCoin();
                    collectionCoin.Coin = coin;
                    collectionCoin.isCollectedCoinColor = "#dddddd";
                    collectionCoins.Add(collectionCoin);
                }
            }
           


            return collectionCoins;
        }

        public Coin Create(Coin coin)
        {
            Coin existingCoin = GetByNameAndCountry(coin);
            if (existingCoin == null)
            {
                _coins.InsertOne(coin);
                return coin;
            }
            else
                return null;
        }
        public Coin GetByNameAndCountry(Coin coinIn) =>
           _coins.Find<Coin>(coin => coin.CoinName == coinIn.CoinName && coin.Country == coinIn.Country && coin.Type == coinIn.Type).FirstOrDefault();
        public void Update(string id, Coin coinIn) =>
            _coins.ReplaceOne(coin => coin.Id == id, coinIn);

        public void Remove(Coin coinIn) {
            _coins.DeleteOne(coin => coin.Id == coinIn.Id);
            var result = _collections.UpdateMany(Builders<Collection>.Filter.AnyEq("CoinCollection",coinIn.Id) , Builders<Collection>.Update.Pull("CoinCollection", coinIn.Id));
        }
            

        public void Remove(string id) =>
            _coins.DeleteOne(coin => coin.Id == id);
        
    }
}
