using System.IO;
using System.Linq;

using NUnit.Framework;

using Rant.Core.IO.Bson;

namespace Rant.Tests.Misc
{
	[TestFixture]
	public class Bson
	{
		private void CompareByteArrays(byte[] array1, byte[] array2)
		{
			for (int i = 0;
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

		[Test]
		public void BsonArray()
		{
			var document = new BsonDocument { ["test"] = new BsonItem(new[] { "testing", "123" }) };
			var data = document.ToByteArray();
			var readDocument = BsonDocument.Read(data);
			Assert.AreEqual(2, readDocument["test"].Count);
			Assert.AreEqual("testing", readDocument["test"][0].Value);
			Assert.AreEqual("123", readDocument["test"][1].Value);
		}

		[Test]
		public void BsonArrayOfObjects()
		{
			var document = new BsonDocument();
			var item1 = new BsonItem { ["prop"] = true };
			var item2 = new BsonItem { ["pro2"] = false };
			document["test"] = new BsonItem(new[] { item1, item2 });
			var data = document.ToByteArray();
			var readDocument = BsonDocument.Read(data);
			Assert.AreEqual(2, readDocument["test"].Count);
			Assert.AreEqual(true, readDocument["test"][0]["prop"].Value);
		}

		[Test(Description = "Make sure the generated file is exactly identical to a test file.")]
		public void BsonByteAccurate()
		{
			// the example document: { "number": 2 } in bson
			var document = new BsonDocument { ["number"] = 2 };
			var data = document.ToByteArray();
			File.WriteAllBytes("TestOutput.bson", data); // write the data for debugging purposes
			var exampleData = File.ReadAllBytes("TestFile.bson");
			// we skip the first byte to account for the string table
			CompareByteArrays(data.Skip(1).ToArray(), exampleData);
		}

		[Test(Description = "Compares a complex generated file to an example file.")]
		public void BsonComplexByteAccurate()
		{
			var document = new BsonDocument
			{
				["number"] = 2,
				["object"] = new BsonItem
				{
					["array"] = new BsonItem(new BsonItem[]
					{
						"string",
						"another_string",
						2,
						5.2
					}),
					["float"] = 4.2223
				},
				["str"] = "string"
			};
			var data = document.ToByteArray();
			File.WriteAllBytes("ComplexTestOutput.bson", data);
			var exampleData = File.ReadAllBytes("ComplexTestFile.bson");
			CompareByteArrays(data.Skip(1).ToArray(), exampleData);
		}

		[Test]
		public void BsonDeepObject()
		{
			var document = new BsonDocument
			{
				["test"] = new BsonItem
				{
					["test_level2"] = new BsonItem
					{
						["test_level3"] = true
					}
				}
			};
			var data = document.ToByteArray();
			var readDocument = BsonDocument.Read(data);
			Assert.AreEqual(true, readDocument["test"]["test_level2"]["test_level3"].Value);
		}

		[Test]
		public void BsonSingleValue()
		{
			var document = new BsonDocument { ["test"] = 2 };
			var data = document.ToByteArray();
			var readDocument = BsonDocument.Read(data);
			Assert.AreEqual(2, readDocument["test"].Value);
		}
	}
}