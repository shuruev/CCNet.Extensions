using System;
using System.Net.Mail;
using VX.Sys;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// Incapsulates all the mail-related functionality.
	/// </summary>
	internal abstract class MailGateway : IDisposable
	{
		/// <summary>
		/// Used in a production build.
		/// </summary>
		private class ProductionGateway : MailGateway
		{
			/// <summary>
			/// SmtpClient instance.
			/// </summary>
			private readonly SmtpClient m_smtpClient;

			/// <summary>
			/// Mail address of a robot used to send emails.
			/// </summary>
			private readonly MailAddress m_sender;

			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			public ProductionGateway(MailAddress sender)
			{
				m_smtpClient = new SmtpClient();
				m_sender = sender;
			}

			/// <summary>
			/// Sends a message.
			/// </summary>
			public override void SendMessage(MailAddress to, string subject, string bodyHtml)
			{
				using (var message = new MailMessage(m_sender, to))
				{
					message.Subject = subject;
					message.IsBodyHtml = true;
					message.Body = HtmlStyler.InlineCss(bodyHtml);
					m_smtpClient.Send(message);
					Debug.Instance.Log("Mail sent to {0}", message.To);
				}
			}

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public override void Dispose()
			{
				m_smtpClient.Dispose();
			}
		}

		/// <summary>
		/// Used in a debug build.
		/// </summary>
		private class DebugGateway : ProductionGateway
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

		/// <summary>
		/// Sends a message.
		/// </summary>
		public abstract void SendMessage(MailAddress to, string subject, string bodyHtml);

		/// <summary>
		/// Creates a MailGateway instance.
		/// </summary>
		public static MailGateway CreateGateway(MailAddress sender)
		{
			if (Debug.Instance.IsDebugModeEnabled)
			{
				return new DebugGateway(sender, new MailAddress(Debug.Instance.OverrideEmail));
			}

			return new ProductionGateway(sender);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public abstract void Dispose();
	}
}
