using EuroCollection.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EuroCollection.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Collection> _collections;
        private readonly string Secret;

        public UserService(IEuroCollectionDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UserCollectionName);
            _collections = database.GetCollection<Collection>(settings.CollectionName);
            Secret = settings.Secret;
        }

        public List<User> Get() =>
            _users.Find(user => true).ToList();

        public User Get(string id) =>
            _users.Find<User>(user => user.Id == id).FirstOrDefault();
        public User GetByUsernameAndEmail(string username, string email) =>
            _users.Find<User>(user => user.UserName == username && user.Email == email).FirstOrDefault();

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public void Update(string id, User userIn) =>
            _users.ReplaceOne(user => user.Id == id, userIn);

        public void Remove(User userIn) =>
            _users.DeleteOne(user => user.Id == userIn.Id);

        public void Remove(string id) {
            _collections.DeleteOne(collections => collections.CollectorId == id);
           _users.DeleteOne(user => user.Id == id);}
           

        public User SignUp(string username, string password, string email)
        {
            var user = _users.Find<User>(usr => usr.Email == email).FirstOrDefault();
            if (user != null)
            {
                return null;
            }
            var userNew = new User();
            userNew.UserName = username;
            userNew.Password = password;
            userNew.Email = email;
            userNew.Role = Role.User;
           
            _users.InsertOne(userNew);
            userNew = GetByUsernameAndEmail(username, email);
            userNew.Password = null;

            Collection col = new Collection();
            col.CollectorId = userNew.Id;
            col.CoinCollection = new List<string>();
            _collections.InsertOne(col);
            
            var collection = _collections.Find<Collection>(collection => collection.CollectorId == userNew.Id).FirstOrDefault();
            userNew = GetByUsernameAndEmail(username, email);
            userNew.CollectionId = collection.Id;
            _users.ReplaceOne(user => user.Id == userNew.Id, userNew);
            return userNew;
        }

        public string Authenticate(string email, string password)     
      {
            //var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);
            var user = _users.Find<User>(usr => usr.Email == email && usr.Password == password).FirstOrDefault();
            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(7),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var writtenToken = tokenHandler.WriteToken(token);
            return writtenToken;
        }
    }
}
