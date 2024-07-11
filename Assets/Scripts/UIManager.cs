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
    private GameObject gameoverImage;           // 게임종료시 이미지
    
    public void InitUI(int level)
    {
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        restartButton = GameObject.Find("RestartButton");
        restartText = GameObject.Find("RestartText").GetComponent<Text>();
        exitButton = GameObject.Find("ExitButton");
        gameoverImage = GameObject.Find("GameoverImage");

        levelText.text = "Floor " + level;
        levelImage.SetActive(true);
        restartButton.SetActive(false);
        exitButton.SetActive(false);
        gameoverImage.SetActive(false);
    }

    public void HideLevelImage()
    {
        levelImage.SetActive(false);
    }

    public void ShowGameOver(int level)
    {
        gameoverImage.SetActive(true);                                          // 게임 오버 이미지 보이기

        restartText.text = "RESTART";
        levelText.text = "After " + level + "floors, you failed.";              // 게임 오버 텍스트
        levelText.rectTransform.anchoredPosition = new Vector3(0f, -150f, 0f);   // 게임 오버 텍스트 위치 이동

        RectTransform rectTransform;
        rectTransform = restartButton.GetComponent<RectTransform>();            // 재시작 버튼 위치 이동
        rectTransform.anchoredPosition = new Vector3(0f, 60f, 0f);

        rectTransform = exitButton.GetComponent<RectTransform>();               // 종료 버튼 위치 이동
        rectTransform.anchoredPosition = Vector3.zero;

        levelImage.SetActive(true);
        restartButton.SetActive(true);
        exitButton.SetActive(true);
    }

}
