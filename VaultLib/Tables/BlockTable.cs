using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VaultLib.Blocks;
using VaultLib.Tables;


namespace VaultLib.Tables
{
    public class BlockTable:Table<byte[]>
    {
		public static readonly uint HeaderBlockIndex=0;


		public static readonly uint BatBlockIndex=1;


		private BlockAllocationTable bat;
		/*public BlockAllocationTable BAT
		{
			get { return bat; }
		}*/
		private NodeAllocationTable nat;
		/*public NodeAllocationTable NAT
		{
			get { return nat; }
		}*/


		private BlockTable(Stream Stream,HeaderBlock Header) :base(Stream, Header, 0,Header.BlockCount,Header.BlockSize)
		{
			bat = new BlockAllocationTable(Stream, Header, GetOffset(BatBlockIndex));
			nat = new NodeAllocationTable(Stream, Header, GetOffset(BatBlockIndex + Header.BlockAllocationTableSize  ));
		}

		public override void Dispose()
		{
			if (Stream != null)
			{
				Stream.Flush();
				Stream.Close();
			}
		}

		protected override bool IsItemFree(byte[] Item)
		{
			return false;
		}

		protected override byte[] Read()
		{
			byte[] buffer = new byte[Header.BlockSize];
			Stream.Read(buffer, 0, Header.BlockSize);
			return buffer;
		}
		
		public BlockType ReadBlock<BlockType>(uint Index)
		{
			Seek(Index);
			return ReadBlock<BlockType>(Stream);
		}

		private static BlockType ReadBlock<BlockType>(Stream Stream)
		{
			BlockType Item;

			int size = Marshal.SizeOf(typeof(BlockType));
			byte[] buffer = new byte[size];
			Stream.Read(buffer, 0, size);

			IntPtr ptr = Marshal.AllocHGlobal(size);
			Marshal.Copy(buffer, 0, ptr, size);
			Item = Marshal.PtrToStructure<BlockType>(ptr);
			Marshal.FreeHGlobal(ptr);

			return Item;
		}


		protected override void Write(byte[] Item)
		{
			Stream.Write(Item, 0, Header.BlockSize);
		}
		public void WriteBlock<BlockType>(uint Index,BlockType Item)
		{
			Seek(Index);

			int size = Marshal.SizeOf(typeof(BlockType));
			byte[] buffer = new byte[size];

			IntPtr ptr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(Item, ptr, true);
			Marshal.Copy(ptr, buffer, 0, size);
			Marshal.FreeHGlobal(ptr);

			Stream.Write(buffer,0, size);

		}

		
		public Node GetRootNode()
		{
			return nat.Read(0);
		}


		public override int Initialize()
		{
			byte[] buffer;
			Node rootNode;
			DirectoryTable rootDirectoryTable;

			buffer = new byte[Header.BlockSize];

			#region fill disk with 0
			Seek(0);
			while(!IsEndOfTable())
			{
				Write(buffer);
			}
			#endregion

			#region write header
			WriteBlock(BlockTable.HeaderBlockIndex,Header);
			#endregion

			#region write block allocation table
			if (bat.Initialize()!=0) return -1;
			#endregion

			if (AllocateNode(out rootNode) != 0) return -1 ;
			rootDirectoryTable=OpenDirectoryTable(rootNode);
			if (rootDirectoryTable == null) return -1;
			rootDirectoryTable.Initialize(rootNode.NodeIndex, rootNode.NodeIndex);

			Stream.Flush();

			return 0;
		}

		public static void Create(string FileName,ushort BlockSize,uint BlockCount,uint NodeCount)
		{
			BlockTable vault;
			HeaderBlock header;
			FileStream stream;

			stream= new FileStream(FileName, FileMode.CreateNew,FileAccess.ReadWrite,FileShare.None);

			header = new HeaderBlock()
			{
				MajorVersion=0,
				MinorVersion=1,
				BlockCount = BlockCount,
				BlockSize = BlockSize,
				NodeCount = NodeCount,
				BlockAllocationTableSize = GetUsedBlocks(BlockCount,BlockAllocationTable.ItemSize,BlockSize),
				NodeAllocationTableSize = GetUsedBlocks(NodeCount,NodeAllocationTable.ItemSize,BlockSize)
			};
			vault = new BlockTable(stream,header);

			vault.Initialize();
			vault.Dispose();
		}

		public static BlockTable Open(string FileName)
		{
			BlockTable vault;
			HeaderBlock header;
			FileStream stream;

			stream = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			header = ReadBlock<HeaderBlock>(stream);
			
			vault = new BlockTable(stream, header);

			return vault;			
		}

		public int AllocateNode(out Node Node)
		{
			uint nodeIndex, blockIndex;

			Node = new Node() { GID = 0, UID = 0 };
			if (nat.GetFreeIndex(out nodeIndex) != 0)
			{
				ErrorUtils.SetErro(ErrorCodes.ENOSPC);
				return -1;
			}
			Node.NodeIndex = nodeIndex;

			if (bat.GetFreeIndex(out blockIndex) != 0)
			{
				ErrorUtils.SetErro(ErrorCodes.ENOSPC);
				return -1;
			}
			Node.BlockIndex = blockIndex;

			nat.Write(nodeIndex, Node);
			bat.Write(blockIndex, BlockAllocationTable.EndOfBlockChain);

			return 0;
		}


		public int FindDirectoryNode(Node RelativeNode,string Path,out Node Node)
		{
			DirectoryTable directoryTable;
			DirectoryEntry entry;
			int result;
			string[] names;

			Node = RelativeNode;
			names = PathUtils.Split(Path);

			directoryTable = null;
			for(int t=0;t<names.Length;t++)
			{
				directoryTable = new DirectoryTable(Stream, Header, GetOffset(Node.BlockIndex));
				result = directoryTable.Find(names[t], out entry);
				directoryTable.Dispose();
				if (result != 0) return -1;
				Node = nat.Read(entry.NodeIndex);
			}

			return 0;
		}

		

		public DirectoryTable OpenDirectoryTable(Node DirectoryNode)
		{
			return new DirectoryTable(Stream, Header, GetOffset(DirectoryNode.BlockIndex)); ;
		}

		/*public byte GetMajorVersion()
		{
			return Header.MajorVersion;
		}
		public byte GetMinorVersion()
		{
			return Header.MinorVersion;
		}

		public uint GetVaultSize()
		{
			return Header.BlockSize * Header.BlockCount;
		}

		public uint GetBlockSize()
		{
			return Header.BlockSize;
		}

		public uint GetBlockCount()
		{
			return Header.BlockCount;
		}
		public uint GetNodeCount()
		{
			return Header.NodeCount;
		}

		*/





	}
}
