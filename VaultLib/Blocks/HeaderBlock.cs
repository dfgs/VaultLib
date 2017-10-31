using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLib.Blocks
{
	public struct HeaderBlock
	{
		public byte MajorVersion
		{
			get;
			set;
		}
		public byte MinorVersion
		{
			get;
			set;
		}
		public ushort BlockSize
		{
			get;
			set;
		}
		public uint BlockCount
		{
			get;
			set;
		}
		public uint NodeCount
		{
			get;
			set;
		}

		public uint BlockAllocationTableSize
		{
			get;
			set;
		}
		public uint NodeAllocationTableSize
		{
			get;
			set;
		}

		


	}
}
