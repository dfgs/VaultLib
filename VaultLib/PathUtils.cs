using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VaultLib
{
	public static class PathUtils
	{
		private static readonly Regex parentPathRegex = new Regex(@"^((?:[^\/]*\/)+)([^\/]+)\/?$");	// cg 0: Parent folder, cg 1: File/directory name, note allow input string to finish with /, Allow relative and absolute paths

		public static bool ExtractParentFolder(string Path,out string ParentFolder,out string FileName)
		{
			Match match;

			ParentFolder = null;FileName = null;
			match = parentPathRegex.Match(Path);
			if (!match.Success)
			{
				ErrorUtils.SetErro(ErrorCodes.ENOTDIR);
				return false;
			}

			ParentFolder = match.Groups[1].Value;
			FileName = match.Groups[2].Value;

			return true;

		}

		public static bool IsRelative(string Path)
		{
			return (Path[0] != '/');
		}

		public static string[] Split(string Path)
		{
			return Path.Split( new char[] { '/' } , StringSplitOptions.RemoveEmptyEntries);
		}

	}
}
