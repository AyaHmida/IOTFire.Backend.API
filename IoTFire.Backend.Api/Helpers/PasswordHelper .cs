namespace IoTFire.Backend.Api.Helpers
{
    public static class PasswordHelper
    {
        private const int WorkFactor = 12;

 
        public static string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
        }

        
        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}

