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

    public void SetData(Sprite cover, string title)
    {
        this.cover.sprite = cover;
        this.title.text = title;
    }

    public void PlayTheGameWithThisMusic()
    {
        GameData.musicName = title.text;
        SceneManager.LoadScene("Game");
    }
}
