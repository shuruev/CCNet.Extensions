using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CCNet.Common;
using CCNet.ProjectChecker.Properties;

namespace CCNet.ProjectChecker
{
	/// <summary>
	/// Checks project during build process.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// Main program.
		/// </summary>
		public static int Main(string[] args)
		{
			/*xxxargs = new[]
			{
				@"ProjectName=GuidedSellingAdmin",
				@"ReferencesDirectory=\\rufrt-vxbuild\d$\CCNET\GuidedSellingAdmin\References",
				@"WorkingDirectorySource=\\rufrt-vxbuild\d$\CCNET\GuidedSellingAdmin\WorkingDirectory\Source",
				@"ExternalReferencesPath=\\rufrt-vxbuild\ExternalReferences",
				@"InternalReferencesPath=\\rufrt-vxbuild\InternalReferences",
				@"ProjectType=WebSite",
				@"AssemblyName=GuidedSellingAdmin",
				@"FriendlyName=UNSPECIFIED",
				@"DownloadZone=UNSPECIFIED",
				@"VisualStudioVersion=2010",
				@"TargetFramework=UNSPECIFIED",
				@"TargetPlatform=AnyCPU",
				@"RootNamespace=GuidedSellingAdmin",
				@"SuppressWarnings=",
				@"AllowUnsafeBlocks=False",
				@"ExpectedVersion=UNSPECIFIED"
			};*/

			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return 0;
			}

			try
			{
				Arguments.Default = ArgumentProperties.Parse(args);
				PerformChecks();
			}
			catch (Exception e)
			{
				return ErrorHandler.Runtime(e);
			}

