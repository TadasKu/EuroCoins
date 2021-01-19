
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EuroCollection.Models
{
    public class CoinWithType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string CoinName { get; set; }

        public int Denomination { get; set; }

        public int Year { get; set; }

        public string Country { get; set; }
        public double Value { get; set; }
        public string Type { get; set; }
    }
}
