using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EuroCollection.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        [Required]
        public string Password { get; set; }
        public string CollectionId { get; set; }
    }
}
