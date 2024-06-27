using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    /* MainScene ��ư�� �Լ��� */
    public void OnRestartButtonClick()
    {
        GameManager.instance.RestartGame();
    }
    public void OnExitButtonClick()
    {
        Debug.Log("exit");
        Application.Quit();
    }

    /* TitleScene ��ư�� �Լ��� */
    public void OnStartButtonClick()
    {
        Debug.Log("start");
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    public void OnScoreButtonClick()
    {
        Debug.Log("score");
    }
}
