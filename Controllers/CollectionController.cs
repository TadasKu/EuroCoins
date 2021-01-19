using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EuroCollection.Models;
using EuroCollection.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EuroCollection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private readonly CollectionService _collectionService;
        public CollectionController(CollectionService collectionService)
        {
            _collectionService = collectionService;
        }
        // GET: api/Collection
        [HttpPost("GetCollections")]
        public ActionResult<List<Collection>> Get([FromBody] string id)
        {
            return _collectionService.Get();
        }
        [HttpPost("AddToCollection")]
        public ActionResult AddToCollection([FromQuery(Name = "coinId")] string coinId, [FromQuery(Name = "userId")] string userId)
        {
                 _collectionService.AddToCollection(coinId, userId);
                return Ok(new { message = "Coin added to collection" });
        }
        [HttpDelete("RemoveFromCollection")]
        public IActionResult RemoveFromCollection([FromQuery(Name = "coinId")] string coinId, [FromQuery(Name = "userId")] string userId)
        {
            _collectionService.RemoveFromCollection(coinId, userId);
            return Ok(new { message = "Coin added to collection" });
        }
        [HttpGet("GetCollectionStatus/{id:length(24)}")]
        public ActionResult<CollectedStruct[]> GetStatus(string id)
        {

             CollectedStruct[] stats = _collectionService.GetStatus(id);
            return stats;
        }
        [HttpGet("GetTotalCoinsCount")]
        public ActionResult<TotalCountStruct[]> GetTotalCoins()
        {

            TotalCountStruct[] stats = _collectionService.GetTotalCoins();
            return stats;
        }
    }


}