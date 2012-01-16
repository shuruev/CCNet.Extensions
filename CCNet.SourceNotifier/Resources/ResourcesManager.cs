using System;
using System.IO;
using System.Reflection;

namespace ConsoleApplication1.Resources
{
	/// <summary>
	/// Incapsulates functions related to embedded resources.
	/// </summary>
	internal static class ResourcesManager
	{
		/// <summary>
		/// Exception thrown in case resource with the specified name could not be found.
		/// </summary>
		public class ResourceNotFoundException : ApplicationException
		{
			/// <summary>
			/// The name of the resource that could not be found.
			/// </summary>
			public readonly string Name;

			/// <summary>
			/// Constructor.
			/// </summary>
			public ResourceNotFoundException(string name)
				: base(string.Format("Resource {0} could not be found", name))
			{
				Name = name;
			}
		}

		/// <summary>
		/// For usage in XSLT-related classes.
		/// Allows for &lt;xsl:import/&gt; commands.
		/// </summary>
		private class XmlUrlResolver : System.Xml.XmlUrlResolver
		{
			/// <summary>
			/// Maps a URI to an object containing the actual resource.
			/// </summary>
			public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
			{
				return GetResource(Path.GetFileName(absoluteUri.AbsolutePath));
			}
		}

		/// <summary>
		/// Instance of XmlUrlResolver.
		/// </summary>
		public static readonly System.Xml.XmlUrlResolver Resolver = new XmlUrlResolver();

		/// <summary>
		/// Returns the binary stream for given resource name.
		/// </summary>
		private static Stream GetResource(string name)
		{
			var result = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(ResourcesManager), name);
			if (result == null)
			{
				throw new ResourceNotFoundException(name);
			}
			return result;
		}
	}
}
