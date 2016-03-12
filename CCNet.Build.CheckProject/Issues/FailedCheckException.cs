using System;

namespace CCNet.Build.CheckProject
{
	public class FailedCheckException : InvalidOperationException
	{
		public FailedCheckException(string message)
			: base(message)
		{
		}

		public FailedCheckException(string format, params object[] args)
			: base(String.Format(format, args))
		{
		}
	}
}
