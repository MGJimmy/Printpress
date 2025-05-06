namespace UserService
{
    public class RegistrationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public AccessToken ConfirmationToken { get; set; } // Added ConfirmationToken
    }
}
