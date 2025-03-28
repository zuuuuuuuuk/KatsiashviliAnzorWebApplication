using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Dto;

namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromoCodeController : ControllerBase
    {
        private readonly IPromoCodeService _promoCodeService;
        private readonly AppDbContext _context;
        public PromoCodeController(IPromoCodeService promoCodeService, AppDbContext appDbContext)
        {
            _promoCodeService = promoCodeService;
            _context = appDbContext;
        }

        [HttpGet]
        public ActionResult GetAllPromoCodes() 
        {
            var promoCodes = _promoCodeService.GetPromoCodeList().ToList();
            return Ok(promoCodes);
        }

        [HttpGet("{id}")]
        public ActionResult GetPromoCodeById(int id) 
        {
            var promoCode = _promoCodeService.GetPromoCodeById(id);
            return Ok(promoCode);
        }

        [HttpGet("code/{Code}")]
        public IActionResult GetPromoCodeByCode(string code)
        {
            var promoCode = _promoCodeService.getPromoCodeByCode(code);
            return Ok(promoCode);
        }

        [HttpPost]
        public IActionResult AddPromoCode(PromoCodeDto promoCodeDto) 
        {
            if (promoCodeDto == null) 
            {
               return BadRequest("promoCode must not be null");
            }
            _context.Add(promoCodeDto);
            return Ok("promoCode added Successfully");
        }

        [HttpPut("id")]
        public IActionResult UpdatePromoCode(int id, PromoCodeDto promoCodeDto) 
        {
            var existingPromoCode = _promoCodeService.GetPromoCodeById(id);

            if (promoCodeDto == null)
            {
                return BadRequest("promoCode body is null");
            }

            if (existingPromoCode == null)
            {
                return BadRequest("existing promocode is null");
            }
            

            if (!string.IsNullOrWhiteSpace(promoCodeDto.Name))
                existingPromoCode.Name = promoCodeDto.Name;
            if (!string.IsNullOrWhiteSpace(promoCodeDto.Description))
                existingPromoCode.Description = promoCodeDto.Description;
            if (!string.IsNullOrWhiteSpace(promoCodeDto.Code))
                existingPromoCode.Code = promoCodeDto.Code;
            if (promoCodeDto.DiscountValue > 0)
            {
                existingPromoCode.DiscountValue = promoCodeDto.DiscountValue;
            }

            _promoCodeService.UpdatePromoCode(existingPromoCode);
            return Ok($"PromoCode with id {id} has been updated successfully"); 
        }

        [HttpDelete("id")]
        public ActionResult DeletePromoCodeById(int id) 
        {
            _promoCodeService.DeletePromoCode(id);
            return Ok($"promoCode with id {id} has been deleted");
        }
    }
}
