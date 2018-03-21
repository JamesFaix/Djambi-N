using NUnit.Framework;
using Shouldly;
using Djambi.Engine.Services;

namespace Djambi.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void InitializingGameStateShouldSucceed()
        {
            var gameInitializationService = new GameInitializationService();
            var playerNames = new[] { "A", "B", "C", "D" };
            var state = gameInitializationService.InitializeGame(playerNames);
            state.HasValue.ShouldBeTrue();
        }
    }
}
