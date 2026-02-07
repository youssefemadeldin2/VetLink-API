namespace VetLink.Services.Services.ReviewService.Dtos
{
    public class ShowReviewDto
    {
        public int Id { get; set; }
        public byte Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BuyerName { get; set; }
    }
}
