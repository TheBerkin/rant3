using NUnit.Framework;
using Rant.IO.Bson;

namespace Rant.Tests.Misc
{
    [TestFixture]
    public class Bson
    {
        [Test]
        public void BsonSingleValue()
        {
            var document = new BsonDocument();
            document["test"] = new BsonItem(2);
            var data = document.Write();
            var readDocument = BsonDocument.Read(data);
            Assert.AreEqual(2, readDocument["test"].Value);
        }

        [Test]
        public void BsonArray()
        {
            var document = new BsonDocument();
            document["test"] = new BsonItem(new string[] { "testing", "123" });
            var data = document.Write();
            var readDocument = BsonDocument.Read(data);
            Assert.AreEqual(2, readDocument["test"].KeyCount);
            Assert.AreEqual("testing", readDocument["test"][0].Value);
            Assert.AreEqual("123", readDocument["test"][1].Value);
        }

        [Test]
        public void BsonDeepObject()
        {
            var document = new BsonDocument();
            document["test"] = new BsonItem();
            document["test"]["test_level2"] = new BsonItem();
            document["test"]["test_level2"]["test_level3"] = new BsonItem(true);
            var data = document.Write();
            var readDocument = BsonDocument.Read(data);
            Assert.AreEqual(true, readDocument["test"]["test_level2"]["test_level3"].Value);
        }
    }
}
