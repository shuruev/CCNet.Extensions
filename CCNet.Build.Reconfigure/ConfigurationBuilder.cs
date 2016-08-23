using System;
using System.Text;
using System.Xml;

namespace CCNet.Build.Reconfigure
{
	public partial class ConfigurationBuilder : IDisposable
	{
		private readonly XmlWriter m_writer;

		public ConfigurationBuilder(string serverName, string filePath)
		{
			m_writer = new XmlTextWriter(filePath, Encoding.UTF8)
			{
				Formatting = Formatting.Indented,
				IndentChar = '\t',
				Indentation = 1
			};

			m_writer.WriteStartDocument();
			m_writer.WriteStartElement("cruisecontrol");
			m_writer.WriteAttributeString("xmlns", "cb", null, "urn:ccnet.config.builder");

			WriteHeader(serverName);
		}

		public void Dispose()
		{
			m_writer.WriteEndElement();
			m_writer.WriteEndDocument();

			m_writer.Dispose();
		}

		private void WriteHeader(string serverName)
		{
			Comment("SERVER NAME");
			using (CbTag("define"))
			{
				Attr("serverName", serverName);
			}

			Comment("IMPORT GLOBAL");
			using (CbTag("include"))
			{
				Attr("href", "Global.config");
			}
		}

		public void Write(IProjectConfiguration config)
		{
			Comment($"PROJECT: {config.UniqueName()}");
			using (Tag("project"))
			{
				WriteProjectHeader(config);
				WriteSourceControl(config);
				WriteTriggers(config);
				WritePrebuild(config);
				WriteTasks(config);
				WritePublishers(config);
			}
		}

		private void WriteProjectHeader(IProjectConfiguration config)
		{
			Tag("name", config.UniqueName());
			Tag("description", config.Description);
			Tag("category", GetCategory(config));
			Tag("queue", GetQueue(config));

			Tag("workingDirectory", config.ProjectDirectory());
			Tag("artifactDirectory", config.ProjectDirectory());

			using (Tag("state"))
			{
				Attr("type", "state");
				Attr("directory", config.ProjectDirectory());
			}

			Tag("webURL", config.WebUrl());

			using (Tag("labeller"))
			{
				Attr("type", "shortDateLabeller");
			}
		}

		private void WriteSourceControl(IProjectConfiguration config)
		{
			using (Tag("sourcecontrol"))
			{
				Attr("type", "multi");
				using (Tag("sourceControls"))
				{
					var tfs = config as ITfsPath;
					if (tfs != null)
					{
						using (Tag("vsts"))
						{
							Tag("executable", "$(tfsExecutable)");
							Tag("server", "$(tfsUrl)");
							Tag("project", tfs.TfsPath);
							Tag("workingDirectory", tfs.SourceDirectory());
							Tag("applyLabel", "false");
							Tag("autoGetSource", "true");
							Tag("cleanCopy", "true");
							Tag("workspace", $"CCNET_{config.Server}_{GetQueue(config)}");
							Tag("deleteWorkspace", "true");
						}
					}

					var references = config as IReferencesDirectory;
					if (references != null)
					{
						using (Tag("filesystem"))
						{
							Tag("repositoryRoot", references.ReferencesDirectory());
							Tag("autoGetSource", "false");
							Tag("ignoreMissingRoot", "true");
						}
					}

					using (Tag("filesystem"))
					{
						Tag("repositoryRoot", config.AdminDirectoryRebuildAll());
						Tag("autoGetSource", "false");
						Tag("ignoreMissingRoot", "true");
					}
				}
			}
		}

		private void WriteTriggers(IProjectConfiguration config)
		{
			using (Tag("triggers"))
			{
				using (Tag("intervalTrigger"))
				{
					Attr("buildCondition", "IfModificationExists");
					Tag("seconds", config.CheckEvery.TotalSeconds.ToString());
					Tag("initialSeconds", "15");
				}

				using (Tag("scheduleTrigger"))
				{
					Attr("buildCondition", "ForceBuild");
					Tag("time", "23:00");
					Tag("randomOffSetInMinutesFromTime", "45");
					using (Tag("weekDays"))
					{
						Tag("weekDay", "Saturday");
					}
				}
			}
		}

		private void WritePrebuild(IProjectConfiguration config)
		{
			using (Tag("prebuild"))
			{
				using (CbTag("DeleteDirectory"))
				{
					Attr("path", config.WorkingDirectory());
				}
			}
		}

		private void WriteTasks(IProjectConfiguration config)
		{
			using (Tag("tasks"))
			{
				WriteCheckProject(config);
				WritePrepareProject(config);
			}
		}

		private void WritePublishers(IProjectConfiguration config)
		{
			using (Tag("publishers"))
			{
				using (Tag("modificationHistory"))
				{
					Attr("onlyLogWhenChangesFound", "true");
				}

				Tag("xmllogger", null);
				Tag("statistics", null);

				using (Tag("artifactcleanup"))
				{
					Attr("cleanUpMethod", "KeepLastXBuilds");
					Attr("cleanUpValue", "100");
				}

				using (Tag("artifactcleanup"))
				{
					Attr("cleanUpMethod", "KeepMaximumXHistoryDataEntries");
					Attr("cleanUpValue", "100");
				}

				if (!String.IsNullOrEmpty(config.OwnerEmail))
				{
					using (CbTag("EmailPublisher"))
					{
						Attr("mailto", config.OwnerEmail);
					}
				}

				using (CbTag("DeleteDirectory"))
				{
					Attr("path", config.WorkingDirectory());
				}
			}
		}
	}
}
