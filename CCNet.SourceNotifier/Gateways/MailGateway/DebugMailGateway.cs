using System.Net.Mail;

namespace CCNet.SourceNotifier.Gateways.MailGateway
{
	/// <summary>
	/// Used in a debug build.
	/// </summary>
	public class DebugMailGateway : ProductionMailGateway
	{
		/// <summary>
		/// All mails will be sent to this address instead of their original destination.
		/// </summary>
		private readonly MailAddress m_overrideEmail;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public DebugMailGateway(MailAddress sender, MailAddress overrideEmail)
			: base(sender)
		{
			m_overrideEmail = overrideEmail;
		}

		/// <summary>
		/// Sends a message.
		/// </summary>
		public override void SendMessage(MailAddress to, string subject, string bodyHtml)
		{
			base.SendMessage(m_overrideEmail, string.Format(Properties.Resources.DebugModeMessageSubjectFormat, subject, to), bodyHtml);
		}
	}
}
