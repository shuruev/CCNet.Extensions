using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Helps storing sensitive data in a secure way.
	/// </summary>
	public static class SecureConfig
	{
		private static CertificateEncryptor s_encryptor;

		/// <summary>
		/// Initializes a secured storage using certificate with specified thumbprint.
		/// </summary>
		public static void Initialize(string thumbprint)
		{
			var certificate = Find(thumbprint);

			if (certificate == null)
				throw new InvalidOperationException("Cannot find the required certificate.");

			if (!certificate.HasPrivateKey)
				throw new InvalidOperationException("The required certificate has no private key.");

			s_encryptor = new CertificateEncryptor(certificate);
		}

		/// <summary>
		/// Tries to find certificate by specified thumbprint.
		/// </summary>
		private static X509Certificate2 Find(string thumbprint)
		{
			var store = new X509Store(StoreLocation.CurrentUser);
			try
			{
				store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

				var found = store.Certificates;

				found = found.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
				found = found.Find(X509FindType.FindByThumbprint, thumbprint, false);

				if (found.Count == 0)
					return null;

				return found[0];
			}
			finally
			{
				store.Close();
			}
		}

		/// <summary>
		/// Encrypts specified text using UTF-8 as its binary representation.
		/// </summary>
		public static string Encrypt(string text)
		{
			var input = Encoding.UTF8.GetBytes(text);
			var output = s_encryptor.Encrypt(input);
			return Convert.ToBase64String(output);
		}

		/// <summary>
		/// Decrypts text from its specified UTF-8 binary representation.
		/// </summary>
		public static string Decrypt(string text)
		{
			var input = Convert.FromBase64String(text);
			var output = s_encryptor.Decrypt(input);
			return Encoding.UTF8.GetString(output);
		}

		/// <summary>
		/// Encrypts specified data specified by its base-64 representation.
		/// </summary>
		public static string Encrypt64(string text)
		{
			var input = Convert.FromBase64String(text);
			var output = s_encryptor.Encrypt(input);
			return Convert.ToBase64String(output);
		}

		/// <summary>
		/// Decrypts data from its specified base-64 representation.
		/// </summary>
		public static string Decrypt64(string text)
		{
			var input = Convert.FromBase64String(text);
			var output = s_encryptor.Decrypt(input);
			return Convert.ToBase64String(output);
		}
	}
}
