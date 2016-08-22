using System;
using System.Collections.Generic;
using System.Linq;
using NetBuild.Tfs;

namespace NetBuild.CheckProject.SourceControl.Tfs
{
	public class TfsSourceControl : ISourceControl
	{
		private readonly TfsClient m_tfs;

		public TfsSourceControl(string tfsUrl)
		{
			m_tfs = new TfsClient(tfsUrl);
		}

		public List<string> GetChildItems(string path)
		{
			path = path.TrimEnd('/');

			return m_tfs.GetChildItems(path)
				.Select(item => item.IsFolder
					? item.ServerPath + '/'
					: item.ServerPath)
				.Select(item => item.Replace(path + '/', String.Empty))
				.Where(item => !String.IsNullOrEmpty(item))
				.ToList();
		}
	}
}
