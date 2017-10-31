using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultLib.Blocks;
using VaultLib.Tables;

namespace VaultLib.Tables
{
	public class NodeAllocationTable : Table<Node>
	{
		public static readonly uint ItemSize = 32;       // reserved size for a node

		public NodeAllocationTable(Stream Stream, HeaderBlock Header, uint Offset) :base(Stream,Header,  Offset,Header.NodeCount,NodeAllocationTable.ItemSize)
		{
		}

		protected override bool IsItemFree(Node Item)
		{
			return Item.BlockIndex == 0;
		}

		protected override Node Read()
		{
			Node node;

			node = new Node();
			node.NodeIndex = Reader.ReadUInt32();		// 4
			node.Mod = Reader.ReadByte();           // 1
			node.UID = Reader.ReadUInt16();         // 2
			node.GID = Reader.ReadUInt16();         // 2
			node.Size = Reader.ReadUInt32();        // 4
			node.BlockIndex=Reader.ReadUInt32();    // 4
													// 17

			return node;
		}

		protected override void Write(Node Item)
		{
			Writer.Write(Item.NodeIndex);
			Writer.Write(Item.Mod);
			Writer.Write(Item.UID);
			Writer.Write(Item.GID);
			Writer.Write(Item.Size);
			Writer.Write(Item.BlockIndex);
		}

		







	}

}
