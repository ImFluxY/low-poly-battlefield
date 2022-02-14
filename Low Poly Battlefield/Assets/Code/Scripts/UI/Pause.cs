using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Pause : MonoBehaviour
{
    public static bool paused = false;
    private bool disconnected = false;

    private void Update()
    {
        if (InputManager.Instance.Pause)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        Debug.Log("Toggle Pause");

        if (disconnected) return;

        paused = !paused;

        transform.GetChild(0).gameObject.SetActive(paused);
        if (paused) CursorController.Instance.EnableCursor(); else CursorController.Instance.DisableCursor();
    }

    public void Quit()
    {
        disconnected = true;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
