namespace GymTracker.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string ImagePath { get; set; } // Dodaj to pole
        public string ImageFileName { get; set; } // Dodaj to pole
        // Możesz dodać inne właściwości oferty, które chcesz edytować
    }
}
