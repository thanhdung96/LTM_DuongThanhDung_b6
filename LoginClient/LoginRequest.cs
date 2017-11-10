using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LoginClient
{
	class LoginRequest
	{
		private const int BUFFER_SIZE = 1024;

		private TcpClient client;
		private NetworkStream netStream;
		private byte[] dataSend, dataReceive;
		private int dataSize;

		public LoginRequest()
		{
			Console.WriteLine("Client init components...");
			client = new TcpClient();
			dataSend = new byte[BUFFER_SIZE];
			dataReceive = new byte[BUFFER_SIZE];
		}

		public void connect()
		{
			client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1724));
			netStream = client.GetStream();
			Console.WriteLine("Client connected to server...");
		}

		public void resquest(string username, string password)
		{
			string data;

			string raw = username + "@" + password;		//prepare login info
			MD5Encrypt md5 = new MD5Encrypt(raw);
			string encrypted = md5.getHash();
			Console.WriteLine("Encrypted data: " + encrypted);

			data = "LOG_REQ";			//send login request to server
			dataSend = Encoding.ASCII.GetBytes(data);
			netStream.Write(dataSend, 0, data.Length);
			Console.WriteLine("Client sent login request...");

			dataSize = netStream.Read(dataReceive, 0, BUFFER_SIZE);		//receiving respond from server
			data = Encoding.ASCII.GetString(dataReceive, 0, dataSize);

			//send username and encrypted to server for authentication
			if (data == "ACK")		//if serve ACKs request
			{
				//send username to server
				dataSend = Encoding.ASCII.GetBytes(username);
				netStream.Write(dataSend, 0, username.Length);
				dataSize = netStream.Read(dataReceive, 0, BUFFER_SIZE);		//receiving respond from server
				data = Encoding.ASCII.GetString(dataReceive, 0, dataSize);
				Console.WriteLine("Client sent username...");
			}
			if (data == "ACK")
			{
				//send encrypted data to server
				dataSend = Encoding.ASCII.GetBytes(encrypted);
				netStream.Write(dataSend, 0, encrypted.Length);
				dataSize = netStream.Read(dataReceive, 0, BUFFER_SIZE);		//receiving respond from server
				data = Encoding.ASCII.GetString(dataReceive, 0, dataSize);
				Console.WriteLine("Client sent encrypted data...");
			}

			dataSize = netStream.Read(dataReceive, 0, BUFFER_SIZE);		//receiving respond from server
			data = Encoding.ASCII.GetString(dataReceive, 0, dataSize);
			if (data == "LOG_OK")		//if serve ACKs request
			{
				dataSize = netStream.Read(dataReceive, 0, BUFFER_SIZE);		//receiving respond from server
				data = Encoding.ASCII.GetString(dataReceive, 0, dataSize);
				Console.WriteLine("Welcome " + data);

			}
			else if (data == "LOG_FAILED")
			{
				Console.WriteLine("Wrong username or password");
			}
		}
	}
}