			return RaiseError.ExitCode;
		}

		/// <summary>
		/// Performs all checks.
		/// </summary>
		private static void PerformChecks()
		{
			CheckWrongProjectFileLocation();
			CheckWrongManifestFileLocation();
			CheckWrongAssemblyInfoFileLocation();
			if (RaiseError.ExitCode > 0)
				return;

			ProjectHelper.LoadProject(Paths.ProjectFile);
			CheckWrongVisualStudioVersion();
			CheckUnknownConfiguration();
			CheckWrongPlatform();
			CheckWrongCommonProperties();
			CheckWrongDebugProperties();
			CheckWrongReleaseProperties();

			CheckWrongManifestContents();
			CheckWrongAssemblyInfoContents();

			CheckWrongReferences();

			CheckWrongFileSet();
			CheckForbiddenFiles();
			CheckWrongConfig();
			CheckWrongDefaultConfig();
		}

		/// <summary>
		/// Displays usage text.
		/// </summary>
		private static void DisplayUsage()
		{
			Console.WriteLine();
			Console.WriteLine(Resources.UsageInfo);
			Console.WriteLine();
		}

		#region Checking file structure

		/// <summary>
		/// Checks "WrongProjectFileLocation" condition.
		/// </summary>
		private static void CheckWrongProjectFileLocation()
		{
			string[] files = Directory.GetFiles(
				Arguments.WorkingDirectorySource,
				String.Format("*.{0}", Paths.ProjectFileExtension),
				SearchOption.AllDirectories);

			if (files.Length == 1)
			{
				if (files[0] == Paths.ProjectFile)
					return;
			}

			RaiseError.WrongProjectFileLocation();
		}

		/// <summary>
		/// Checks "WrongManifestFileLocation" condition.
		/// </summary>
		private static void CheckWrongManifestFileLocation()
		{
			if (Arguments.ProjectType != ProjectType.ClickOnce)
				return;

			string[] files = Directory.GetFiles(Arguments.WorkingDirectorySource, "App.manifest", SearchOption.AllDirectories);
			if (files.Length == 1)
			{
				if (files[0] == Paths.ManifestFile)
					return;
			}

			RaiseError.WrongManifestFileLocation();
		}

		/// <summary>
		/// Checks "WrongAssemblyInfoFileLocation" condition.
		/// </summary>
		private static void CheckWrongAssemblyInfoFileLocation()
		{
			if (Arguments.ProjectType == ProjectType.Azure)
				return;

			string[] files = Directory.GetFiles(Arguments.WorkingDirectorySource, "AssemblyInfo.cs", SearchOption.AllDirectories);
			if (files.Length == 1)
			{
				if (files[0] == Paths.AssemblyInfoFile)
					return;
			}

			RaiseError.WrongAssemblyInfoFileLocation();
		}

		#endregion

		#region Checking project properties

		/// <summary>
		/// Checks "WrongVisualStudioVersion" condition.
		/// </summary>
		private static void CheckWrongVisualStudioVersion()
		{
			string currentVersion = ProjectHelper.GetVisualStudioVersion();
			if (currentVersion == Arguments.VisualStudioVersion)
				return;

			RaiseError.WrongVisualStudioVersion(currentVersion);
		}

		/// <summary>
		/// Checks "UnknownConfiguration" condition.
		/// </summary>
		private static void CheckUnknownConfiguration()
		{
			List<string> configurations = ProjectHelper.GetUsedConfigurations();
			configurations.Remove("Debug");
			configurations.Remove("Release");

			if (configurations.Count == 0)
				return;

			RaiseError.UnknownConfiguration(configurations[0]);
		}

		/// <summary>
		/// Checks "WrongPlatform" condition.
		/// </summary>
		private static void CheckWrongPlatform()
		{
			List<string> platforms = ProjectHelper.GetUsedPlatforms();

			platforms.Remove(Arguments.TargetPlatform);
			if (platforms.Count == 0)
				return;

			RaiseError.WrongPlatform(platforms[0]);
		}

		/// <summary>
		/// Checks "WrongCommonProperties" condition.
		/// </summary>
		[SuppressMessage(
			"Shuruev.StyleCop.CSharp.StyleCopPlus",
			"SP2100:CodeLineMustNotBeLongerThan",
			Justification = "Allowed here for better maintability.")]
		[SuppressMessage(
			"Shuruev.StyleCop.CSharp.StyleCopPlus",
			"SP2101:MethodMustNotContainMoreLinesThan",
			Justification = "Allowed here for better maintability.")]
		private static void CheckWrongCommonProperties()
		{
			Dictionary<string, string> properties = ProjectHelper.GetCommonProperties();
			Dictionary<string, string> required = new Dictionary<string, string>();
			Dictionary<string, string> allowed = new Dictionary<string, string>();

			switch (Arguments.ProjectType)
			{
				case ProjectType.WebSite:
					allowed.Add("AppConfig", "$(ProjectConfigFileName)");
					break;
			}

			allowed.Add("AppDesignerFolder", "Properties");

			switch (Arguments.ProjectType)
			{
				case ProjectType.ClickOnce:
					required.Add("ApplicationIcon", null);
					break;
				case ProjectType.SystemTool:
					allowed.Add("ApplicationIcon", null);
					break;
				default:
					allowed.Add("ApplicationIcon", String.Empty);
					break;
			}

			required.Add("AssemblyName", Arguments.AssemblyName);
			allowed.Add("AssemblyKeyContainerName", String.Empty);
			allowed.Add("AssemblyOriginatorKeyFile", String.Empty);

			switch (Arguments.ProjectType)
			{
				case ProjectType.Library:
				case ProjectType.WebSite:
				case ProjectType.Test:
					allowed.Add("AutoUnifyAssemblyReferences", null);
					break;
			}

			allowed.Add("CodeContractsAssemblyMode", null);
			allowed.Add("Configuration", null);
			allowed.Add("DefaultClientScript", null);
			allowed.Add("DefaultHTMLPageLayout", null);
			allowed.Add("DefaultTargetSchema", null);
			allowed.Add("DelaySign", "false");
			allowed.Add("FileAlignment", "512");
			allowed.Add("FileUpgradeFlags", null);
			allowed.Add("IISExpressSSLPort", null);
			allowed.Add("IISExpressAnonymousAuthentication", null);
			allowed.Add("IISExpressWindowsAuthentication", null);
			allowed.Add("IISExpressUseClassicPipelineMode", null);
			allowed.Add("GenerateResourceNeverLockTypeAssemblies", "true");
			allowed.Add("MvcBuildViews", null);
			allowed.Add("Nonshipping", null);
			allowed.Add("OldToolsVersion", null);

			switch (Arguments.ProjectType)
			{
				case ProjectType.ClickOnce:
				case ProjectType.SystemTool:
					required.Add("OutputType", "WinExe");
					break;
				case ProjectType.Console:
				case ProjectType.WindowsService:
					required.Add("OutputType", "Exe");
					break;
				case ProjectType.WebService:
				case ProjectType.WebSite:
				case ProjectType.Library:
				case ProjectType.Test:
				case ProjectType.Azure:
					required.Add("OutputType", "Library");
					break;
			}

			allowed.Add("Platform", null);
			allowed.Add("PlatformTarget", Arguments.TargetPlatform);

			if (Arguments.ProjectName == "CC.PresentationEngine")
			{
				allowed.Add("PostBuildEvent", null);
			}
			else
			{
				allowed.Add("PostBuildEvent", String.Empty);
			}

			allowed.Add("PreBuildEvent", String.Empty);
			allowed.Add("ProductVersion", null);
			allowed.Add("ProjectGuid", null);
			allowed.Add("ProjectType", "Local");
			allowed.Add("ProjectTypeGuids", null);
			allowed.Add("PublishWizardCompleted", null);
			allowed.Add("RoleType", null);
			required.Add("RootNamespace", Arguments.RootNamespace);
			allowed.Add("RunPostBuildEvent", "OnBuildSuccess");
			required.Add("SccAuxPath", "SAK");
			required.Add("SccLocalPath", "SAK");
			required.Add("SccProjectName", "SAK");
			required.Add("SccProvider", "SAK");
			allowed.Add("SchemaVersion", null);
			allowed.Add("SignAssembly", "false");

			switch (Arguments.ProjectType)
			{
				case ProjectType.ClickOnce:
					required.Add("ApplicationManifest", @"Properties\App.manifest");
					required.Add("ApplicationRevision", "0");
					required.Add("ApplicationVersion", "1.0.0.0");
					required.Add("BootstrapperEnabled", "false");
					required.Add("GenerateManifests", "false");
					required.Add("Install", "true");
					required.Add("InstallFrom", "Web");
					required.Add("InstallUrl", "http://download.cnetcontentsolutions.com/{0}/{1}/".Display(Arguments.DownloadZone, Arguments.AssemblyName));
					required.Add("IsWebBootstrapper", "true");
					required.Add("ManifestCertificateThumbprint", "909985E98496E68FB99801301BDF4D03F6670FDF");
					required.Add("ManifestKeyFile", "channel-codesigncrt.p12");
					required.Add("ManifestTimestampUrl", "http://timestamp.comodoca.com/authenticode");
					required.Add("MapFileExtensions", "true");
					required.Add("MinimumRequiredVersion", "1.0.0.0");
					required.Add("ProductName", Arguments.FriendlyName);
					required.Add("PublisherName", "CNET Content Solutions");
					required.Add("PublishUrl", @"D:\publish\{0}\".Display(Arguments.AssemblyName));
					required.Add("SignManifests", "true");
					required.Add("UpdateEnabled", "true");
					allowed.Add("UpdateInterval", null);
					allowed.Add("UpdateIntervalUnits", null);
					required.Add("UpdateMode", "Foreground");
					required.Add("UpdatePeriodically", "false");
					required.Add("UpdateRequired", "true");
					required.Add("UseApplicationTrust", "false");
					break;
				default:
					allowed.Add("ApplicationRevision", "0");
					allowed.Add("ApplicationVersion", "1.0.0.%2a");
					allowed.Add("BootstrapperEnabled", "true");
					allowed.Add("Install", "true");
					allowed.Add("InstallFrom", "Disk");
					allowed.Add("IsWebBootstrapper", "false");
					allowed.Add("PublishUrl", @"publish\");
					allowed.Add("GenerateManifests", "false");
					allowed.Add("MapFileExtensions", "true");
					allowed.Add("SignManifests", "true");
					allowed.Add("UpdateEnabled", "false");
					allowed.Add("UpdateInterval", null);
					allowed.Add("UpdateIntervalUnits", null);
					allowed.Add("UpdateMode", "Foreground");
					allowed.Add("UpdatePeriodically", "false");
					allowed.Add("UpdateRequired", "false");
					allowed.Add("UseApplicationTrust", "false");
					allowed.Add("JSLintSkip", null);
					break;
			}

			if (Arguments.ProjectType == ProjectType.Azure)
			{
				allowed.Add("CloudExtensionsDir", null);
				required.Add("Name", Arguments.ProjectName);
				allowed.Add("StartDevelopmentStorage", null);
			}

			allowed.Add("SilverlightApplicationList", null);
			allowed.Add("StartupObject", null);
			allowed.Add("TargetFrameworkProfile", null);

			switch (Arguments.TargetFramework)
			{
				case TargetFramework.Net20:
					allowed.Add("TargetFrameworkVersion", "v2.0");
					break;
				case TargetFramework.Net35:
					required.Add("TargetFrameworkVersion", "v3.5");
					break;
				case TargetFramework.Net40:
					required.Add("TargetFrameworkVersion", "v4.0");
					break;
				case TargetFramework.Net45:
					required.Add("TargetFrameworkVersion", "v4.5");
					break;
			}

			allowed.Add("TargetZone", null);
			allowed.Add("UpgradeBackupLocation", null);
			allowed.Add("UseIISExpress", null);
			allowed.Add("UseIISExpressByDefault", null);
			allowed.Add("VisualStudioVersion", "10.0");
			allowed.Add("VSToolsPath", null);
			allowed.Add("Win32Resource", String.Empty);
			allowed.Add("WebReference_EnableLegacyEventingModel", null);
			allowed.Add("WebReference_EnableProperties", null);
			allowed.Add("WebReference_EnableSQLTypes", null);
			allowed.Add("MvcProjectUpgradeChecked", null);

			string description;
			if (ValidationHelper.CheckProperties(
				properties,
				required,
				allowed,
				out description))
				return;

			RaiseError.WrongCommonProperties(description);
		}

		/// <summary>
		/// Checks "WrongDebugProperties" condition.
		/// </summary>
		private static void CheckWrongDebugProperties()
		{
			Dictionary<string, string> properties = ProjectHelper.GetDebugProperties();
			Dictionary<string, string> required = new Dictionary<string, string>();
			Dictionary<string, string> allowed = new Dictionary<string, string>();

			if (Arguments.AllowUnsafeBlocks)
			{
				required.Add("AllowUnsafeBlocks", "true");
			}
			else
			{
				allowed.Add("AllowUnsafeBlocks", "false");
			}

			allowed.Add("BaseAddress", "285212672");
			allowed.Add("CheckForOverflowUnderflow", "false");
			allowed.Add("CodeAnalysisModuleSuppressionsFile", null);
			allowed.Add("CodeAnalysisFailOnMissingRules", null);
			allowed.Add("CodeAnalysisIgnoreBuiltInRules", null);
			allowed.Add("CodeAnalysisIgnoreBuiltInRuleSets", null);
			allowed.Add("CodeAnalysisIgnoreGeneratedCode", null);
			allowed.Add("CodeAnalysisLogFile", null);
			allowed.Add("CodeAnalysisRuleDirectories", null);
			allowed.Add("CodeAnalysisRules", null);
			allowed.Add("CodeAnalysisRuleSet", null);
			allowed.Add("CodeAnalysisRuleSetDirectories", null);
			allowed.Add("CodeAnalysisUseTypeNameInSuppression", null);
			allowed.Add("CodeContractsArithmeticObligations", null);
			allowed.Add("CodeContractsAnalysisWarningLevel", null);
			allowed.Add("CodeContractsBaseLineFile", null);
			allowed.Add("CodeContractsBoundsObligations", null);
			allowed.Add("CodeContractsCacheAnalysisResults", null);
			allowed.Add("CodeContractsCustomRewriterAssembly", null);
			allowed.Add("CodeContractsCustomRewriterClass", null);
			allowed.Add("CodeContractsDisjunctiveRequires", null);
			allowed.Add("CodeContractsEmitXMLDocs", null);
			allowed.Add("CodeContractsEnableRuntimeChecking", null);
			allowed.Add("CodeContractsEnumObligations", null);
			allowed.Add("CodeContractsExtraAnalysisOptions", null);
			allowed.Add("CodeContractsExtraRewriteOptions", null);
			allowed.Add("CodeContractsInferEnsures", null);
			allowed.Add("CodeContractsInferObjectInvariants", null);
			allowed.Add("CodeContractsInferRequires", null);
			allowed.Add("CodeContractsLibPaths", null);
			allowed.Add("CodeContractsNonNullObligations", null);
			allowed.Add("CodeContractsRedundantAssumptions", null);
			allowed.Add("CodeContractsReferenceAssembly", null);
			allowed.Add("CodeContractsRunCodeAnalysis", null);
			allowed.Add("CodeContractsRunInBackground", null);
			allowed.Add("CodeContractsRuntimeCallSiteRequires", null);
			allowed.Add("CodeContractsRuntimeCheckingLevel", null);
			allowed.Add("CodeContractsRuntimeOnlyPublicSurface", null);
			allowed.Add("CodeContractsRuntimeSkipQuantifiers", null);
			allowed.Add("CodeContractsSuggestAssumptions", null);
			allowed.Add("CodeContractsSuggestEnsures", null);
			allowed.Add("CodeContractsSuggestObjectInvariants", null);
			allowed.Add("CodeContractsSuggestRequires", null);
			allowed.Add("CodeContractsRuntimeThrowOnFailure", null);
			allowed.Add("CodeContractsShowSquigglies", null);
			allowed.Add("CodeContractsUseBaseLine", null);
			allowed.Add("CodeContractsMissingPublicRequiresAsWarnings", null);
			allowed.Add("CodeContractsSQLServerOption", null);
			allowed.Add("CodeContractsFailBuildOnWarnings", null);
			allowed.Add("ConfigurationOverrideFile", String.Empty);
			required.Add("DebugSymbols", "true");
			required.Add("DebugType", "full");
			required.Add("DefineConstants", "DEBUG;TRACE");
			required.Add("ErrorReport", "prompt");
			allowed.Add("ExcludeGeneratedDebugSymbol", null);
			allowed.Add("FileAlignment", "512");
			allowed.Add("FxCopRules", null);
			allowed.Add("NoStdLib", "false");

			switch (Arguments.ProjectType)
			{
				case ProjectType.Test:
					allowed.Add("NoWarn", null);
					break;
				default:
					if (String.IsNullOrEmpty(Arguments.SuppressWarnings))
					{
						allowed.Add("NoWarn", Arguments.SuppressWarnings);
					}
					else
					{
						required.Add("NoWarn", Arguments.SuppressWarnings);
					}

					break;
			}

			required.Add("Optimize", "false");
			allowed.Add("RunCodeAnalysis", null);

			switch (Arguments.ProjectType)
			{
				case ProjectType.WebService:
				case ProjectType.WebSite:
					required.Add("OutputPath", @"bin\");
					required.Add("DocumentationFile", @"bin\{0}.xml".Display(Arguments.AssemblyName));
					break;
				case ProjectType.Test:
				case ProjectType.Azure:
					required.Add("OutputPath", @"bin\Debug\");
					allowed.Add("DocumentationFile", null);
					break;
				default:
					required.Add("OutputPath", @"bin\Debug\");
					required.Add("DocumentationFile", @"bin\Debug\{0}.xml".Display(Arguments.AssemblyName));
					break;
			}

			allowed.Add("PlatformTarget", Arguments.TargetPlatform);
			allowed.Add("RegisterForComInterop", "false");
			allowed.Add("RemoveIntegerChecks", "false");
			allowed.Add("TreatWarningsAsErrors", "false");
			required.Add("WarningLevel", "4");
			allowed.Add("UseVSHostingProcess", null);

			string description;
			if (ValidationHelper.CheckProperties(
				properties,
				required,
				allowed,
				out description))
				return;

			RaiseError.WrongDebugProperties(description);
		}

		/// <summary>
		/// Checks "WrongReleaseProperties" condition.
		/// </summary>
		private static void CheckWrongReleaseProperties()
		{
			Dictionary<string, string> properties = ProjectHelper.GetReleaseProperties();
			Dictionary<string, string> required = new Dictionary<string, string>();
			Dictionary<string, string> allowed = new Dictionary<string, string>();

			if (Arguments.AllowUnsafeBlocks)
			{
				required.Add("AllowUnsafeBlocks", "true");
			}
			else
			{
				allowed.Add("AllowUnsafeBlocks", "false");
			}

			allowed.Add("BaseAddress", "285212672");
			allowed.Add("CheckForOverflowUnderflow", "false");
			allowed.Add("CodeAnalysisModuleSuppressionsFile", null);
			allowed.Add("CodeAnalysisFailOnMissingRules", null);
			allowed.Add("CodeAnalysisIgnoreBuiltInRules", null);
			allowed.Add("CodeAnalysisIgnoreBuiltInRuleSets", null);
			allowed.Add("CodeAnalysisIgnoreGeneratedCode", null);
			allowed.Add("CodeAnalysisLogFile", null);
			allowed.Add("CodeAnalysisRuleDirectories", null);
			allowed.Add("CodeAnalysisRules", null);
			allowed.Add("CodeAnalysisRuleSet", null);
			allowed.Add("CodeAnalysisRuleSetDirectories", null);
			allowed.Add("CodeAnalysisUseTypeNameInSuppression", null);
			allowed.Add("CodeContractsArithmeticObligations", null);
			allowed.Add("CodeContractsAnalysisWarningLevel", null);
			allowed.Add("CodeContractsBaseLineFile", null);
			allowed.Add("CodeContractsBoundsObligations", null);
			allowed.Add("CodeContractsCacheAnalysisResults", null);
			allowed.Add("CodeContractsCustomRewriterAssembly", null);
			allowed.Add("CodeContractsCustomRewriterClass", null);
			allowed.Add("CodeContractsDisjunctiveRequires", null);
			allowed.Add("CodeContractsEmitXMLDocs", null);
			allowed.Add("CodeContractsEnableRuntimeChecking", null);
			allowed.Add("CodeContractsEnumObligations", null);
			allowed.Add("CodeContractsExtraAnalysisOptions", null);
			allowed.Add("CodeContractsExtraRewriteOptions", null);
			allowed.Add("CodeContractsInferEnsures", null);
			allowed.Add("CodeContractsInferObjectInvariants", null);
			allowed.Add("CodeContractsInferRequires", null);
			allowed.Add("CodeContractsLibPaths", null);
			allowed.Add("CodeContractsNonNullObligations", null);
			allowed.Add("CodeContractsRedundantAssumptions", null);
			allowed.Add("CodeContractsReferenceAssembly", null);
			allowed.Add("CodeContractsRunCodeAnalysis", null);
			allowed.Add("CodeContractsRunInBackground", null);
			allowed.Add("CodeContractsRuntimeCallSiteRequires", null);
			allowed.Add("CodeContractsRuntimeCheckingLevel", null);
			allowed.Add("CodeContractsRuntimeOnlyPublicSurface", null);
			allowed.Add("CodeContractsRuntimeSkipQuantifiers", null);
			allowed.Add("CodeContractsSuggestAssumptions", null);
			allowed.Add("CodeContractsSuggestEnsures", null);
			allowed.Add("CodeContractsSuggestObjectInvariants", null);
			allowed.Add("CodeContractsSuggestRequires", null);
			allowed.Add("CodeContractsRuntimeThrowOnFailure", null);
			allowed.Add("CodeContractsShowSquigglies", null);
			allowed.Add("CodeContractsUseBaseLine", null);
			allowed.Add("CodeContractsMissingPublicRequiresAsWarnings", null);
			allowed.Add("CodeContractsSQLServerOption", null);
			allowed.Add("CodeContractsFailBuildOnWarnings", null);
			allowed.Add("ConfigurationOverrideFile", String.Empty);
			allowed.Add("DebugSymbols", "true");
			required.Add("DebugType", "pdbonly");
			required.Add("DefineConstants", "TRACE");
			required.Add("ErrorReport", "prompt");
			allowed.Add("ExcludeGeneratedDebugSymbol", null);
			allowed.Add("FileAlignment", "512");
			allowed.Add("FxCopRules", null);
			allowed.Add("NoStdLib", "false");

			switch (Arguments.ProjectType)
			{
				case ProjectType.Test:
					allowed.Add("NoWarn", null);
					break;
				default:
					if (String.IsNullOrEmpty(Arguments.SuppressWarnings))
					{
						allowed.Add("NoWarn", Arguments.SuppressWarnings);
					}
					else
					{
						required.Add("NoWarn", Arguments.SuppressWarnings);
					}

					break;
			}

			required.Add("Optimize", "true");

			switch (Arguments.ProjectType)
			{
				case ProjectType.WebService:
				case ProjectType.WebSite:
					required.Add("OutputPath", @"bin\");
					required.Add("DocumentationFile", @"bin\{0}.xml".Display(Arguments.AssemblyName));
					break;
				case ProjectType.Test:
				case ProjectType.Azure:
					required.Add("OutputPath", @"bin\Release\");
					allowed.Add("DocumentationFile", null);
					break;
				default:
					required.Add("OutputPath", @"bin\Release\");
					required.Add("DocumentationFile", @"bin\Release\{0}.xml".Display(Arguments.AssemblyName));
					break;
			}

			allowed.Add("PlatformTarget", Arguments.TargetPlatform);
			allowed.Add("RegisterForComInterop", "false");
			allowed.Add("RemoveIntegerChecks", "false");
			allowed.Add("TreatWarningsAsErrors", "false");
			required.Add("WarningLevel", "4");
			allowed.Add("UseVSHostingProcess", null);

			string description;
			if (ValidationHelper.CheckProperties(
				properties,
				required,
				allowed,
				out description))
				return;

			RaiseError.WrongReleaseProperties(description);
		}

		#endregion

		#region Checking file contents

		/// <summary>
		/// Checks "WrongManifestContents" condition.
		/// </summary>
		[SuppressMessage(
			"Shuruev.StyleCop.CSharp.StyleCopPlus",
			"SP2100:CodeLineMustNotBeLongerThan",
			Justification = "Allowed for here better maintability.")]
		public static void CheckWrongManifestContents()
		{
			if (Arguments.ProjectType != ProjectType.ClickOnce)
				return;

			string xml = File.ReadAllText(Paths.ManifestFile);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			Dictionary<string, string> properties = PropertiesHelper.ParseFromXml(doc);
			Dictionary<string, string> required = new Dictionary<string, string>();
			Dictionary<string, string> allowed = new Dictionary<string, string>();

			required.Add("/asmv1:assembly[@manifestVersion]", "1.0");
			required.Add("/asmv1:assembly/assemblyIdentity[@version]", "1.0.0.0");
			required.Add("/asmv1:assembly/assemblyIdentity[@name]", "{0}.app".Display(Arguments.AssemblyName));
			required.Add("/asmv1:assembly/trustInfo/security/requestedPrivileges/requestedExecutionLevel[@level]", "asInvoker");
			required.Add("/asmv1:assembly/trustInfo/security/requestedPrivileges/requestedExecutionLevel[@uiAccess]", "false");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/defaultAssemblyRequest[@permissionSetReference]", "Custom");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/PermissionSet[@class]", "System.Security.PermissionSet");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/PermissionSet[@version]", "1");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/PermissionSet[@ID]", "Custom");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/PermissionSet[@SameSite]", "site");
			required.Add("/asmv1:assembly/trustInfo/security/applicationRequestMinimum/PermissionSet[@Unrestricted]", "true");
			allowed.Add("/asmv1:assembly/compatibility/application", String.Empty);

			string description;
			if (ValidationHelper.CheckProperties(
				properties,
				required,
				allowed,
				out description))
				return;

			RaiseError.WrongManifestContents(description);
		}

		/// <summary>
		/// Checks "WrongAssemblyInfoContents" condition.
		/// </summary>
		public static void CheckWrongAssemblyInfoContents()
		{
			if (Arguments.ProjectType == ProjectType.Azure)
				return;

			string[] lines = File.ReadAllLines(Paths.AssemblyInfoFile);

			Dictionary<string, string> properties = PropertiesHelper.ParseFromAssemblyInfo(lines);
			if (properties.ContainsKey("AssemblyCopyright"))
			{
				properties["AssemblyCopyright"] = properties["AssemblyCopyright"]
					.Replace(
						"Copyright © CNET Content Solutions 2010",
						"Copyright © CNET Content Solutions 2011")
					.Replace(
						"Copyright © CNET Content Solutions 2011",
						"Copyright © CNET Content Solutions 2012")
					.Replace(
						"Copyright © CNET Content Solutions 2012",
						"Copyright © CNET Content Solutions 2013")
					.Replace(
						"Copyright © CNET Content Solutions 2013",
						"Copyright © CNET Content Solutions 2014");
			}

			Dictionary<string, string> required = new Dictionary<string, string>();
			Dictionary<string, string> allowed = new Dictionary<string, string>();

			switch (Arguments.ProjectType)
			{
				case ProjectType.ClickOnce:
				case ProjectType.SystemTool:
				case ProjectType.WindowsService:
					required.Add("AssemblyTitle", Arguments.FriendlyName);
					required.Add("AssemblyProduct", Arguments.FriendlyName);
					break;
				default:
					required.Add("AssemblyTitle", Arguments.AssemblyName);
					required.Add("AssemblyProduct", Arguments.AssemblyName);
					break;
			}

			required.Add("AssemblyDescription", String.Empty);
			required.Add("AssemblyConfiguration", String.Empty);
			required.Add("AssemblyCompany", "CNET Content Solutions");
			required.Add("AssemblyCopyright", "Copyright © CNET Content Solutions 2014");
			required.Add("AssemblyTrademark", String.Empty);
			required.Add("AssemblyCulture", String.Empty);
			required.Add("AssemblyVersion", Arguments.ExpectedVersion);
			allowed.Add("AssemblyFileVersion", Arguments.ExpectedVersion);

			allowed.Add("NeutralResourcesLanguage", "en");
			allowed.Add("ComVisible", "false");
			allowed.Add("Guid", null);

			string description;
			if (ValidationHelper.CheckProperties(
				properties,
				required,
				allowed,
				out description))
				return;

			RaiseError.WrongAssemblyInfoContents(description);
		}

		#endregion

		#region Checking project references

		/// <summary>
		/// Checks "WrongReferences" condition.
		/// </summary>
		public static void CheckWrongReferences()
		{
			StringBuilder message = new StringBuilder();
			List<string> projects = ProjectHelper.GetProjectReferences();

			if (Arguments.ProjectType != ProjectType.Azure)
			{
				foreach (string project in projects)
				{
					message.AppendLine(
						Strings.DontUseProjectReference
						.Display(project));
				}
			}

			List<Reference> references = ProjectHelper.GetBinaryReferences();
			CheckReferenceProperties(references, message);

			List<ReferenceFile> allExternals = ReferenceFolder.GetAllFiles(Arguments.ExternalReferencesPath);
			List<ReferenceFile> allInternals = ReferenceFolder.GetAllFiles(Arguments.InternalReferencesPath);
			List<ReferenceFile> usedExternals = new List<ReferenceFile>();
			List<ReferenceFile> usedInternals = new List<ReferenceFile>();
			List<Reference> usedGac = new List<Reference>();

			foreach (Reference reference in references)
			{
				ReferenceFile usedExternal = allExternals.Where(file => file.AssemblyName == reference.Name).FirstOrDefault();
				ReferenceFile usedInternal = allInternals.Where(file => file.AssemblyName == reference.Name).FirstOrDefault();

				if (usedExternal != null)
				{
					usedExternals.Add(usedExternal);
				}
				else if (usedInternal != null)
				{
					usedInternals.Add(usedInternal);
				}
				else
				{
					usedGac.Add(reference);
				}
			}

			foreach (string project in projects)
			{
				ReferenceFile usedInternal = allInternals
					.Where(file => file.AssemblyName == Path.GetFileNameWithoutExtension(project))
					.FirstOrDefault();

				if (usedInternal != null)
				{
					usedInternals.Add(usedInternal);
				}
			}

			ReferenceMark.SetupActual(
				ReferenceType.External,
				Arguments.ReferencesDirectory,
				usedExternals.Select(item => item.ProjectName).Distinct());

			ReferenceMark.SetupActual(
				ReferenceType.Internal,
				Arguments.ReferencesDirectory,
				usedInternals.Select(item => item.ProjectName).Distinct());

			List<string> requiredGac = new List<string>();
			List<string> allowedGac = new List<string>();
			allowedGac.Add("envdte");
			allowedGac.Add("envdte80");
			allowedGac.Add("Microsoft.Contracts");
			allowedGac.Add("Microsoft.CSharp");
			allowedGac.Add("Microsoft.JScript");
			allowedGac.Add("Microsoft.mshtml");
			allowedGac.Add("Microsoft.ReportViewer.Common");
			allowedGac.Add("Microsoft.ReportViewer.WebForms");
			allowedGac.Add("Microsoft.Synchronization");
			allowedGac.Add("Microsoft.Synchronization.Data");
			allowedGac.Add("Microsoft.Synchronization.Data.SqlServer");
			allowedGac.Add("Microsoft.VisualBasic");
			allowedGac.Add("Microsoft.VisualBasic.Compatibility.Data");
			allowedGac.Add("Microsoft.VisualStudio.QualityTools.UnitTestFramework");
			allowedGac.Add("PresentationCore");
			allowedGac.Add("PresentationFramework");
			allowedGac.Add("System");
			allowedGac.Add("System.Core");
			allowedGac.Add("System.ComponentModel.DataAnnotations");
			allowedGac.Add("System.Configuration");
			allowedGac.Add("System.configuration");
			allowedGac.Add("System.Configuration.Install");
			allowedGac.Add("System.Data");
			allowedGac.Add("System.Data.DataSetExtensions");
			allowedGac.Add("System.Data.Entity");
			allowedGac.Add("System.Data.Linq");
			allowedGac.Add("System.Data.Services");
			allowedGac.Add("System.Data.Services.Client");
			allowedGac.Add("System.Deployment");
			allowedGac.Add("System.Design");
			allowedGac.Add("System.DirectoryServices");
			allowedGac.Add("System.DirectoryServices.AccountManagement");
			allowedGac.Add("System.Drawing");
			allowedGac.Add("System.EnterpriseServices");
			allowedGac.Add("System.IdentityModel");
			allowedGac.Add("System.IdentityModel.Selectors");
			allowedGac.Add("System.Management");
			allowedGac.Add("System.Messaging");
			allowedGac.Add("System.Runtime.Remoting");
			allowedGac.Add("System.Runtime.Serialization");
			allowedGac.Add("System.Runtime.Serialization.Formatters.Soap");
			allowedGac.Add("System.Security");
			allowedGac.Add("System.ServiceModel");
			allowedGac.Add("System.ServiceModel.Activation");
			allowedGac.Add("System.ServiceModel.Web");
			allowedGac.Add("System.ServiceModel.DomainServices.EntityFramework");
			allowedGac.Add("System.ServiceModel.DomainServices.Hosting");
			allowedGac.Add("System.ServiceModel.DomainServices.Server");
			allowedGac.Add("System.ServiceProcess");
			allowedGac.Add("System.Transactions");
			allowedGac.Add("System.Web");
			allowedGac.Add("System.Web.Abstractions");
			allowedGac.Add("System.Web.ApplicationServices");
			allowedGac.Add("System.Web.DynamicData");
			allowedGac.Add("System.Web.Entity");
			allowedGac.Add("System.Web.Extensions");
			allowedGac.Add("System.Web.Extensions.Design");
			allowedGac.Add("System.Web.Helpers");
			allowedGac.Add("System.Web.Mobile");
			allowedGac.Add("System.Web.Routing");
			allowedGac.Add("System.Web.Services");
			allowedGac.Add("System.Web.WebPages");
			allowedGac.Add("System.Windows.Forms");
			allowedGac.Add("System.Xaml");
			allowedGac.Add("System.XML");
			allowedGac.Add("System.Xml");
			allowedGac.Add("System.Xml.Linq");
			allowedGac.Add("UIAutomationClient");
			allowedGac.Add("UIAutomationClientsideProviders");
			allowedGac.Add("UIAutomationProvider");
			allowedGac.Add("UIAutomationTypes");
			allowedGac.Add("WindowsBase");

			string description;
			if (!ValidationHelper.CheckEntries(
				usedGac.Select(reference => reference.Name).ToList(),
				requiredGac,
				allowedGac,
				out description))
			{
				message.Append(description);
			}

			if (message.Length == 0)
				return;

			RaiseError.WrongReferences(message.ToString());
		}

		/// <summary>
		/// Checks properties that should not be specified directly.
		/// </summary>
		private static void CheckReferenceProperties(IEnumerable<Reference> references, StringBuilder message)
		{
			List<string> exceptionsSpecific = new List<string>();
			exceptionsSpecific.Add("Microsoft.ApplicationServer.Caching.AzureClientHelper");
			exceptionsSpecific.Add("Microsoft.ApplicationServer.Caching.AzureCommon");
			exceptionsSpecific.Add("Microsoft.ApplicationServer.Caching.Client");
			exceptionsSpecific.Add("Microsoft.ApplicationServer.Caching.Core");
			exceptionsSpecific.Add("Microsoft.VisualStudio.QualityTools.UnitTestFramework");
			exceptionsSpecific.Add("Microsoft.Web.DistributedCache");
			exceptionsSpecific.Add("Microsoft.WindowsFabric.Common");
			exceptionsSpecific.Add("Microsoft.WindowsFabric.Data.Common");
			exceptionsSpecific.Add("System.ServiceModel.DomainServices.EntityFramework");
			exceptionsSpecific.Add("System.ServiceModel.DomainServices.Hosting");
			exceptionsSpecific.Add("System.ServiceModel.DomainServices.Server");
			exceptionsSpecific.Add("System.Web.Helpers");
			exceptionsSpecific.Add("System.Web.Mvc");
			exceptionsSpecific.Add("System.Web.Razor");
			exceptionsSpecific.Add("System.Web.WebPages");
			exceptionsSpecific.Add("System.Web.WebPages.Deployment");
			exceptionsSpecific.Add("System.Web.WebPages.Razor");
			
			List<string> exceptionsMayVary = new List<string>();
			exceptionsMayVary.Add("StackExchange.Redis");
			exceptionsMayVary.Add("System.IO");
			exceptionsMayVary.Add("System.Runtime");
			exceptionsMayVary.Add("System.Threading.Tasks");
			exceptionsMayVary.Add("Microsoft.Threading.Tasks");
			exceptionsMayVary.Add("Microsoft.Threading.Tasks.Extensions");
			exceptionsMayVary.Add("Microsoft.Threading.Tasks.Extensions.Desktop");
			exceptionsMayVary.Add("Microsoft.WindowsAzure.Configuration");
			exceptionsMayVary.Add("Microsoft.WindowsAzure.Diagnostics");
			exceptionsMayVary.Add("Microsoft.WindowsAzure.ServiceRuntime");
			exceptionsMayVary.Add("Microsoft.WindowsAzure.Storage");
			exceptionsMayVary.Add("Microsoft.WindowsAzure.StorageClient");

			foreach (Reference reference in references)
			{
				CheckDirectlySpecifiedProperties(reference, message);

				if (exceptionsMayVary.Contains(reference.Name))
				{
					continue;
				}

				if (exceptionsSpecific.Contains(reference.Name))
				{
					if (!reference.IsSpecificVersion)
					{
						message.AppendLine(
							Strings.UseSpecificVersion
							.Display(reference.Name));
					}
				}
				else
				{
					if (reference.IsSpecificVersion)
					{
						message.AppendLine(
							Strings.DontUseSpecificVersion
							.Display(reference.Name));
					}
				}
			}
		}

		/// <summary>
		/// Checks properties that should not be specified directly.
		/// </summary>
		private static void CheckDirectlySpecifiedProperties(Reference reference, StringBuilder message)
		{
			Dictionary<string, bool> allowCopyLocal = new Dictionary<string, bool>();
			allowCopyLocal.Add("Microsoft.Web.Infrastructure", true);
			allowCopyLocal.Add("Microsoft.WindowsAzure.ServiceRuntime", true);
			allowCopyLocal.Add("System.Web.Helpers", true);
			allowCopyLocal.Add("System.Web.Mvc", true);
			allowCopyLocal.Add("System.Web.Razor", true);
			allowCopyLocal.Add("System.Web.WebPages.Deployment", true);
			allowCopyLocal.Add("System.Web.WebPages", true);
			allowCopyLocal.Add("System.Web.WebPages.Razor", true);
			allowCopyLocal.Add("ImageMagick.Net", false);
			allowCopyLocal.Add("Microsoft.ReportViewer.Common", true);
			allowCopyLocal.Add("Microsoft.ReportViewer.WebForms", true);

			if (reference.Aliases != null)
			{
				message.AppendLine(
					Strings.DontSpecifyPropertyDirectly
					.Display(reference.Name, "Aliases"));
			}

			if (reference.Private != null)
			{
				if (allowCopyLocal.ContainsKey(reference.Name))
				{
					if ((allowCopyLocal[reference.Name] && reference.Private == "True")
						|| (!allowCopyLocal[reference.Name] && reference.Private == "False"))
					{
						// this is allowed exception
					}
					else
					{
						message.AppendLine(
							Strings.DontSpecifyPropertyDirectly
							.Display(reference.Name, "Copy Local"));
					}
				}
				else
				{
					message.AppendLine(
						Strings.DontSpecifyPropertyDirectly
						.Display(reference.Name, "Copy Local"));
				}
			}

			if (reference.EmbedInteropTypes != null)
			{
				message.AppendLine(
					Strings.DontSpecifyPropertyDirectly
					.Display(reference.Name, "Embed Interop Types"));
			}
		}

		#endregion

		#region Checking project items

		/// <summary>
		/// Checks "WrongFileSet" condition.
		/// </summary>
		public static void CheckWrongFileSet()
		{
			StringBuilder message = new StringBuilder();

			List<string> items = Directory.GetFiles(Arguments.WorkingDirectorySource, "*", SearchOption.AllDirectories)
				.Where(item => item != Paths.SourceControlProjectMetadataFile)
				.Where(item => Path.GetFileName(item) != "mssccprj.scc")
				.Where(item => Path.GetFileName(item) != "vssver2.scc")
				.Select(item => item.Replace(Arguments.WorkingDirectorySource, String.Empty).TrimStart('\\'))
				.ToList();

			List<string> required = ProjectHelper.GetProjectItems()
				.Select(item => item.FullName
					.Replace("%27", "'"))
				.Union(new[] { Paths.ProjectFile })
				.Select(item => item.Replace(Arguments.WorkingDirectorySource, String.Empty).TrimStart('\\'))
				.ToList();

			string description;
			if (!ValidationHelper.CheckEntries(
				items,
				required,
				new string[] { },
				out description))
			{
				message.Append(description);
			}

			if (message.Length == 0)
				return;

			RaiseError.WrongFileSet(message.ToString());
		}

		/// <summary>
		/// Checks "ForbiddenFiles" condition.
		/// </summary>
		public static void CheckForbiddenFiles()
		{
			StringBuilder message = new StringBuilder();

			List<string> forbidden = new List<string>();
			forbidden.Add("app.config");
			forbidden.Add("ivy.xml");
			forbidden.Add("Local.testsettings");
			forbidden.Add("publish.bat");
			forbidden.Add("publish.cmd");
			forbidden.Add("Restart.bat");
			forbidden.Add("Register.bat");
			forbidden.Add("Start.bat");
			forbidden.Add("Stop.bat");
			forbidden.Add("UnRegister.bat");
			forbidden.Add("web.config");
			forbidden.Add("web.config.default");
			forbidden.Add("Web.Debug.config");
			forbidden.Add("Web.Release.config");
			forbidden.Add("DeploymentInstruction.txt");

			List<string> items = ProjectHelper.GetProjectItems()
				.Select(item => Path.GetFileName(item.FullName))
				.ToList();

			string description;
			if (!ValidationHelper.CheckEntries(
				items,
				forbidden,
				out description))
			{
				message.Append(description);
			}

			if (message.Length == 0)
				return;

			RaiseError.ForbiddenFiles(message.ToString());
		}

		/// <summary>
		/// Checks "WrongConfig" condition.
		/// </summary>
		public static void CheckWrongConfig()
		{
			string configFileName;
			ProjectItemType type;
			CopyToOutputDirectory copyToOutput;

			switch (Arguments.ProjectType)
			{
				case ProjectType.Console:
				case ProjectType.SystemTool:
				case ProjectType.WindowsService:
					configFileName = "App.config";
					type = ProjectItemType.None;
					copyToOutput = CopyToOutputDirectory.None;
					break;
				case ProjectType.WebService:
				case ProjectType.WebSite:
					configFileName = "Web.config";
					type = ProjectItemType.Content;
					copyToOutput = CopyToOutputDirectory.None;
					break;
				case ProjectType.ClickOnce:
				case ProjectType.Library:
				case ProjectType.Test:
				case ProjectType.Azure:
					return;
				default:
					throw new InvalidOperationException(
						String.Format("Unknown project type: {0}.", Arguments.ProjectType));
			}

			CheckConfigProperties(configFileName, type, copyToOutput);
		}

		/// <summary>
		/// Checks "WrongDefaultConfig" condition.
		/// </summary>
		public static void CheckWrongDefaultConfig()
		{
			string configFileName;
			ProjectItemType type;
			CopyToOutputDirectory copyToOutput;

			switch (Arguments.ProjectType)
			{
				case ProjectType.Console:
				case ProjectType.SystemTool:
				case ProjectType.WindowsService:
					configFileName = "{0}.exe.config.default".Display(Arguments.AssemblyName);
					type = ProjectItemType.Content;
					copyToOutput = CopyToOutputDirectory.Always;
					break;
				case ProjectType.WebService:
				case ProjectType.WebSite:
					configFileName = "Web.config.default";
					type = ProjectItemType.Content;
					copyToOutput = CopyToOutputDirectory.None;
					break;
				case ProjectType.ClickOnce:
				case ProjectType.Library:
				case ProjectType.Test:
				case ProjectType.Azure:
					return;
				default:
					throw new InvalidOperationException(
						String.Format("Unknown project type: {0}.", Arguments.ProjectType));
			}

			CheckConfigProperties(configFileName, type, copyToOutput);
		}

		/// <summary>
		/// Checks properties for configuration file.
		/// </summary>
		private static void CheckConfigProperties(
			string configFileName,
			ProjectItemType type,
			CopyToOutputDirectory copyToOutput)
		{
			IEnumerable<ProjectItem> items = ProjectHelper.GetProjectItems()
				.Where(item => Path.GetFileName(item.FullName) == configFileName);

			if (items.Count() != 1)
			{
				bool ignore = false;

				if (configFileName == "Web.config"
					&& items.Count() > 0)
					ignore = true;

				if (!ignore)
					RaiseError.WrongConfigFileLocation(configFileName);
			}

			foreach (ProjectItem config in items)
			{
				StringBuilder message = new StringBuilder();

				string description;
				if (!ValidationHelper.CheckProperties(
					BuildConfigProperties(config.Type, config.CopyToOutput),
					BuildConfigProperties(type, copyToOutput),
					new Dictionary<string, string>(),
					out description))
				{
					message.Append(description);
				}

				if (message.Length == 0)
					continue;

				RaiseError.WrongFileProperties(configFileName, message.ToString());
			}
		}

		/// <summary>
		/// Builds properties collection for configuration file.
		/// </summary>
		private static Dictionary<string, string> BuildConfigProperties(
			ProjectItemType type,
			CopyToOutputDirectory copyToOutput)
		{
			Dictionary<string, string> properties = new Dictionary<string, string>();
			properties["BuildAction"] = type.ToString();
			properties["CopyToOutput"] = copyToOutput.ToString();
			return properties;
		}

		#endregion
	}
}
