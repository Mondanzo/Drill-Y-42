using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.VersionControl;

class PerforcePlusIngore {
	private string regex;

	public static PerforcePlusIngore CreateFileType(string regexString) {
		var trimmedString = regexString.TrimStart();
		if (trimmedString.StartsWith("r")) {
			return new PerforcePlusIngore {
				regex = trimmedString.Substring(1)
			};
		}
		return new PerforcePlusIngore {
			regex = trimmedString.Replace("/", "\\/")
				.Replace(".", "\\.")
				.Replace("*", "\\*")
				.Replace("(", "\\(")
				.Replace(")", "\\)")
				.Replace("?", "\\?")
				.Replace("+", "\\+")
		};
	}

	public bool CheckIfAssetIsIgnored(Asset asset) {
		return Regex.Match(asset.assetPath, regex).Success;
	}
}