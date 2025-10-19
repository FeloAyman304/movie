using System.ComponentModel.DataAnnotations;

namespace movie_hospital_1.dataModel
{
    public class Cinema
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string? Location { get; set; }

        public string? ImageURL { get; set; }

        public ICollection<Movie>? Movies { get; set; }

    }
}