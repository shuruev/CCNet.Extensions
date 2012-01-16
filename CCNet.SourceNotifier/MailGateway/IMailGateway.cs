using System;
using System.Net.Mail;

namespace CCNet.SourceNotifier.MailGateway
{
	/// <summary>
	/// Provides SendMessage method.
	/// </summary>
	public interface IMailGateway : IDisposable
	{
		/// <summary>
		/// Sends a message.
		/// </summary>
		void SendMessage(MailAddress to, string subject, string bodyHtml);
	}
}
