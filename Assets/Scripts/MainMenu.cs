using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private bool showingList = false;
    private bool showingOptions = false;
    [SerializeField] private Animator musicList;
    [SerializeField] private Animator buttons;
    [SerializeField] private Animator optionsMenu;

    private MusicLister musicLister;

    private void Start()
    {
        musicLister = musicList.GetComponent<MusicLister>();
        if (musicLister == null)
        {
            Debug.LogError("MusicLister component not found on MainMenu GameObject");
        }
        else
            musicLister.List();

        musicList.Play("Hidden");
        buttons.Play("Shown");
    }

    public void ToggleMusicList()
    {
        showingList = !showingList;
        if (showingList)
        {
            musicLister.List();
            musicList.SetTrigger("Show");
            buttons.SetTrigger("Hide");
        }
        else
        {
            musicList.SetTrigger("Hide");
            buttons.SetTrigger("Show");
        }
    }

    public void ToggleOptions()
    {
        showingOptions = !showingOptions;
        if (showingOptions)
        {
            optionsMenu.SetTrigger("Show");
            buttons.SetTrigger("Hide");
        }
        else
        {
            optionsMenu.SetTrigger("Hide");
            buttons.SetTrigger("Show");
        }
    }

    public void QuitGame()
    {
        if (Application.isEditor)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }
}
