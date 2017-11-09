namespace LoginServer
{
	class Program
	{
		private static LoginService loginSvs;

		static void Main(string[] args)
		{
			loginSvs = new LoginService();
			loginSvs.start();
		}
	}
}
