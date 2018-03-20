using Djambi.Engine;
using NUnit.Framework;
using Shouldly;

namespace Djambi.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void InitializingGameStateShouldSucceed()
        {
            var factory = new GameStateInitializer();
            var playerNames = new[] { "A", "B", "C", "D" };
            var state = factory.InitializeGame(playerNames);
            state.HasValue.ShouldBeTrue();
        }
    }
}
