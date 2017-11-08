namespace SumArrayServer
{
	class Program
	{
		private static DataServer dataServer;

		static void Main(string[] args)
		{
			dataServer = new DataServer();
			dataServer.startListening();
		}
	}
}
