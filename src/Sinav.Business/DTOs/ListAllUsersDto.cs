namespace Sinav.Business.DTOs
{
    public class ListAllUsersDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        public string Staff { get; set; }
        public bool IsApproved { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Union { get; set; }
    }
}