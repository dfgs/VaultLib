using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultLib.Blocks;

namespace VaultLib.Tables
{
	public abstract class Table<ItemType>:IDisposable
	{
		private uint offset;
		private uint count;
		private uint itemSize;

		private HeaderBlock header;
		protected HeaderBlock Header
		{
			get { return header; }
		}

		private BinaryWriter writer;
		protected BinaryWriter Writer
		{
			get { return writer; }
		}
		private BinaryReader reader;
		protected BinaryReader Reader
		{
			get { return reader; }
		}

		private Stream stream;
		protected Stream Stream
		{
			get { return stream; }
		}
		
		public Table(Stream Stream,HeaderBlock Header,uint Offset,uint Count,uint ItemSize)
		{
			if (ItemSize == 0) throw (new InvalidDataException($"Item size must be greater than zero"));
			if (Count == 0) throw (new InvalidDataException($"Item count must be greater than zero"));
			if (Header.BlockSize % ItemSize != 0) throw (new InvalidDataException($"Block size must be a multiple of {ItemSize} bytes"));

			this.header = Header;
			this.stream = Stream;
			this.offset = Offset;
			this.count = Count;
			this.itemSize = ItemSize;
			writer = new BinaryWriter(stream);
			reader = new BinaryReader(stream);
			
		}

		public virtual void Dispose()
		{
			
		}

		protected abstract ItemType Read();
		protected abstract void Write(ItemType Item);
		protected abstract bool IsItemFree(ItemType Item);

		public ItemType Read(uint Index)
		{
			Seek(Index);
			return Read();
		}

		
		public void Write(uint Index,ItemType Item)
		{
			Seek(Index);
			Write(Item);
		}

		public void Clear()
		{
			byte[] buffer;

			Seek(0);
			buffer = new byte[GetItemSize()* GetCount() ];
			Stream.Write(buffer, 0, (int)GetItemSize());
		}

		public void Clear(uint Index)
		{
			byte[] buffer;

			Seek(Index);
			buffer = new byte[GetItemSize()];
			Stream.Write(buffer, 0, (int)GetItemSize());
		}

		public uint GetItemSize()
		{
			return itemSize;
		}

		public void Seek(uint Index)
		{
			stream.Seek(offset + Index * itemSize,SeekOrigin.Begin);
		}

		public virtual int Initialize()
		{
			Clear();
			return 0;
		}

		public bool IsEndOfTable()
		{
			return stream.Position >= offset + count * GetItemSize();
		}

		public uint GetOffset()
		{
			return offset;
		}
		public uint GetOffset(uint Index)
		{
			return offset + Index * itemSize;
		}

		public uint GetCount()
		{
			return count;
		}
		public static uint GetUsedBlocks(uint Count,uint ItemSize,ushort BlockSize)
		{
			return (uint)Math.Ceiling((float)(Count * ItemSize) / (float)BlockSize);
		}

		public int GetFreeIndex(out uint Index)
		{
			ItemType item;

			Index = 0;
			Seek(0);
			while (!IsEndOfTable())
			{
				item = Read();
				if (IsItemFree(item)) return 0;
				Index++;
			}

			return -1;
		}





	}
}
