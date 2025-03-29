using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Implementation;
using KatsiashviliAnzorWebApplication.Dto;

namespace KatsiashviliAnzorWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        public ReviewController(IReviewService reviewService, IUserService userService, IProductService productService)
        {
            _reviewService = reviewService;
            _userService = userService;
            _productService = productService;
        }


        [HttpPost("{userId}/{productId}")]
        public IActionResult AddReview(int userId, int productId, ReviewDto review)
        {
            if (review == null)
            {
                return BadRequest("Review is null");
            }

            var user = _userService.GetUserById(userId);
            var product = _productService.GetProductById(productId);

            if (product == null)
            {
                return BadRequest("internal product nullRefference error");
            }
            var rev = new Review()
            {
                UserId = userId,
                User = user,
                ProductId = productId,
                Product = product,
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                CreatedAt = DateTime.UtcNow,
            };

            _reviewService.AddReview(rev);

            var newAverageRating = _reviewService.GetAllReviews()
                                        .Where(r => r.ProductId == productId)
                                        .Average(r => r.Rating);

            // Update the product with the new average rating
            product.AverageRating = newAverageRating;
            

            _productService.UpdateProduct(product);

            return Ok("Review added successfully");
        }





        [HttpGet]
        public IActionResult GetAllReviews()
        {
            var reviews = _reviewService.GetAllReviews();
            return Ok(reviews);
        }




        [HttpGet("{id}")]
        public IActionResult GetReviewById(int id)
        {
            var review = _reviewService.GetReviewById(id);
            return Ok(review);
        }



        [HttpGet("product/{productId}")]
        public IActionResult GetReviewsByProductId(int productId)
        {
            var reviews = _reviewService.GetReviewsByProductId(productId);
            return Ok(reviews);
        }



        [HttpGet("user/{userId}")]
        public IActionResult GetReviewsByUserId(int userId)
        {
            var reviews = _reviewService.GetReviewsByUserId(userId);
            return Ok(reviews);
        }



        [HttpPut("{id}")]
        public IActionResult UpdateReview(int id, ReviewDto review)
        { 
            if (review == null)
            {
                return BadRequest("review is null");
            }
            var existingReview = _reviewService.GetReviewById(id);
            if (existingReview == null)
            {
                return BadRequest("review with that id doesn't exist");
            }
            if(!string.IsNullOrEmpty(review.ReviewText) && review.ReviewText != "string")
                existingReview.ReviewText = review.ReviewText;
            if(review.Rating > 0) 
                existingReview.Rating = review.Rating;

            _reviewService.UpdateReview(existingReview);
            return Ok($"Review with id {id} has been updated successfully");
        }



        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            var review = _reviewService.GetReviewById(id);
            if (review == null)
            {
                return BadRequest("can not find review with that id");
            }
            _reviewService.DeleteReview(id);
            return Ok($"Review with id {id} has been deleted successfully");
        }



    }
}
