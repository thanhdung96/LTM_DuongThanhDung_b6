namespace SumArrayClient
{
	class Program
	{
		private static DataClient dataClient;

		static void Main(string[] args)
		{
			dataClient = new DataClient(new System.Collections.Generic.List<int> {1,2,3,4,5});
			int result = dataClient.sendData();
			System.Console.WriteLine("Result: " + result);
		}
	}
}
