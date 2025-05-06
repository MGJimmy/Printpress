namespace UserService
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public AccessToken Token { get; set; } // Updated to use AccessToken
    }
}
