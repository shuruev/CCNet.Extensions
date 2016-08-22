using System;

namespace NetBuild.CheckProject
{
	/// <summary>
	/// An exception representing a failed check.
	/// </summary>
	public class CheckException : InvalidOperationException
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CheckException(string message)
			: base(message)
		{
		}
	}
}
