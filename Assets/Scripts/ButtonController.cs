using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public void OnStartButtonClick()
    {
        GameManager.instance.RestartGame();
    }
    public void OnExitButtonClick()
    {
        Application.Quit();
    }
}
