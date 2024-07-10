using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI용 변수들
    private Text levelText;                     // 레벨 숫자를 표시할 텍스트 UI
    private GameObject levelImage;              // LevelImage UI의 레퍼런스
    
    private GameObject restartButton;           // 시작 버튼 UI
    private Text restartText;                   // 시작 버튼에 들어가는 텍스트 UI
    private GameObject exitButton;              // 게임 종료 버튼 UI
    
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
        levelText.text = "After " + level + "floors, you failed.";   // 게임 오버 텍스트

        levelImage.SetActive(true);
        restartButton.SetActive(true);
        exitButton.SetActive(true);
    }

}
