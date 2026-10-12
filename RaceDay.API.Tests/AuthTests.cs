using Xunit;

namespace RaceDayAPI.Tests
{
    public class AuthTests
    {
        [Fact]
        public void Password_Should_Be_Hashed_And_Verified()
        {
            var password = "Password123!";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var isValid = BCrypt.Net.BCrypt.Verify(password, hash);
            Assert.True(isValid);
        }

        [Fact]
        public void Wrong_Password_Should_Not_Verify()
        {
            var password = "Password123!";
            var wrongPassword = "WrongPassword";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var isValid = BCrypt.Net.BCrypt.Verify(wrongPassword, hash);
            Assert.False(isValid);
        }
    }
}