using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SumArrayServer
{
	class DataServer
	{
		private const int BUFFER_SIZE = 1024;
		private const int CLIENT_NUM = 2;

		private TcpListener server;
		private List<TcpClient> clients;
		private List<NetworkStream> netStreams;
		private List<List<int>> dataBlocks;
		private List<int> dataBlockSizes;
		private List<int> sums;
		private byte[] dataSends, dataReceives;

		private int dataSize;

		public DataServer()
		{
			Console.WriteLine("Server init components...");
			server = new TcpListener(new IPEndPoint(IPAddress.Any, 1724));
			clients = new List<TcpClient>();
			netStreams = new List<NetworkStream>();
			dataBlocks = new List<List<int>>();
			dataBlockSizes = new List<int>();		//data blocks size receives from client
			sums = new List<int>();
			dataSends = new byte[BUFFER_SIZE];
			dataReceives = new byte[BUFFER_SIZE];
		}

		private void acceptClient()
		{
			TcpClient client = server.AcceptTcpClient();
			clients.Add(client);
			netStreams.Add(client.GetStream());
			dataBlocks.Add(new List<int>());
			sums.Add(0);		//default total value
		}

		public void startListening()
		{
			server.Start(CLIENT_NUM);		//accept only 2 client max
			Console.WriteLine("Server listening...");

			for (int i = 0; i < CLIENT_NUM; i++)		//server accepting 2 clients
			{
				acceptClient();
				Console.WriteLine("Server accepted client " + i);
			}

			for (int i = 0; i < CLIENT_NUM; i++)		//server accepting 2 clients
			{
				string signal = "ACK";

				netStreams[i].Read(dataReceives, 0, BUFFER_SIZE);
				int blockSize = BitConverter.ToInt32(dataReceives, 0);
				dataBlockSizes.Add(blockSize);	//receive data block size from client
				Console.WriteLine("Server received data block size from client " + i);
				dataSends = Encoding.ASCII.GetBytes(signal);		//ACK data block size
				netStreams[i].Write(dataSends, 0, signal.Length);
			}

			for (int i = 0; i < CLIENT_NUM; i++)
			{
				Console.WriteLine("\nServer receiving data block from client " + i);
				for (int j = 0; j < dataBlockSizes[i]; j++)
				{
					string signal = "ACK";

					dataSize = netStreams[i].Read(dataReceives, 0, BUFFER_SIZE);
					dataBlocks[i].Add(BitConverter.ToInt32(dataReceives,0));

					dataSends = Encoding.ASCII.GetBytes(signal);
					netStreams[i].Write(dataSends, 0, signal.Length);
				}
				Console.WriteLine("\nData block from client " + i);
				for (int j = 0; j < dataBlockSizes[i]; j++)
				{
					Console.Write(dataBlocks[i][j] + " ");
					sums[i] += dataBlocks[i][j];
				}
			}

			for (int i = 0; i < CLIENT_NUM; i++)
			{
				Console.WriteLine("\nSum of client " + i + ": " + sums[i]);
				dataSends = BitConverter.GetBytes(sums[i]);
				netStreams[i].Write(dataSends, 0, sizeof(int));
			}
		}

		private void disconnect()
		{
			for (int i = 0; i < CLIENT_NUM; i++)
			{
				clients[i].Close();
				netStreams[i].Close();
				dataBlocks[i].Clear();
				dataBlockSizes[i] = 0;
				sums[i] = 0;
			}
		}
	}
}
