using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Models;

namespace KatsiashviliAnzorWebApplication.Services.Abstraction
{
    public interface IReviewService
    {
        void AddReview(Review review);
        Review GetReviewById(int id);
        List<ReviewDto> GetReviewsByProductId(int productId);
        List<Review> GetAllReviews();
        List<Review> GetReviewsByUserId(int userId);
        void UpdateReview(Review review);
        void DeleteReview(int id);
        
    }
}
