using CCNet.Common;

namespace CCNet.Coverage2Xml
{
	/// <summary>
	/// Command line properties for current project.
	/// </summary>
	public static class Arguments
	{
		/// <summary>
		/// Gets or sets a default instance.
		/// </summary>
		public static ArgumentProperties Default { get; set; }

		/// <summary>
		/// Gets path to data.coverage file.
		/// </summary>
		public static string DataCoverageFile
		{
			get { return Default.GetValue("DataCoverageFile"); }
		}

		/// <summary>
		/// Gets path to output xml file.
		/// </summary>
		public static string XmlCoverageFile
		{
			get { return Default.GetValue("XmlCoverageFile"); }
		}
	}
}
