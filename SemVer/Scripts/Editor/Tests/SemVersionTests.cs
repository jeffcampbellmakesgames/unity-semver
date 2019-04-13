/*
MIT License

Copyright (c) 2019 Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace JCMG.SemVer.Editor.Tests
{
	[TestFixture]
	internal class SemVersionTests
	{
		[SetUp]
		public void Setup()
		{
			LogAssert.ignoreFailingMessages = false;
		}

		[Test]
		public void CompareTestWithStrings1()
		{
			Assert.True(SemVersion.Equals("1.0.0", "1"));
		}

		[Test]
		public void CompareTestWithStrings2()
		{
			var v = new SemVersion(1, 0, 0);
			Assert.True(v < "1.1");
		}

		[Test]
		public void CompareTestWithStrings3()
		{
			var v = new SemVersion(1, 2);
			Assert.True(v > "1.0.0");
		}

		[Test]
		public void CreateVersionTest()
		{
			var v = new SemVersion(1, 2, 3, "a", "b");

			Assert.AreEqual(1, v.Major);
			Assert.AreEqual(2, v.Minor);
			Assert.AreEqual(3, v.Patch);
			Assert.AreEqual("a", v.Prerelease);
			Assert.AreEqual("b", v.Build);
		}

		[Test]
		public void CreateVersionTestWithNulls()
		{
			var v = new SemVersion(1, 2, 3, null, null);

			Assert.AreEqual(1, v.Major);
			Assert.AreEqual(2, v.Minor);
			Assert.AreEqual(3, v.Patch);
			Assert.AreEqual("", v.Prerelease);
			Assert.AreEqual("", v.Build);
		}

		[Test]
		public void CreateVersionTestWithSystemVersion1()
		{
			var nonSemanticVersion = new Version(0, 0);
			var v = new SemVersion(nonSemanticVersion);

			Assert.AreEqual(0, v.Major);
			Assert.AreEqual(0, v.Minor);
			Assert.AreEqual(0, v.Patch);
			Assert.AreEqual("", v.Build);
			Assert.AreEqual("", v.Prerelease);
		}

		[Test]
		public void CreateVersionTestWithSystemVersion3()
		{
			var nonSemanticVersion = new Version(1, 2, 0, 3);
			var v = new SemVersion(nonSemanticVersion);

			Assert.AreEqual(1, v.Major);
			Assert.AreEqual(2, v.Minor);
			Assert.AreEqual(3, v.Patch);
			Assert.AreEqual("", v.Build);
			Assert.AreEqual("", v.Prerelease);
		}

		[Test]
		public void CreateVersionTestWithSystemVersion4()
		{
			var nonSemanticVersion = new Version(1, 2, 4, 3);
			var v = new SemVersion(nonSemanticVersion);

			Assert.AreEqual(1, v.Major);
			Assert.AreEqual(2, v.Minor);
			Assert.AreEqual(3, v.Patch);
			Assert.AreEqual("4", v.Build);
			Assert.AreEqual("", v.Prerelease);
		}

		[Test]
		public void ParseTest1()
		{
			var version = SemVersion.Parse("1.2.45-alpha+nightly.23");

			Assert.AreEqual(1, version.Major);
			Assert.AreEqual(2, version.Minor);
			Assert.AreEqual(45, version.Patch);
			Assert.AreEqual("alpha", version.Prerelease);
			Assert.AreEqual("nightly.23", version.Build);
		}

		[Test]
		public void ParseTest2()
		{
			var version = SemVersion.Parse("1");

			Assert.AreEqual(1, version.Major);
			Assert.AreEqual(0, version.Minor);
			Assert.AreEqual(0, version.Patch);
			Assert.AreEqual("", version.Prerelease);
			Assert.AreEqual("", version.Build);
		}

		[Test]
		public void ParseTest3()
		{
			var version = SemVersion.Parse("1.2.45-alpha-beta+nightly.23.43-bla");

			Assert.AreEqual(1, version.Major);
			Assert.AreEqual(2, version.Minor);
			Assert.AreEqual(45, version.Patch);
			Assert.AreEqual("alpha-beta", version.Prerelease);
			Assert.AreEqual("nightly.23.43-bla", version.Build);
		}

		[Test]
		public void ParseTest4()
		{
			var version = SemVersion.Parse("2.0.0+nightly.23.43-bla");

			Assert.AreEqual(2, version.Major);
			Assert.AreEqual(0, version.Minor);
			Assert.AreEqual(0, version.Patch);
			Assert.AreEqual("", version.Prerelease);
			Assert.AreEqual("nightly.23.43-bla", version.Build);
		}

		[Test]
		public void ParseTest5()
		{
			var version = SemVersion.Parse("2.0+nightly.23.43-bla");

			Assert.AreEqual(2, version.Major);
			Assert.AreEqual(0, version.Minor);
			Assert.AreEqual(0, version.Patch);
			Assert.AreEqual("", version.Prerelease);
			Assert.AreEqual("nightly.23.43-bla", version.Build);
		}

		[Test]
		public void ParseTest6()
		{
			var version = SemVersion.Parse("2.1-alpha");

			Assert.AreEqual(2, version.Major);
			Assert.AreEqual(1, version.Minor);
			Assert.AreEqual(0, version.Patch);
			Assert.AreEqual("alpha", version.Prerelease);
			Assert.AreEqual("", version.Build);
		}

		[Test]
		public void ParseTest7()
		{
			Assert.Throws<ArgumentException>(() => SemVersion.Parse("ui-2.1-alpha"));
		}

		[Test]
		public void ParseTestStrict1()
		{
			var version = SemVersion.Parse("1.3.4", true);

			Assert.AreEqual(1, version.Major);
			Assert.AreEqual(3, version.Minor);
			Assert.AreEqual(4, version.Patch);
			Assert.AreEqual("", version.Prerelease);
			Assert.AreEqual("", version.Build);
		}

		[Test]
		public void ParseTestStrict2()
		{
			Assert.Throws<InvalidOperationException>(() => SemVersion.Parse("1", true));
		}

		[Test]
		public void ParseTestStrict3()
		{
			Assert.Throws<InvalidOperationException>(() => SemVersion.Parse("1.3", true));
		}

		[Test]
		public void ParseTestStrict4()
		{
			Assert.Throws<InvalidOperationException>(() => SemVersion.Parse("1.3-alpha", true));
		}

		[Test]
		public void TryParseTest1()
		{
			SemVersion v;
			Assert.True(SemVersion.TryParse("1.2.45-alpha-beta+nightly.23.43-bla", out v));
		}

		[Test]
		public void TryParseTest2()
		{
			LogAssert.ignoreFailingMessages = true;

			SemVersion v;
			Assert.False(SemVersion.TryParse("ui-2.1-alpha", out v));
		}

		[Test]
		public void TryParseTest3()
		{
			LogAssert.ignoreFailingMessages = true;

			SemVersion v;
			Assert.False(SemVersion.TryParse("", out v));
		}

		[Test]
		public void TryParseTest4()
		{
			LogAssert.ignoreFailingMessages = true;

			SemVersion v;
			Assert.False(SemVersion.TryParse(null, out v));
		}

		[Test]
		public void TryParseTest5()
		{
			SemVersion v;
			Assert.True(SemVersion.TryParse("1.2", out v, false));
		}

		[Test]
		public void TryParseTest6()
		{
			LogAssert.ignoreFailingMessages = true;

			SemVersion v;
			Assert.False(SemVersion.TryParse("1.2", out v, true));
		}

		[Test]
		public void ToStringTest()
		{
			var version = new SemVersion(1, 2, 0, "beta", "dev-mha.120");

			Assert.AreEqual("1.2.0-beta+dev-mha.120", version.ToString());
		}

		[Test]
		public void AreEqualTest1()
		{
			var v1 = new SemVersion(1, 2, build: "nightly");
			var v2 = new SemVersion(1, 2, build: "nightly");

			var r = v1.Equals(v2);
			Assert.True(r);
		}

		[Test]
		public void EqualTest2()
		{
			var v1 = new SemVersion(1, 2, prerelease: "alpha", build: "dev");
			var v2 = new SemVersion(1, 2, prerelease: "alpha", build: "dev");

			var r = v1.Equals(v2);
			Assert.True(r);
		}

		[Test]
		public void EqualTest3()
		{
			var v1 = SemVersion.Parse("1.2-nightly+dev");
			var v2 = SemVersion.Parse("1.2.0-nightly");

			var r = v1.Equals(v2);
			Assert.False(r);
		}

		[Test]
		public void EqualTest4()
		{
			var v1 = SemVersion.Parse("1.2-nightly");
			var v2 = SemVersion.Parse("1.2.0-nightly2");

			var r = v1.Equals(v2);
			Assert.False(r);
		}

		[Test]
		public void EqualTest5()
		{
			var v1 = SemVersion.Parse("1.2.1");
			var v2 = SemVersion.Parse("1.2.0");

			var r = v1.Equals(v2);
			Assert.False(r);
		}

		[Test]
		public void EqualTest6()
		{
			var v1 = SemVersion.Parse("1.4.0");
			var v2 = SemVersion.Parse("1.2.0");

			var r = v1.Equals(v2);
			Assert.False(r);
		}

		[Test]
		public void EqualByReferenceTest()
		{
			var v1 = SemVersion.Parse("1.2-nightly");

			var r = v1.Equals(v1);
			Assert.True(r);
		}

		[Test]
		public void CompareTest1()
		{
			var v1 = SemVersion.Parse("1.0.0");
			var v2 = SemVersion.Parse("2.0.0");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareTest2()
		{
			var v1 = SemVersion.Parse("1.0.0-beta+dev.123");
			var v2 = SemVersion.Parse("1-beta+dev.123");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(0, r);
		}

		[Test]
		public void CompareTest3()
		{
			var v1 = SemVersion.Parse("1.0.0-alpha+dev.123");
			var v2 = SemVersion.Parse("1-beta+dev.123");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareTest4()
		{
			var v1 = SemVersion.Parse("1.0.0-alpha");
			var v2 = SemVersion.Parse("1.0.0");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareTest5()
		{
			var v1 = SemVersion.Parse("1.0.0");
			var v2 = SemVersion.Parse("1.0.0-alpha");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(1, r);
		}

		[Test]
		public void CompareTest6()
		{
			var v1 = SemVersion.Parse("1.0.0");
			var v2 = SemVersion.Parse("1.0.1-alpha");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareTest7()
		{
			var v1 = SemVersion.Parse("0.0.1");
			var v2 = SemVersion.Parse("0.0.1+build.12");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareTest8()
		{
			var v1 = SemVersion.Parse("0.0.1+build.13");
			var v2 = SemVersion.Parse("0.0.1+build.12.2");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(1, r);
		}

		[Test]
		public void CompareTest9()
		{
			var v1 = SemVersion.Parse("0.0.1-13");
			var v2 = SemVersion.Parse("0.0.1-b");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareTest10()
		{
			var v1 = SemVersion.Parse("0.0.1+uiui");
			var v2 = SemVersion.Parse("0.0.1+12");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(1, r);
		}

		[Test]
		public void CompareTest11()
		{
			var v1 = SemVersion.Parse("0.0.1+bu");
			var v2 = SemVersion.Parse("0.0.1");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(1, r);
		}

		[Test]
		public void CompareTest12()
		{
			var v1 = SemVersion.Parse("0.1.1+bu");
			var v2 = SemVersion.Parse("0.2.1");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareTest13()
		{
			var v1 = SemVersion.Parse("0.1.1-gamma.12.87");
			var v2 = SemVersion.Parse("0.1.1-gamma.12.88");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareTest14()
		{
			var v1 = SemVersion.Parse("0.1.1-gamma.12.87");
			var v2 = SemVersion.Parse("0.1.1-gamma.12.87.1");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareTest15()
		{
			var v1 = SemVersion.Parse("0.1.1-gamma.12.87.99");
			var v2 = SemVersion.Parse("0.1.1-gamma.12.87.X");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareTest16()
		{
			var v1 = SemVersion.Parse("0.1.1-gamma.12.87");
			var v2 = SemVersion.Parse("0.1.1-gamma.12.87.X");

			var r = v1.CompareTo(v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void CompareNullTest()
		{
			var v1 = SemVersion.Parse("0.0.1+bu");
			var r = v1.CompareTo(null);
			Assert.AreEqual(1, r);
		}

		[Test]
		public void TestHashCode()
		{
			var v1 = SemVersion.Parse("1.0.0-1+b");
			var v2 = SemVersion.Parse("1.0.0-1+c");

			var h1 = v1.GetHashCode();
			var h2 = v2.GetHashCode();

			Assert.AreNotEqual(h1, h2);
		}

		[Test]
		public void TestStringConversion()
		{
			SemVersion v = "1.0.0";
			Assert.AreEqual(1, v.Major);
		}

		[Test]
		public void TestUntypedCompareTo()
		{
			var v1 = new SemVersion(1);
			var c = v1.CompareTo((object)v1);

			Assert.AreEqual(0, c);
		}

		[Test]
		public void StaticEqualsTest1()
		{
			var v1 = new SemVersion(1, 2, 3);
			var v2 = new SemVersion(1, 2, 3);

			var r = SemVersion.Equals(v1, v2);
			Assert.True(r);
		}

		[Test]
		public void StaticEqualsTest2()
		{
			var r = SemVersion.Equals(null, null);
			Assert.True(r);
		}

		[Test]
		public void StaticEqualsTest3()
		{
			var v1 = new SemVersion(1);

			var r = SemVersion.Equals(v1, null);
			Assert.False(r);
		}

		[Test]
		public void StaticCompareTest1()
		{
			var v1 = new SemVersion(1);
			var v2 = new SemVersion(2);

			var r = SemVersion.Compare(v1, v2);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void StaticCompareTest2()
		{
			var v1 = new SemVersion(1);

			var r = SemVersion.Compare(v1, null);
			Assert.AreEqual(1, r);
		}

		[Test]
		public void StaticCompareTest3()
		{
			var v1 = new SemVersion(1);

			var r = SemVersion.Compare(null, v1);
			Assert.AreEqual(-1, r);
		}

		[Test]
		public void StaticCompareTest4()
		{
			var r = SemVersion.Compare(null, null);
			Assert.AreEqual(0, r);
		}

		[Test]
		public void EqualsOperatorTest()
		{
			var v1 = new SemVersion(1);
			var v2 = new SemVersion(1);

			var r = v1 == v2;
			Assert.True(r);
		}

		[Test]
		public void UnequalOperatorTest()
		{
			var v1 = new SemVersion(1);
			var v2 = new SemVersion(2);

			var r = v1 != v2;
			Assert.True(r);
		}

		[Test]
		public void GreaterOperatorTest()
		{
			var v1 = new SemVersion(1);
			var v2 = new SemVersion(2);

			var r = v2 > v1;
			Assert.True(r);
		}

		[Test]
		public void GreaterOperatorTest2()
		{
			var v1 = new SemVersion(1, 0, 0, "alpha");
			var v2 = new SemVersion(1, 0, 0, "rc");

			var r = v2 > v1;
			Assert.True(r);
		}

		[Test]
		public void GreaterOperatorTest3()
		{
			var v1 = new SemVersion(1, 0, 0, "-ci.1");
			var v2 = new SemVersion(1, 0, 0, "alpha");

			var r = v2 > v1;
			Assert.True(r);
		}

		[Test]
		public void GreaterOrEqualOperatorTest1()
		{
			var v1 = new SemVersion(1);
			var v2 = new SemVersion(1);

			var r = v1 >= v2;
			Assert.True(r);
		}

		[Test]
		public void GreaterOrEqualOperatorTest2()
		{
			var v1 = new SemVersion(2);
			var v2 = new SemVersion(1);

			var r = v1 >= v2;
			Assert.True(r);
		}

		[Test]
		public void LessOperatorTest()
		{
			var v1 = new SemVersion(1);
			var v2 = new SemVersion(2);

			var r = v1 < v2;
			Assert.True(r);
		}

		[Test]
		public void LessOperatorTest2()
		{
			var v1 = new SemVersion(1, 0, 0, "alpha");
			var v2 = new SemVersion(1, 0, 0, "rc");

			var r = v1 < v2;
			Assert.True(r);
		}

		[Test]
		public void LessOperatorTest3()
		{
			var v1 = new SemVersion(1, 0, 0, "-ci.1");
			var v2 = new SemVersion(1, 0, 0, "alpha");

			var r = v1 < v2;
			Assert.True(r);
		}

		[Test]
		public void LessOrEqualOperatorTest1()
		{
			var v1 = new SemVersion(1);
			var v2 = new SemVersion(1);

			var r = v1 <= v2;
			Assert.True(r);
		}

		[Test]
		public void LessOrEqualOperatorTest2()
		{
			var v1 = new SemVersion(1);
			var v2 = new SemVersion(2);

			var r = v1 <= v2;
			Assert.True(r);
		}

		[Test]
		public void TestChangeMajor()
		{
			var v1 = new SemVersion(1, 2, 3, "alpha", "dev");
			var v2 = v1.Change(major: 5);

			Assert.AreEqual(5, v2.Major);
			Assert.AreEqual(2, v2.Minor);
			Assert.AreEqual(3, v2.Patch);
			Assert.AreEqual("alpha", v2.Prerelease);
			Assert.AreEqual("dev", v2.Build);
		}

		[Test]
		public void TestChangeMinor()
		{
			var v1 = new SemVersion(1, 2, 3, "alpha", "dev");
			var v2 = v1.Change(minor: 5);

			Assert.AreEqual(1, v2.Major);
			Assert.AreEqual(5, v2.Minor);
			Assert.AreEqual(3, v2.Patch);
			Assert.AreEqual("alpha", v2.Prerelease);
			Assert.AreEqual("dev", v2.Build);
		}

		[Test]
		public void TestChangePatch()
		{
			var v1 = new SemVersion(1, 2, 3, "alpha", "dev");
			var v2 = v1.Change(patch: 5);

			Assert.AreEqual(1, v2.Major);
			Assert.AreEqual(2, v2.Minor);
			Assert.AreEqual(5, v2.Patch);
			Assert.AreEqual("alpha", v2.Prerelease);
			Assert.AreEqual("dev", v2.Build);
		}

		[Test]
		public void TestChangePrerelease()
		{
			var v1 = new SemVersion(1, 2, 3, "alpha", "dev");
			var v2 = v1.Change(prerelease: "beta");

			Assert.AreEqual(1, v2.Major);
			Assert.AreEqual(2, v2.Minor);
			Assert.AreEqual(3, v2.Patch);
			Assert.AreEqual("beta", v2.Prerelease);
			Assert.AreEqual("dev", v2.Build);
		}

		[Test]
		public void TestSerialization()
		{
			var semVer = new SemVersion(1, 2, 3, "alpha", "dev");
			SemVersion semVerSerializedDeserialized;
			using (var ms = new MemoryStream())
			{
				var bf = new BinaryFormatter();
				bf.Serialize(ms, semVer);
				ms.Position = 0;
				semVerSerializedDeserialized = (SemVersion)bf.Deserialize(ms);
			}

			Assert.AreEqual(semVer, semVerSerializedDeserialized);
		}
	}
}
