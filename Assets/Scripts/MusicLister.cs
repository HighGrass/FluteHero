using System.IO;
using UnityEngine;

public class MusicLister : MonoBehaviour
{
    [SerializeField] private MusicItem itemPrefab;

    public void List()
    {

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (transform.GetChild(i).TryGetComponent<MusicItem>(out var item))
                Destroy(item.gameObject);
        }

        string musicFolderPath = Path.Combine(Application.streamingAssetsPath, "Musics");
        string[] folderPaths = null;

        if (Directory.Exists(musicFolderPath))
        {
            folderPaths = Directory.GetDirectories(musicFolderPath);
        }

        if (folderPaths == null || folderPaths.Length == 0)
        {
            Debug.LogWarning("No music folders found in StreamingAssets/Musics");
            return;
        }

        foreach (string folderPath in folderPaths)
        {
            string musicName = Path.GetFileName(folderPath);

            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(folderPath, "assetbundle"));
            if (assetBundle == null)
            {
                Debug.LogWarning($"Failed to load AssetBundle at {Path.Combine(folderPath, "assetbundle")}");
                continue;
            }

            Sprite cover = assetBundle.LoadAsset<Sprite>("assets/cover.png");
            if (cover == null)
            {
                cover = assetBundle.LoadAsset<Sprite>("assets/cover.jpg");
            }
            MusicItem itemObj = Instantiate(itemPrefab.gameObject, transform).GetComponent<MusicItem>();
            itemObj.transform.SetSiblingIndex(0);

            itemObj.SetData(cover, musicName);

            assetBundle.Unload(false);
        }

    }
}
