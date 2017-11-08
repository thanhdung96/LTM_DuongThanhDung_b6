using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
 
namespace SumArrayClient
{
	//client send to server first (data block size)
	class DataClient
	{
		private const int BUFFER_SIZE = 1024;

		private TcpClient client;
		private NetworkStream netStream;
		private List<int> blockData;		//list of integer to send to server
		private byte[] dataSend, dataReceive;
		private int dataSize;		//number of bytes received from server

		public DataClient(List<int> blockData)
		{
			Console.WriteLine("Client init components...");
			this.blockData = blockData;
			dataReceive = new byte[BUFFER_SIZE];
			dataSend = new byte[BUFFER_SIZE];

			client = new TcpClient();
		}

		public int sendData()
		{
			//connectin to server
			client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1724));
			netStream = client.GetStream();
			Console.WriteLine("Client connected to server...");

			//sending block data size to server
			dataSend = BitConverter.GetBytes(blockData.Count);
			netStream.Write(dataSend, 0, sizeof(int));		//sending 4 bytes of data block size (type int is 4 bytes)
			Console.WriteLine("Client sent block data size...");

			dataSize = netStream.Read(dataReceive, 0, BUFFER_SIZE);	//receive signal from server
			string signal = Encoding.ASCII.GetString(dataReceive, 0, dataSize);
			if (signal == "ACK")		//if server received data block size and sent ACK; first check
			{
				Console.WriteLine("Client sending block data...");
				foreach (int value in blockData)		//send each value in block data to server
				{
					if (signal == "ACK")
					{
						dataSend = BitConverter.GetBytes(value);
						netStream.Write(dataSend, 0, sizeof(int));		//sending 4 bytes of each value in block data

						dataSize = netStream.Read(dataReceive, 0, BUFFER_SIZE);	//receive signal from server
						signal = Encoding.ASCII.GetString(dataReceive, 0, dataSize);
					}
					else
					{
						break;
					}
				}
				Console.WriteLine("Client sent block data...");
			}

			if (signal == "ACK")		//server received all values in block data and ACKed
			{
				//client receive sum result from server
				netStream.Read(dataReceive, 0, 4);	//receive 4 bytes of sum result from server
				int result = BitConverter.ToInt32(dataReceive, 0);
				Console.WriteLine("Client received sum resutl...");
				return result;
			}
			return -1;
		}
	}
}
