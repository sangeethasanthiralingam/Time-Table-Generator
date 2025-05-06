using BCrypt.Net;

namespace Time_Table_Generator.Helpers
{
    public static class PasswordHelper
    {
        // Hash password
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password); // Correct way to call BCrypt
        }

        // Verify password
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword); // Correct way to verify
        }
    }
}