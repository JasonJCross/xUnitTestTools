using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace xUnitTestTools.Tests
{
	public class EqualityTests
	{
		public ITestOutputHelper TestOutputHelper { get; }

		public EqualityTests(ITestOutputHelper testOutputHelper)
		{
			TestOutputHelper = testOutputHelper;
		}

		public class TestClass : IEquatable<TestClass>
		{
			public TestClass(int i, string s, double d, decimal m, object o)
			{
				Int = i;
				String = s;
				Double = d;
				Decimal = m;
				Object = o;
			}

			public int Int { get; }
			public string String { get; }
			public double Double { get; }
			public decimal Decimal { get; }
			public object Object { get; }

			public bool Equals(TestClass other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return Int == other.Int && String == other.String && Double.Equals(other.Double) && Decimal == other.Decimal && Equals(Object, other.Object);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				return obj.GetType() == this.GetType() && Equals((TestClass)obj);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(Int, String, Double, Decimal, Object);
			}
		}

		public static IEnumerable<object[]> IsEqual_Tests_Data()
		{
			yield return new object[] { 1, 1, true };
			yield return new object[] { "Test", "Test", true };
			yield return new object[] { null, null, true };
			yield return new object[] { 1, 2, false };
			yield return new object[] { 1, 1m, false };
			yield return new object[] { 1, "2", false };
			yield return new object[] { new TestClass(1, "2", 3, 4, 5), new TestClass(1, "2", 3, 4, 5), true };
			yield return new object[] { new TestClass(1, "2", 3, 4, new TestClass(1, "2", 3, 4, 5)), new TestClass(1, "2", 3, 4, new TestClass(1, "2", 3, 4, 5)), true };
			yield return new object[] { new TestClass(1, "23", 3, 4, 5), new TestClass(1, "2", 3, 4, 5), false };
			yield return new object[] { new TestClass(1, "2", 3, 4, new TestClass(1, "23", 3, 4, 5)), new TestClass(1, "2", 3, 4, new TestClass(1, "2", 3, 4, 5)), false };
		}

		[Theory, MemberData(nameof(IsEqual_Tests_Data))]
		public void IsEqual_Tests(object expectedIn, object actualIn, bool expected)
		{
			var equality = new Equality(TestOutputHelper);

			var actual = equality.IsEqual(expectedIn, actualIn);

			Assert.Equal(expected, actual);
		}

		public static IEnumerable<object[]> AreEnumerablesEqual_Tests_Data()
		{
			yield return new object[] { new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5 }, true };
			yield return new object[] { new[] { 1 }, new[] { 1, 2, 3, 4, 5 }, false };
			yield return new object[] { new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2 }, false };
			yield return new object[]
			{
				new []
				{
					new TestClass(1, "2", 3, 4, 5),
					new TestClass(2, "3", 4, 5, 6),
					new TestClass(3, "4", 5, 6, 7),
				},
				new []
				{
					new TestClass(1, "2", 3, 4, 5),
					new TestClass(2, "3", 4, 5, 6),
					new TestClass(3, "4", 5, 6, 7),
				},
				true
			};
			yield return new object[]
			{
				new []
				{
					new TestClass(1, "2", 3, 4, 5),
					new TestClass(2, "3", 4, 5, 6),
					new TestClass(3, "4", 5, 6, 7),
				},
				new []
				{
					new TestClass(1, "2", 3, 4, 5),
					new TestClass(2, "33", 4, 5, 6),
					new TestClass(3, "4", 5, 6, 7),
				},
				false
			};
		}

		[Theory, MemberData(nameof(AreEnumerablesEqual_Tests_Data))]
		public void AreEnumerablesEqual_Tests(IEnumerable expectedIn, IEnumerable actualIn, bool expected)
		{
			var equality = new Equality(TestOutputHelper);

			var actual = equality.AreEnumerablesEqual(expectedIn, actualIn);

			Assert.Equal(expected, actual);
		}
	}
}
