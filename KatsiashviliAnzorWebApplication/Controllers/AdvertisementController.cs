using KatsiashviliAnzorWebApplication.Migrations;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;

        public AdvertisementController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        [HttpGet]
        public IActionResult GetAllAds()
        {
            var ads = _advertisementService.GetAllAdvertisements();
           if (ads == null)
            {
                return BadRequest("ads are null");
            }
            return Ok(ads);
        }

        [HttpGet("id")]
        public IActionResult GetAdById(int id)
        {
            var ad = _advertisementService.GetAdvertisementById(id);
            if (ad == null)
            {
                return BadRequest("ad with that id not found");
            }
            return Ok(ad);
        }

        [HttpPost] 
        public IActionResult AddAdvertisement(AdvertisementDto advertisementDto)
        {
            var adToAdd = new Advertisement()
            {
                Title = advertisementDto.Title,
                ImageUrl = advertisementDto.ImageUrl,
                RedirectUrl = advertisementDto.RedirectUrl,
            };
            _advertisementService.CreateAdvertisement(adToAdd);
            return Ok("Advertisement created successfully");
        }

        [HttpPut("id")]
        public IActionResult UpdateAdvertisement(int id, AdvertisementDto advertisementDto)
        {
            var adToChange = _advertisementService.GetAdvertisementById(id);


            if (advertisementDto == null)
            {
                return BadRequest("advertisement must not be null");
            }
            
            if(!string.IsNullOrEmpty(advertisementDto.Title) && advertisementDto.Title != "string")
                adToChange.Title = advertisementDto.Title;
            if(!string.IsNullOrEmpty(advertisementDto.ImageUrl) && advertisementDto.ImageUrl != "string")
                adToChange.ImageUrl = advertisementDto.ImageUrl;
            if(!string.IsNullOrEmpty(advertisementDto.RedirectUrl) && advertisementDto.RedirectUrl != "string")
                adToChange.RedirectUrl = advertisementDto.RedirectUrl;
         
         

            _advertisementService.UpdateAdvertisement(adToChange);

            return Ok("advertisement updated");
        }


        [HttpDelete("id")]
        public IActionResult DeleteAdvertisement(int id)
        {
            var advertisement = _advertisementService.GetAdvertisementById(id);
            if (advertisement == null)
            {
                return BadRequest("cant find advertisement with that ID");
            }
            _advertisementService.DeleteAdvertisement(id);
            return Ok($"advertisement with id {id} has been deleted");
        }


    }
}
