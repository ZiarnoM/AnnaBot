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
            Message message = _bot.checkMessage("BotenAnna","!" + asd);
            Assert.AreEqual(asd, message.command);
            Assert.Pass("JEST WYNERWISCIE");
        }

        [Test]
        public void ReverseShouldReturnReversedString()
        {
            string str = "abc";
            string expected = "cba";
            
            Assert.AreEqual(expected, CommandRunner.Reverse(str));
        }

        [Test]
        public void QuoteShouldReturnStringInQuotes()
        {
            string str = "abc";
            string expected = "\"abc\"";
            
            Assert.AreEqual(expected, CommandRunner.addQuotes(str));
        }

        [Test]
        public void getDestinationFolderNameShouldReturnValidFolderName()
        {
            string str = "-o folderName some other stuff";
            string expected = "folderName";
            
            Assert.AreEqual(expected, CommandRunner.getPublishFolderName(str));
        }
        
        [Test]
        public void getDestinationFolderNameShouldReturnValidFolderName2()
        {
            string str = "--output folderName some other stuff";
            string expected = "folderName";
            
            Assert.AreEqual(expected, CommandRunner.getPublishFolderName(str));
        }
        
        
    }
}