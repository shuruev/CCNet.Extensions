﻿namespace NetBuild.CheckProject
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			return new App().Run(new Args(args));
		}
	}
}
