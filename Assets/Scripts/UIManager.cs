using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI�� ������
    private Text levelText;                     // ���� ���ڸ� ǥ���� �ؽ�Ʈ UI
    private GameObject levelImage;              // LevelImage UI�� ���۷���
    
    private GameObject restartButton;           // ���� ��ư UI
    private Text restartText;                   // ���� ��ư�� ���� �ؽ�Ʈ UI
    private GameObject exitButton;              // ���� ���� ��ư UI
    
    public void InitUI(int level)
    {
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        restartButton = GameObject.Find("RestartButton");
        restartText = GameObject.Find("RestartText").GetComponent<Text>();
        exitButton = GameObject.Find("ExitButton");

        levelText.text = "Floor " + level;
        levelImage.SetActive(true);
        restartButton.SetActive(false);
        exitButton.SetActive(false);
    }

    public void HideLevelImage()
    {
        levelImage.SetActive(false);
    }

    public void ShowGameOver(int level)
    {
        restartText.text = "RESTART";
        levelText.text = "After " + level + "floors, you failed.";   // ���� ���� �ؽ�Ʈ

        levelImage.SetActive(true);
        restartButton.SetActive(true);
        exitButton.SetActive(true);
    }

}
