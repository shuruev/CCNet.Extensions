﻿using System;
using System.Net.Mail;
using CCNet.SourceNotifier.Properties;
using VX.Sys;

namespace CCNet.SourceNotifier.Gateways.MailGateway
{
	/// <summary>
	/// Used in a production build.
	/// </summary>
	public class ProductionMailGateway : IMailGateway
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
		public ProductionMailGateway(MailAddress sender)
		{
			m_smtpClient = new SmtpClient();
			m_sender = sender;
		}

		/// <summary>
		/// Sends a message.
		/// </summary>
		public virtual void SendMessage(MailAddress to, string subject, string bodyHtml)
		{
			using (var message = new MailMessage(m_sender, to))
			{
				message.Subject = subject;
				message.IsBodyHtml = true;
				message.Body = HtmlStyler.InlineCss(bodyHtml);
				m_smtpClient.Send(message);
				Debug.Instance.Log(Resources.MailSentLogMessage, message.Subject, message.To);
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			m_smtpClient.Dispose();
		}
	}
}
