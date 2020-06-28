using System.Collections;
using Xunit.Abstractions;

namespace xUnitTestTools
{
	public class Equality : IEquality
	{
		public ITestOutputHelper TestOutputHelper { get; }

		public Equality(ITestOutputHelper testOutputHelper)
		{
			TestOutputHelper = testOutputHelper;
		}

		public bool AreEnumerablesEqual(IEnumerable expected, IEnumerable actual)
		{
			if (expected is null || actual is null)
			{
				TestOutputHelper.WriteLine($"Null enumerable.");
				return false;
			}

			var expectedEnum = expected.GetEnumerator();
			var actualEnum = actual.GetEnumerator();

			var moveNext = expectedEnum.MoveNext() && actualEnum.MoveNext();

			while (moveNext)
			{
				if (IsEqual(expectedEnum.Current, actualEnum.Current) == false) { return false; }

				if (!expectedEnum.MoveNext())
				{
					moveNext = false;
					if (actualEnum.MoveNext())
					{
						TestOutputHelper.WriteLine($"Actual has more elements.");
						return false;
					}

					continue;
				}

				if (!actualEnum.MoveNext())
				{
					TestOutputHelper.WriteLine($"Expected has more elements.");
					return false;
				}
			}

			return true;
		}

		public bool IsEqual<T>(T expected, T actual)
		{
			if (expected == null || actual == null)
			{
				return expected?.GetHashCode() == actual?.GetHashCode();
			}

			var expectedType = expected.GetType();
			var actualType = actual.GetType();

			if (expectedType != actualType)
			{
				TestOutputHelper.WriteLine($"Type Mismatch: {expectedType.FullName}, {actualType.FullName}");
				return false;
			}
			if (expectedType.IsPrimitive) { return expected.GetHashCode() == actual.GetHashCode(); }
			if (expectedType == typeof(string)) { return expected.GetHashCode() == actual.GetHashCode(); }

			foreach (var property in expectedType.GetProperties())
			{
				var expectedProp = property.GetValue(expected);
				var actualProp = property.GetValue(actual);

				if (expectedProp?.GetHashCode() == actualProp?.GetHashCode())
				{
					continue;
				}

				TestOutputHelper.WriteLine($"{typeof(T)}:{property.Name} Expected [{expectedProp}] Actual [{actualProp}]");

				return false;
			}

			return true;
		}
	}
}
