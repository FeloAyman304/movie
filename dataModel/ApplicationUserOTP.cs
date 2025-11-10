namespace movie_hospital_1.dataModel
{
    public class ApplicationUserOTP
    {
        public string Id { get; set; }
     
        public string OTP { get; set; }
        public DateTime createAt { get; set; }
        public DateTime validTo { get; set; }
        public bool isValid { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
}


}
