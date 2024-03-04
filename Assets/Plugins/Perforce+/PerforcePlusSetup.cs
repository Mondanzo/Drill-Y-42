using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;


[InitializeOnLoad]
public class PerforcePlusSetup {

	private static List<PerforcePlusIngore> ignoredFiles;
	private static DateTime lastWriteTime;
	
	static PerforcePlusSetup() {
		Provider.preSubmitCallback += preSubmit;

		ReloadIgnore();
	}


	[MenuItem("Perforce+/Reload .unityIgnore")]
	public static void ReloadIgnore() {
		if (File.Exists("./.unityIgnore")) {
			if(File.GetLastWriteTimeUtc("./.unityIngore") != lastWriteTime) {
				ignoredFiles = new List<PerforcePlusIngore>();
				foreach (var line in File.ReadLines("./.unityIgnore")) {
					var trimmedLine = line.Trim();
					if (trimmedLine.StartsWith('#')) continue;
					if (trimmedLine == "") continue;
					ignoredFiles.Add(PerforcePlusIngore.CreateFileType(line));
				}
			}
			Debug.LogWarning("Loaded a total of " + ignoredFiles.Count + " ignored rulesets.");
		} else {
			Debug.LogWarning("Couldn't find a .unityIgnore");
		}
	}


	[MenuItem("Perforce+/Checkout .unityIgnore")]
	public static void CheckoutIgnore() {
		if (File.Exists("./.unityIgnore")) {
			Provider.Checkout(".unityIgnore", CheckoutMode.Exact).Wait();
		} else {
			File.Create("./.unityIgnore");
		}
	}
	

	[MenuItem("Perforce+/Reload .unityIgnore", true)]
	public static bool CheckIfIgnoreExists() {
		return File.Exists("./.unityIgnore");
	}


	static bool preSubmit(AssetList list, ref string changesetID, ref string changesetDescription) {
		ReloadIgnore();
		AssetList moveList = new AssetList();
		AssetList revertList = new AssetList();

		foreach (var asset in list) {
			if (asset == null) continue;
			if (moveList.Contains(asset) || revertList.Contains(asset)) continue;

			if (!File.Exists(asset.assetPath)) {
				moveList.Remove(asset);
			}
			
			foreach (var file in ignoredFiles) {
				if(file.CheckIfAssetIsIgnored(asset)) {
					if (asset.IsState(Asset.States.AddedLocal) && ! asset.IsOneOfStates(new []{Asset.States.LockedRemote, Asset.States.CheckedOutRemote, Asset.States.MovedRemote})) {
						revertList.Add(asset);
					} else {
						moveList.Add(asset);
					}
					break;
				}
			}
		}

		if(revertList.Count > 0) {
			foreach (var revert in revertList) {
				list.Remove(revert);
				
				if(Provider.RevertIsValid(revert, RevertMode.Normal)) {
					Provider.Revert(revert, RevertMode.Normal).Wait();
				}
			}
		}

		foreach (var a in moveList) {
			list.Remove(a);
		}

		return true;
	}
}