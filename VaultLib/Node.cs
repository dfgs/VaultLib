using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLib
{
	public struct Node
	{
		public uint NodeIndex
		{
			get;
			set;
		}
		public byte Mod
		{
			get;
			set;
		}
		public ushort UID
		{
			get;
			set;
		}
		public ushort GID
		{
			get;
			set;
		}
		public uint Size
		{
			get;
			set;
		}
		public uint BlockIndex
		{
			get;
			set;
		}

		

	}
}
