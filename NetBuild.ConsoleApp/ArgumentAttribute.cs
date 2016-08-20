using System;

namespace NetBuild.ConsoleApp
{
	public abstract class ArgumentAttribute : Attribute
	{
		public string Name { get; }
		public string Description { get; }
		public bool Required { get; }

		protected ArgumentAttribute(string name, string description, bool required)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			Name = name;
			Description = description;
			Required = required;
		}
	}
}
