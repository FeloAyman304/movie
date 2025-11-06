using System.ComponentModel.DataAnnotations;

namespace movie_hospital_1.dataModel
{
    public class RegisterVM
    {
        public int Id { get; set; }
        [Required]
        public string firstName { get; set; }=string.Empty;
        [Required]
        public string lastName { get; set; }=string.Empty;
        [Required,EmailAddress]
        public string Email { get; set; }=string.Empty;
        [Required]
        public string userName { get; set; }=string.Empty;
        [Required,DataType(DataType.Password)]
        public string password { get; set; }=string.Empty;
        [Required, DataType(DataType.Password), Compare(nameof(password))]
        public string confirmedPassword { get; set; }=string.Empty;
    }
}
