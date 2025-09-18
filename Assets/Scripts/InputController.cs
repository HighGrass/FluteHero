using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private FluteHeroGame gameController;

    public void OnConnectionEvent(bool connected)
    {
        if (connected) Debug.Log("Arduino connected");
        else Debug.Log("Arduino disconnected");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.A)) gameController.CheckNoteHit(5, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.S)) gameController.CheckNoteHit(4, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.D)) gameController.CheckNoteHit(3, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.J)) gameController.CheckNoteHit(2, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.K)) gameController.CheckNoteHit(1, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.L)) gameController.CheckNoteHit(0, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.M)) SceneManager.LoadScene("MainMenu");
    }

    public void OnMessageArrived(string message)
    {
        if (string.IsNullOrEmpty(message) || message.Length < 6)
            return;

        for (int i = 0; i < 6; i++)
        {
            char c = message[i];
            if (c == '1')
            {
                gameController.CheckNoteHit(i, gameController.GetCurrentMusicTime());
            }
            // else if (c == '2') Debug.Log($"Pin {i} RELEASED");

        }
    }

}
