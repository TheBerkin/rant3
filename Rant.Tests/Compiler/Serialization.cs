using System;
using System.IO;

using NUnit.Framework;

namespace Rant.Tests.Compiler
{
	[TestFixture]
	public class Serialization
	{
		private readonly RantEngine rant = new RantEngine();

		[TestCase(@"")]
		[TestCase(@"Test")]
		[TestCase(@"\100,c")]
		[TestCase(@"\100,c \100,d")]
		[TestCase(@"{Test}")]
		[TestCase(@"{A|B|C}")]
		[TestCase(@"[r:10]{A|B|C}")]
		[TestCase(@"[r:[n:5;10]]{A|B|C}")]
		[TestCase(@"[r:[n:5;10]]{[repeach][x:_;forward]{A|B|C}}")]
		[TestCase(@"[$[test]:[xpin:_][x:_;forward][after:[xstep:_]]{Hello|World}][$test] [$test]")]
		[TestCase(@"[r:10]{(2)A|([n:2;3])B}")]
		[TestCase(@"[q:This is a quote [q:with a quote] in it.]")]
		[TestCase(@"{A{B|C}|D{E|F}}")]
		[TestCase(@"[`[aeiou]+[wy]?`:The quick brown fox jumps over the lazy dog.;ur]")]
		[TestCase(@"[$[concat:a;b]:[arg:a][arg:b]][$concat:Hello\s;World!]")]
		[TestCase(@"[$[runx2:@a]:[arg:a] [arg:a]][$runx2:[x:_;forward][xpin:_][after:[xstep:_]]{Hello|World!}]")]
		[TestCase(@"<noun.plural(1-3) -a|b|c -d|e|f ? `foo`i ?! `bar` :: !a =b &c>")]
		public void SerializeAndExecute(string pattern)
		{
			var ms = new MemoryStream();
			var pgmSer = RantProgram.CompileString(pattern);
			pgmSer.SaveToStream(ms);
			ms.Seek(0, SeekOrigin.Begin);
			var pgmDeser = RantProgram.LoadStream("Test", ms);
			var resultSer = rant.Do(pgmSer, seed: 0).Main;
			var resultDeser = rant.Do(pgmDeser, seed: 0).Main;
			Console.WriteLine($"Before: '{resultSer}'");
			Console.WriteLine($"After: '{resultDeser}'");
			Assert.AreEqual(resultSer, resultDeser);
		}
	}
}