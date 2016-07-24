using NUnit.Framework;

using System.IO;

using Rant.Internals.IO.Bson;

namespace Rant.Tests.Misc
{
    [TestFixture]
    public class Bson
    {
        private void CompareByteArrays(byte[] array1, byte[] array2)
        {
            for (var i = 0;
                i < (array1.Length > array2.Length ? array2.Length : array1.Length);
                i++)
            {
                // we use this instead of data.SequenceEqual because this tells us exactly what byte is wrong
                if (array1[i] != array2[i])
                {
                    // print in hex instead of decimal
                    Assert.AreEqual(array2[i].ToString("X2"), array1[i].ToString("X2"));
                }
            }
        }

        [Test(Description = "Make sure the generated file is exactly identical to a test file.")]
        public void BsonByteAccurate()
        {
            // the example document: { "number": 2 } in bson
            var document = new BsonDocument();
            document["number"] = 2;
            var data = document.ToByteArray();
            File.WriteAllBytes("TestOutput.bson", data); // write the data for debugging purposes
            var exampleData = File.ReadAllBytes("TestFile.bson");
            CompareByteArrays(data, exampleData);
        }
        [Test(Description = "Compares a complex generated file to an example file.")]
        public void BsonComplexByteAccurate()
        {
            var document = new BsonDocument();
            document["number"] = 2;
            document["object"] = new BsonItem();
            document["object"]["array"] = new BsonItem(new BsonItem[]
            {
                "string",
                "another_string",
                2,
                5.2
            });
            document["object"]["float"] = 4.2223;
            document["str"] = "string";
            var data = document.ToByteArray();
            File.WriteAllBytes("ComplexTestOutput.bson", data);
            var exampleData = File.ReadAllBytes("ComplexTestFile.bson");
            CompareByteArrays(data, exampleData);
        }

        [Test]
        public void BsonSingleValue()
        {
            var document = new BsonDocument();
            document["test"] = 2;
            var data = document.ToByteArray();
            var readDocument = BsonDocument.Read(data);
            Assert.AreEqual(2, readDocument["test"].Value);
        }

        [Test]
        public void BsonArrayOfObjects()
        {
            var document = new BsonDocument();
            var item1 = new BsonItem();
            item1["prop"] = true;
            var item2 = new BsonItem();
            item2["pro2"] = false;
            document["test"] = new BsonItem(new BsonItem[] { item1, item2 });
            var data = document.ToByteArray();
            var readDocument = BsonDocument.Read(data);
            Assert.AreEqual(2, readDocument["test"].Count);
            Assert.AreEqual(true, readDocument["test"][0]["prop"].Value);
        }

        [Test]
        public void BsonArray()
        {
            var document = new BsonDocument();
            document["test"] = new BsonItem(new string[] { "testing", "123" });
            var data = document.ToByteArray();
            var readDocument = BsonDocument.Read(data);
            Assert.AreEqual(2, readDocument["test"].Count);
            Assert.AreEqual("testing", readDocument["test"][0].Value);
            Assert.AreEqual("123", readDocument["test"][1].Value);
        }

        [Test]
        public void BsonDeepObject()
        {
            var document = new BsonDocument();
            document["test"] = new BsonItem();
            document["test"]["test_level2"] = new BsonItem();
            document["test"]["test_level2"]["test_level3"] = true;
            var data = document.ToByteArray();
            var readDocument = BsonDocument.Read(data);
            Assert.AreEqual(true, readDocument["test"]["test_level2"]["test_level3"].Value);
        }
    }
}
