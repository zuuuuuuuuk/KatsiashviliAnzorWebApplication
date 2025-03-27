using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;

namespace KatsiashviliAnzorWebApplication.Services.Implementation
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public void AddReview(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public void DeleteReview(int id)
        {
            _context.Remove(GetReviewById(id));
            _context.SaveChanges();
        }

        public List<Review> GetAllReviews()
        {
            return _context.Reviews.ToList();
        }

        public Review GetReviewById(int id)
        {
           return _context.Reviews.FirstOrDefault(r => r.Id == id);
        }

        public List<Review> GetReviewsByProductId(int productId)
        {
            return _context.Reviews.Where(r => r.ProductId == productId).ToList();
        }

        public List<Review> GetReviewsByUserId(int userId)
        {
            return _context.Reviews.Where(r => r.UserId == userId).ToList();
        }

        public void UpdateReview(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
        }
    }
}
