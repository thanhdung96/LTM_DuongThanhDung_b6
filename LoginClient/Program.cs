namespace LoginClient
{
	class Program
	{
		private static LoginRequest req;

		static void Main(string[] args)
		{
			req = new LoginRequest();
			req.connect();
			req.resquest("dung", "password");
			System.Console.ReadKey();
		}
	}
}
