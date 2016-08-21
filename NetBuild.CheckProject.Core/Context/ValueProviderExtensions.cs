namespace NetBuild.CheckProject
{
	/// <summary>
	/// Extends checking context methods.
	/// </summary>
	public static class ValueProviderExtensions
	{
		/// <summary>
		/// Gets a single context value using specified provider.
		/// </summary>
		public static TValue Value<TContext, TValue>(this CheckContext context) where TContext : ValueProvider<TValue>, new()
		{
			return context.Of<TContext>().Value;
		}

		/// <summary>
		/// Gets a string context value using specified provider.
		/// </summary>
		public static string Value<TContext>(this CheckContext context) where TContext : ValueProvider<string>, new()
		{
			return Value<TContext, string>(context);
		}
	}
}
