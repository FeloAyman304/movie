using System.ComponentModel.DataAnnotations;

namespace movie_hospital_1.dataModel
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Movie>? Movies
        {
            get; set;
        }
    }
}
