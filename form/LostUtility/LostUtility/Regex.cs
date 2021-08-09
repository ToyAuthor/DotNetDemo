using StdRegex = System.Text.RegularExpressions;
using SysContainer = System.Collections.Generic;

namespace LostUtility
{
	public class Regex
	{
		private StdRegex.Regex _core;

		public Regex( string pattern )
		{
			_core = new StdRegex.Regex( pattern );
		}

		public SysContainer.List<string> GetMatches( string input )
		{
			var list = new SysContainer.List<string>();

			foreach ( StdRegex.Match match in _core.Matches( input ) )
			{
				list.Add( match.Value );
			}

			return list;
		}
	}
}
