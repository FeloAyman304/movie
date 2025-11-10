using System.ComponentModel.DataAnnotations;

namespace movie_hospital_1.dataModel
{
    public class LoginVM
    {
        public int Id { get; set; }
        [Required]
        public string UserOREmail { get; set; }=string.Empty;
        [Required,DataType(DataType.Password)]
        public string password { get; set; }=string.Empty; 
        public bool RememberMe { get; set; }
    }
}
