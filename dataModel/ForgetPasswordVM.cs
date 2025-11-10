using System.ComponentModel.DataAnnotations;

namespace movie_hospital_1.dataModel
{
    public class ForgetPasswordVM
    {
        public int Id { get; set; }
        [Required]
        public string usernameOrEmail { get; set; } = string.Empty;

    }
}
