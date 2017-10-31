using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultLib.Blocks;

namespace VaultLib.Tables
{
	public class DirectoryTable : Table<DirectoryEntry>
	{
		public static readonly uint MaxNameLength = 3;
		public static readonly uint ItemSize = 4+1+MaxNameLength;       // size of an index + size of name + name data


		public DirectoryTable(Stream Stream, HeaderBlock Header, uint Offset ) : base(Stream, Header, Offset, Header.BlockSize/ItemSize, ItemSize)
		{
			if (this.GetCount() <=2 ) throw (new InvalidDataException($"Item count must be greater than two"));
		}

		protected override bool IsItemFree(DirectoryEntry Item)
		{
			return Item.NodeIndex == 0;
		}

		protected override DirectoryEntry Read()
		{
			DirectoryEntry item = new DirectoryEntry();
			item.NodeIndex = Reader.ReadUInt32();
			item.NameSize = Reader.ReadByte();
			item.Name = new byte[DirectoryTable.MaxNameLength];
			Reader.Read(item.Name, 0, (int)DirectoryTable.MaxNameLength);
			return item;
		}

		protected override void Write(DirectoryEntry Item)
		{
			Writer.Write(Item.NodeIndex);
			Writer.Write(Item.NameSize);
			Writer.Write(Item.Name);
		}

		public int Find(string Name,out DirectoryEntry Entry)
		{
			Entry = new DirectoryEntry();

			Seek(0);
			while(!IsEndOfTable())
			{
				Entry = Read();
				if (Entry.GetName() == Name) return 0;
			}
			ErrorUtils.SetErro(ErrorCodes.ENOENT);
			return -1;
		}

		public int Initialize(uint DirectoryNodeIndex,uint ParentNodeIndex)
		{
			DirectoryEntry entry;

			if (Initialize() != 0) return -1;

			entry = new DirectoryEntry();
			entry.SetName(".");
			entry.NodeIndex = DirectoryNodeIndex;
			Write(0, entry);

			entry = new DirectoryEntry();
			entry.SetName("..");
			entry.NodeIndex = ParentNodeIndex;
			Write(1, entry);

			return 0;
		}
		




	}
}
