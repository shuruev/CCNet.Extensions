using System.Net.Mail;

namespace CCNet.SourceNotifier.MailGateway
{
	/// <summary>
	/// Used in a debug build.
	/// </summary>
	public class DebugGateway : ProductionGateway
	{
		/// <summary>
		/// All mails will be sent to this address instead of their original destination.
		/// </summary>
		private readonly MailAddress m_overrideEmail;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public DebugGateway(MailAddress sender, MailAddress overrideEmail)
			: base(sender)
		{
			m_overrideEmail = overrideEmail;
		}

		/// <summary>
		/// Sends a message.
		/// </summary>
		public override void SendMessage(MailAddress to, string subject, string bodyHtml)
		{
			base.SendMessage(m_overrideEmail, string.Format("{0} (originally to {1})", subject, to), bodyHtml);
		}
	}
}
