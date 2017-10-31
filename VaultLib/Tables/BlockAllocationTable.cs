using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultLib.Blocks;

namespace VaultLib.Tables
{
	public enum BlockTypes:uint { Free=0, Header=1,BlockAllocationTable=2, NodeAllocationTable = 3 };

	public class BlockAllocationTable : Table<uint>
	{
		public static readonly uint ItemSize = 4;       // size of an index
		public static readonly uint EndOfBlockChain = 0xFFFFFFFF;

		public BlockAllocationTable(Stream Stream,HeaderBlock Header, uint Offset) :base(Stream,Header,  Offset,Header.BlockCount,BlockAllocationTable.ItemSize)
		{
		}
				
		protected override uint Read()
		{
			return Reader.ReadUInt32();
		}

		protected override void Write(uint Item)
		{
			Writer.Write(Item);
		}
				
		protected override bool IsItemFree(uint Item)
		{
			return Item == (int)BlockTypes.Free;
		}

		public override int Initialize()
		{
			Seek(0);

			Write((uint)BlockTypes.Header);

			for (int t = 0; t < Header.BlockAllocationTableSize; t++)
			{
				Write((uint)BlockTypes.BlockAllocationTable);
			}

			for (int t = 0; t < Header.NodeAllocationTableSize; t++)
			{
				Write((uint)BlockTypes.NodeAllocationTable);
			}

			while (!IsEndOfTable())
			{
				Write((uint)BlockTypes.Free);
			}

			Stream.Flush();

			return 0;
		}
	
		
	

		
	}
}
