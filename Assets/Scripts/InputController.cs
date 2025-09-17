using System;
using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Alpha1)) gameController.CheckNoteHit(0, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.Alpha2)) gameController.CheckNoteHit(1, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.Alpha3)) gameController.CheckNoteHit(2, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.Alpha4)) gameController.CheckNoteHit(3, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.Alpha5)) gameController.CheckNoteHit(4, gameController.GetCurrentMusicTime());
        if (Input.GetKeyDown(KeyCode.Alpha6)) gameController.CheckNoteHit(5, gameController.GetCurrentMusicTime());
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
                Debug.Log($"Pin {i} PRESSED");
                gameController.CheckNoteHit(i, gameController.GetCurrentMusicTime());
            }
            else if (c == '2') Debug.Log($"Pin {i} RELEASED");

        }
    }

}
