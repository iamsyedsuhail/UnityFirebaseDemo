using System.IO;
using UnityEditor;
using UnityEngine;

namespace IzanamiWorkshop.Editor
{
	public class AssetBundleCreator : MonoBehaviour
	{

		[MenuItem("Assets/Build AssetBundles")]
		private static void BuildAllAssetBundles()
		{
			string outputPath = Application.streamingAssetsPath + "/Bundles";
			AssetBundleBuild[] assetBundleBuilds = GatherAssets();

			BuildPipeline.BuildAssetBundles(outputPath, assetBundleBuilds, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
		}

		private static AssetBundleBuild[] GatherAssets()
		{
			string mainFolder = "Assets/LocalAssets/";
			string rawAssetsDataPath = Path.Combine(Application.dataPath, "LocalAssets");
			string[] assetBundleNameListPath = GatherSubFolderPaths(rawAssetsDataPath);
			string[] subFolderNames = GatherSubFolderNames(assetBundleNameListPath);

			AssetBundleBuild[] assetBundleBuildMap = new AssetBundleBuild[assetBundleNameListPath.Length];

			for (int i = 0; i < assetBundleBuildMap.Length; i++)
			{
				string bundleName = subFolderNames[i];
				assetBundleBuildMap[i].assetBundleName = bundleName;
				string rawAssetPath = Path.Combine(rawAssetsDataPath, assetBundleNameListPath[i]);
				string[] assetPaths = GatherAssetPathFromSubFolders(rawAssetPath);
				string[] correctedAssetPaths = MapAssetPathToUnityHierarchy(mainFolder, subFolderNames[i], assetPaths);
				assetBundleBuildMap[i].assetNames = correctedAssetPaths;
			}


			return assetBundleBuildMap;
		}

		private static string[] GatherSubFolderPaths(string path)
		{
			string[] folderList = Directory.GetDirectories(path);
			return folderList;
		}

		private static string[] GatherSubFolderNames(string[] path)
		{
			string[] folderList = new string[path.Length];
			for (int i = 0; i < folderList.Length; i++)
			{
				folderList[i] = Path.GetFileName(path[i]);
			}
			return folderList;
		}

		//maps raw path to to be compatible with unity/assets path
		private static string[] MapAssetPathToUnityHierarchy(string mainFolder, string subfolder, string[] assetNames)
		{
			string[] correctedPath = new string[assetNames.Length];
			for (int i = 0; i < assetNames.Length; i++)
			{
				correctedPath[i] = mainFolder + subfolder + "/" + Path.GetFileName(assetNames[i]);

			}
			return correctedPath;
		}

		private static string[] GatherAssetPathFromSubFolders(string path)
		{
			string[] fileList = Directory.GetFiles(path);
			return fileList;
		}
	}
}
