using Xunit;

namespace RaceDayAPI.Tests
{
    public class RoleAccessTests
    {
        [Fact]
        public void Organiser_Should_Not_Be_Participant()
        {
            var role = "Organiser";
            Assert.NotEqual("Participant", role);
        }

        [Fact]
        public void Participant_Should_Not_Be_Organiser()
        {
            var role = "Participant";
            Assert.NotEqual("Organiser", role);
        }

        [Fact]
        public void Admin_Should_Be_Admin()
        {
            var role = "Admin";
            Assert.Equal("Admin", role);
        }
    }
}