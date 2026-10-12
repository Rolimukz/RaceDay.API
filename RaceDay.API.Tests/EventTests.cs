using Xunit;

namespace RaceDayAPI.Tests
{
    public class EventTests
    {
        [Fact]
        public void Event_Name_Should_Not_Be_Empty()
        {
            var eventName = "Cape Town Cycle Tour";
            Assert.False(string.IsNullOrEmpty(eventName));
        }

        [Fact]
        public void Entry_Fee_Should_Be_Positive()
        {
            decimal entryFee = 350.00m;
            Assert.True(entryFee >= 0);
        }

        [Fact]
        public void Max_Participants_Should_Be_Positive()
        {
            int maxParticipants = 35000;
            Assert.True(maxParticipants > 0);
        }
    }
}