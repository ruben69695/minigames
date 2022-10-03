using Autofac.Extras.Moq;

namespace Minigames.Tests
{
    public class TestBase
    {
        protected AutoMock _mockProvider;

        public TestBase()
        {
            _mockProvider = AutoMock.GetLoose();
        }
    }
}