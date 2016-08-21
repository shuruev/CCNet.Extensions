using System;

namespace NetBuild.CheckProject
{
	public class CheckException : InvalidOperationException
	{
		public CheckException(string message)
			: base(message)
		{
		}

		public CheckException(string format, params object[] args)
			: base(String.Format(format, args))
		{
		}
	}
}
