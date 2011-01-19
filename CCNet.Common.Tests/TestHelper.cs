using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCNet.Common.Tests
{
	/// <summary>
	/// Helper methods for testing.
	/// </summary>
	public static class TestHelper
	{
		/// <summary>
		/// Simple delegate for code being tested.
		/// </summary>
		public delegate void TestCode();

		/// <summary>
		/// Checks whether specified code throws exception.
		/// </summary>
		public static void Throws(
			TestCode action,
			Type exceptionType = null)
		{
			bool throwed = false;

			try
			{
				action();
			}
			catch (Exception exception)
			{
				if (exceptionType == null)
				{
					throwed = true;
				}
				else if (exception.GetType() == exceptionType)
				{
					throwed = true;
				}
				else
				{
					throw;
				}
			}

			if (!throwed)
				throw new AssertFailedException("Expected exception was not thrown.");
		}
	}
}

