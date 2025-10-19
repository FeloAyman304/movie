using System.ComponentModel.DataAnnotations;

namespace movie_hospital_1.dataModel
{
    public class Actor
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; }

        public ICollection<MovieActor>? MovieActors { get; set; }
    }
}
