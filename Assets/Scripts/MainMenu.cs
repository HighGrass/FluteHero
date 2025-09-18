using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private bool showingList = false;
    [SerializeField] private Animator musicList;
    [SerializeField] private Animator buttons;

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
}
