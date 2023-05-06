namespace Web.Models.Customer
{
    public class AddCustomerRequestModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Birthdate { get; set; }
        public string TCKN { get; set; } = string.Empty;
    }
}
