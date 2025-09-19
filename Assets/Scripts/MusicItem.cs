using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicItem : MonoBehaviour
{
    [SerializeField]
    private Image cover;
    [SerializeField]
    private TextMeshProUGUI title;

    private string musicName;

    public void SetData(Sprite cover, string title)
    {
        this.cover.sprite = cover;
        musicName = title;
        this.title.text = title != null ? title.Replace('_', ' ') : "Unknown Title";
    }

    public void PlayTheGameWithThisMusic()
    {
        GameData.musicName = musicName;
        SceneManager.LoadScene("Game");
    }
}
