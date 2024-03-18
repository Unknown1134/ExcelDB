namespace ExcelDatabase.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? PhoneNumber {  get; set; }
        public string? Email { get; set; }
        public string? Country { get; set; } 
        public bool? IsActiveNow { get; set; }
        public bool? isNumberValid { get; set; }
    }
}
