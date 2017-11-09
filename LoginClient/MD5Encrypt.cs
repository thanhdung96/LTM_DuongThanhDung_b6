using System;
using System.Security.Cryptography;
using System.Text;

namespace LoginClient
{
	class MD5Encrypt
	{
		private MD5CryptoServiceProvider md5;
		private byte[] hashedBytes;
		private UTF8Encoding utf8;

		private string raw;

		public MD5Encrypt(string raw)
		{
			this.raw = raw;
			md5 = new MD5CryptoServiceProvider();
			utf8 = new UTF8Encoding();
		}

		public MD5Encrypt()
		{
			this.raw = "";
			md5 = new MD5CryptoServiceProvider();
			hashedBytes = new byte[48];
			utf8 = new UTF8Encoding();
		}

		public void setRaw(string raw)
		{
			this.raw = raw;
		}

		public string getHash()
		{
			hashedBytes = md5.ComputeHash(utf8.GetBytes(raw));
			string encrypted = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
			return encrypted;
		}
	}
}
