using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultLib.Tables;

namespace VaultLib
{
	public class Vault:IDisposable
	{
		private BlockTable blockTable;
		private Node currentNode;
		private Node rootNode;

		public Vault(string FileName)
		{
			blockTable = BlockTable.Open(FileName);
			rootNode = blockTable.GetRootNode();
			currentNode = rootNode;
		}

		public void Dispose()
		{
			if (blockTable != null) blockTable.Dispose();
			
		}

		public static void Create(string FileName, ushort BlockSize, uint BlockCount, uint NodeCount)
		{
			BlockTable.Create(FileName, BlockSize, BlockCount, NodeCount);
		}

		public int mkdir(string Path)
		{
			Node relativeNode,newNode,parentNode;
			DirectoryEntry newDirectoryEntry;
			string parentFolder, directoryName;
			uint directoryEntryIndex;
			DirectoryTable parentDirectoryTable,newDirectoryTable;

			if (!PathUtils.ExtractParentFolder(Path, out parentFolder, out directoryName)) return -1;

			if (PathUtils.IsRelative(parentFolder)) relativeNode = this.currentNode;
			else relativeNode = rootNode;

			if (blockTable.FindDirectoryNode(relativeNode, parentFolder, out parentNode) != 0) return -1;

			parentDirectoryTable=blockTable.OpenDirectoryTable(parentNode);
			if (parentDirectoryTable == null) return -1;

			if (parentDirectoryTable.GetFreeIndex(out directoryEntryIndex) != 0)
			{
				ErrorUtils.SetErro(ErrorCodes.ENOSPC);
				return -1;
			}

			if (blockTable.AllocateNode(out newNode) != 0) return -1;
			newDirectoryTable=blockTable.OpenDirectoryTable(newNode);

			newDirectoryTable.Initialize(newNode.NodeIndex, parentNode.NodeIndex);


			#region write Node and allocated datablock
			newDirectoryEntry = new DirectoryEntry();
			newDirectoryEntry.SetName(directoryName);
			newDirectoryEntry.NodeIndex = newNode.NodeIndex;
			parentDirectoryTable.Write(directoryEntryIndex, newDirectoryEntry);
			#endregion


			return 0;
		}



	}
}
