using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_hospital_1.dataModel
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }
        public string? ImageURL { get; set; }

        public bool InCinema { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "السعر لازم يكون بين 0 و 1000")]
        public decimal Price { get; set; }  

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public int CinemaId { get; set; }
        [ForeignKey("CinemaId")]
        public Cinema? Cinema { get; set; }

        public ICollection<MovieActor>? MovieActors { get; set; }
    }
}
