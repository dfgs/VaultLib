using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultLib.Tables;

namespace VaultLib
{
	public struct DirectoryEntry
	{

		public uint NodeIndex
		{
			get;
			set;
		}

		public byte NameSize
		{
			get;
			set;
		}

		public byte[] Name
		{
			get;
			set;
		}
		
		

		public string GetName()
		{
			return Encoding.UTF8.GetString(Name,0,NameSize);
		}
		public void SetName(string Value)
		{
			Name=Encoding.UTF8.GetBytes(Value);
			if (Name.Length > DirectoryTable.MaxNameLength) throw (new InvalidOperationException($"Name must not exceed {DirectoryTable.MaxNameLength} bytes"));
			NameSize = (byte)Name.Length;
		}


	}
}
