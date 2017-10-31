using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultLib;

namespace TestVault
{
	class Program
	{
		static void Main(string[] args)
		{
			Vault vault;
			string path = @"d:\test.vlt";

			try
			{
				File.Delete(path);
				Vault.Create(path, 32, 128,16);//*/

				vault = new Vault(path);
				int result = vault.mkdir("/tit");
				Console.WriteLine(result);
				result = vault.mkdir("/tit/tot");
				Console.WriteLine(result);

				vault.Dispose();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.WriteLine("EOP");
			Console.Write(">"); Console.ReadLine();
		}
	}
}
