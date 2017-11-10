using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LoginServer
{
	class LoginService
	{
		private const int BUFFER_SIZE = 1024;
		private const int CLIENT_NUM = 2;
		private string data;
		private int dataSize;

		private TcpListener server;
		private List<TcpClient> clients;
		private List<NetworkStream> netStreams;
		private byte[] dataSends, dataReceives;

		private Dictionary<string, Dictionary<string, string>> users;		//{username : {encrypted : display name}}; datastore
		private List<string> usernames;
		private List<string> encrypted;

		public LoginService()
		{
			Console.WriteLine("Server init components...");
			server = new TcpListener(new IPEndPoint(IPAddress.Any, 1724));
			clients = new List<TcpClient>();
			netStreams = new List<NetworkStream>();
			dataSends = new byte[BUFFER_SIZE];
			dataReceives = new byte[BUFFER_SIZE];

			users = new Dictionary<string, Dictionary<string, string>>();
			Dictionary<string, string> temp = new Dictionary<string, string>();
			//md5("dung@password") = 6d5d99f94c4ce287028053ef6ddcfffc
			temp.Add("6d5d99f94c4ce287028053ef6ddcfffc", "Duong Thanh Dung");
			users.Add("dung", temp);
			usernames = new List<string>();
			encrypted = new List<string>();
		}

		private void acceptClient()
		{
			TcpClient client = server.AcceptTcpClient();
			clients.Add(client);
			netStreams.Add(client.GetStream());
		}

		public void start()
		{
			server.Start(CLIENT_NUM);
			Console.WriteLine("Server started listening...");

			Console.WriteLine("Server accepting clients...");
			for (int i = 0; i < CLIENT_NUM; i++)
			{
				acceptClient();
				Console.WriteLine("Server accepted client " + i);
			}

			for (int i = 0; i < CLIENT_NUM; i++)		//receiving username from 2 clients
			{
				string signal = "ACK";

				dataSize = netStreams[i].Read(dataReceives, 0, BUFFER_SIZE);
				data = Encoding.ASCII.GetString(dataReceives, 0, dataSize);
				Console.WriteLine("Received login request from client " + i);
				if (data == "LOG_REQ")
				{
					dataSends = Encoding.ASCII.GetBytes(signal);		//ACK data block size
					netStreams[i].Write(dataSends, 0, signal.Length);
				}
			}

			for (int i = 0; i < CLIENT_NUM; i++)		//receiving username from 2 clients
			{
				string signal = "ACK";

				dataSize = netStreams[i].Read(dataReceives, 0, BUFFER_SIZE);
				data = Encoding.ASCII.GetString(dataReceives, 0, dataSize);
				Console.WriteLine("Received username from client " + i + ": " + data);
				usernames.Add(data);
				dataSends = Encoding.ASCII.GetBytes(signal);		//ACK data block size
				netStreams[i].Write(dataSends, 0, signal.Length);
			}

			for (int i = 0; i < CLIENT_NUM; i++)		//receiving encrypted from 2 clients
			{
				string signal = "ACK";

				dataSize = netStreams[i].Read(dataReceives, 0, BUFFER_SIZE);
				data = Encoding.ASCII.GetString(dataReceives, 0, dataSize);
				Console.WriteLine("Received encrypted data from client " + i + ": " + data);
				encrypted.Add(data);
				dataSends = Encoding.ASCII.GetBytes(signal);		//ACK data block size
				netStreams[i].Write(dataSends, 0, signal.Length);
			}

			for (int i = 0; i < CLIENT_NUM; i++)		//sending respond
			{
				string signal;
				try
				{
					String displayName = findUser(usernames[i], encrypted[i]);
					//if user exist
					//send "LOG_OK" to client
					//send display name found to client to client
					signal = "LOG_OK";
					dataSends = Encoding.ASCII.GetBytes(signal);		//ACK data block size
					netStreams[i].Write(dataSends, 0, signal.Length);

					dataSends = Encoding.ASCII.GetBytes(displayName);
					netStreams[i].Write(dataSends, 0, displayName.Length);
				}
				catch (KeyNotFoundException)
				{
					//send "LOG_FAIL" to client
					//continue to next loop
					signal = "LOG_FAIL";
					dataSends = Encoding.ASCII.GetBytes(signal);		//ACK data block size
					netStreams[i].Write(dataSends, 0, signal.Length);
				}
			}
		}

		private String findUser(string username, string encrypted)
		{
			try
			{
				return users[username][encrypted];
			}
			catch
			{
				throw new KeyNotFoundException();
			}
		}
	}
}
