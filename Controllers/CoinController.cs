using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EuroCollection.Models;
using EuroCollection.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EuroCollection.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CoinController : ControllerBase
    {
        private readonly CoinService _coinService;
        public CoinController(CoinService coinService)
        {
            _coinService = coinService;
        }

        // GET: api/Coin
        // [Authorize(Roles = Role.Viewer)]
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<Coin>> Get()
        {
            return _coinService.Get();
        }
        [AllowAnonymous]
        [HttpGet("countries")]
        public ActionResult<List<string>> GetCountries()
        {
            List<Coin> coins = _coinService.Get();
            List<string> allCountries = coins.Select( item => item.Country).ToList();
            List<string> sortedCountries = allCountries.Distinct().ToList();
            sortedCountries.Sort();
            return sortedCountries;
        }
        [AllowAnonymous]
        [HttpGet("coinsByCountry")]
        public ActionResult<List<CollectedCoin>> GetByCountry([FromQuery(Name = "id")] string id, [FromQuery(Name = "country")] string country)
        {       
            return _coinService.GetByCountry(id, country);
        }

        [AllowAnonymous]
        // GET: api/Coin/5
        [HttpGet("{id:length(24)}", Name = "Get")]
        public ActionResult<Coin> Get(string id)
        {
            var coin = _coinService.Get(id);
            var photoName = id + ".jpg";
            var b = System.IO.File.ReadAllBytes("./CoinsPhotos/"+photoName);
            if (coin == null)
            {
                return NotFound();
            }

            return Ok(coin);
        }
        [AllowAnonymous]
        [HttpGet("CoinPhoto")]
        public ActionResult<byte[]> GetCoinPhoto([FromQuery(Name = "id")]string id)
        {
            var filePath = Path.Combine("./CoinsPhotos/" + id + ".jpg");
            if (System.IO.File.Exists(filePath))
            {
                var b = System.IO.File.ReadAllBytes(filePath);
                return File(b, "image/jpeg").FileContents;
            }
            else
            {
                return null;
            }
        }
        // POST: api/Coin
        [Authorize(Roles =Role.Admin)]
        [HttpPost]
        public ActionResult<Coin> Post([FromBody] Coin value)
        {
            Coin createdCoin = _coinService.Create(value);
            if (createdCoin != null)
            {
                return Ok(createdCoin);
            }
            return BadRequest();


        }
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("DeletePhoto")]
        public ActionResult DeletePhoto([FromQuery(Name = "id")]string id)
        {
            var filePath = Path.Combine("./CoinsPhotos/" + id + ".jpg");
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return NoContent();
        }
        [Authorize(Roles = Role.Admin)]
        [HttpPost("AddCoin")]
        public ActionResult AddCoin([FromBody]CoinWithPhoto coin)
            {

                Coin coinToCreate = new Coin();
                coinToCreate.CoinName = coin.CoinName;
                coinToCreate.Denomination = coin.Denomination;
                coinToCreate.Year = coin.Year;
                coinToCreate.Country = coin.Country;
                coinToCreate.Value = coin.Value;
                coinToCreate.Type = coin.Type;
                Coin createdCoin = _coinService.Create(coinToCreate);
            if (createdCoin != null)
            {
                byte[] bytes = Convert.FromBase64String(coin.Photo);
                var filePath = Path.Combine("./CoinsPhotos/" + createdCoin.Id + ".jpg");
                System.IO.File.WriteAllBytes(filePath, bytes);
                return Ok();
            }
            else
            {
                return Ok(new { message = "Coin already exists" });
            }
            
            
        }
        // PUT: api/Coin/5
        [Authorize(Roles = Role.Admin)]
        [HttpPut("{id:length(24)}")]
        public ActionResult<Coin> Put(string id, [FromBody] CoinWithPhoto coin)
        {
            Coin coinToEdit = new Coin();
            coinToEdit.Id = id;
            coinToEdit.CoinName = coin.CoinName;
            coinToEdit.Denomination = coin.Denomination;
            coinToEdit.Year = coin.Year;
            coinToEdit.Country = coin.Country;
            coinToEdit.Value = coin.Value;
            coinToEdit.Type = coin.Type;

            var coinToSearch = _coinService.Get(id);
            if (coinToSearch == null)
            {
                return NotFound();
            }
            _coinService.Update(id, coinToEdit);
            if (coin.Photo!=null)
            {
                byte[] bytes = Convert.FromBase64String(coin.Photo);
                var filePath = Path.Combine("./CoinsPhotos/" + coinToSearch.Id + ".jpg");
                System.IO.File.WriteAllBytes(filePath, bytes);
            }
            
            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id:length(24)}")]
        public ActionResult<Coin> Delete(string id)
        {
            var coin = _coinService.Get(id);
            var filePath = Path.Combine("./CoinsPhotos/" + id + ".jpg");
           
           
            if (coin == null)
            {
                return NotFound();
            }
            else
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _coinService.Remove(coin);
                return NoContent();
            }
            


        }
    }
}
