using KatsiashviliAnzorWebApplication.Data;
using KatsiashviliAnzorWebApplication.Dto;
using KatsiashviliAnzorWebApplication.Models;
using KatsiashviliAnzorWebApplication.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

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


        public List<ReviewDto> GetReviewsByProductId(int productId)
        {
            return _context.Reviews
                .Include(r => r.User) // 👈 Force loading of the User object
                .Where(r => r.ProductId == productId)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    CreatedAt = r.CreatedAt,
                    User = new ReviewUserDto
                    {
                        Id = r.User.Id,
                        FirstName = r.User.FirstName,
                        LastName = r.User.LastName,
                        Role = r.User.Role
                    }
                })
                .ToList();
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
