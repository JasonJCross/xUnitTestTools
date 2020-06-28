using System.Collections;

namespace xUnitTestTools
{
	public interface IEquality
	{
		bool AreEnumerablesEqual(IEnumerable expected, IEnumerable actual);
		bool IsEqual<T>(T expected, T actual);
	}
}