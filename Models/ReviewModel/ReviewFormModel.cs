using Microsoft.AspNetCore.Mvc;

namespace Rozetka.Models.ReviewModel
{
    public class ReviewFormModel
    {
        [FromForm(Name = "review-idproduct")]
        public int IdProduct { get; set; }

        [FromForm(Name = "review-rating")]
        public int Rating { get; set; }

        [FromForm(Name = "review-comment")]
        public String Comment { get; set; } = null!;
    }
}
