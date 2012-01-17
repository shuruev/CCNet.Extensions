using System.Net.Mail;

namespace CCNet.SourceNotifier.Gateways.MailGateway
{
	/// <summary>
	/// MailGateway factory.
	/// </summary>
	public static class MailGatewayFactory
	{
		/// <summary>
		/// Creates a MailGateway instance.
		/// </summary>
		public static IMailGateway CreateGateway(MailAddress sender)
		{
			if (Debug.Instance.IsDebugModeEnabled)
			{
				return new DebugGateway(sender, new MailAddress(Debug.Instance.OverrideEmail));
			}

			return new ProductionGateway(sender);
		}
	}
}
