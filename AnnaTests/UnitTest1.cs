using Anna;
using NUnit.Framework;

namespace AnnaTests
{
    public class Tests
    {
        private static IrcBot _bot;

        [SetUp]
        public void Setup()
        {
            _bot = new IrcBot(null);
        }

        [Test]
        public void Test1()
        {
            string asd = "asd";
            Message message = _bot.checkMessage("!" + asd);
            Assert.AreEqual(asd, message.command);
            Assert.Pass("JEST WYNERWISCIE");
        }
    }
}