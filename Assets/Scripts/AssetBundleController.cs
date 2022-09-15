using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace IzanamiWorkshop.Firebase.GameComponents
{
	public class AssetBundleController : MonoBehaviour
	{
		public static AssetBundleController Instance;

		[SerializeField] private bool clearCache;

        private void Awake()
        {
			Instance = this;
		}

        // Start is called before the first frame update
        void Start()
		{
			if (clearCache)
				Caching.ClearCache();
		}

		public void DownloadAssetsFromFirebase(string url)
		{
			StopCoroutine(IDownloadAssetsFromFirebase(url));
			StartCoroutine(IDownloadAssetsFromFirebase(url));
		}

		private IEnumerator IDownloadAssetsFromFirebase(string url)
		{
			using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url))
			{
				yield return uwr.SendWebRequest();

				if (uwr.result != UnityWebRequest.Result.Success)
				{
					Debug.Log(uwr.error);
				}
				else
				{
					// Get downloaded asset bundle
					Debug.Log("Bundle Download Success");
					AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
					HandleSuccess(bundle);
				}
			}
		}

		private void LocalAsset()
		{
			var assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "Bundles/prefabs"));
			if (assetBundle == null)
			{
				Debug.Log("Failed to load AssetBundle!");
				return;
			}

			GameObject go = assetBundle.LoadAsset<GameObject>("Cube");
			Instantiate(go);

			//unload bundle if there is no more reference to be used from
			assetBundle.Unload(false);
		}

		private void HandleSuccess(AssetBundle assetBundle)
		{
			//we just want to load cube prefab from bundle and instantiate.
			GameObject go = assetBundle.LoadAsset("Cube") as GameObject;
			Instantiate(go);
			Debug.Log("Instanted  Cube");

			//unload bundle if there is no more reference to be used from
			assetBundle.Unload(false);
		}
	}
}
