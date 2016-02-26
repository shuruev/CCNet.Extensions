using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Encrypts using a specific certificate.
	/// </summary>
	public class CertificateEncryptor
	{
		private readonly X509Certificate2 m_certificate;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CertificateEncryptor(X509Certificate2 certificate)
		{
			if (certificate == null)
				throw new ArgumentNullException("certificate");

			m_certificate = certificate;
		}

		/// <summary>
		/// Encrypts specified data.
		/// </summary>
		public byte[] Encrypt(byte[] data)
		{
			var rsa = (RSACryptoServiceProvider)m_certificate.PublicKey.Key;
			return rsa.Encrypt(data, true);
		}

		/// <summary>
		/// Decrypts specified data.
		/// </summary>
		public byte[] Decrypt(byte[] data)
		{
			var rsa = (RSACryptoServiceProvider)m_certificate.PrivateKey;
			return rsa.Decrypt(data, true);
		}
	}
}
