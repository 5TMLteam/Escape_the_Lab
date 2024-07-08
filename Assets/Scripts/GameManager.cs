using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct Score
{
    public int level;
    public string name;
}


public class GameManager : MonoBehaviour
{
    public static List<Score> scores = null;

    /* public 변수 */
    public List<int> testScores;
    public static GameManager instance = null;          // GameManager를 싱글톤 패턴으로 만들 때 사용할 변수
    public BoardManager boardScript;                    // 스테이지를 만드는 오브젝트
    public float levelStartDelay = 2f;                  // 레벨이 시작되기 전에 초단위로 대기할 시간
    // Player용 변수들
    public int playerFoodPoints = 100;                  // 플레이어 포만감
    [HideInInspector] public bool playersTurn = true;   // 
    // Enemy용 변수들
    public float turnDelay = 0.1f;                      // 각 턴 사이 대기시간

    /* private 변수 */
    private int level = 0;                      // 
    private List<Enemy> enemies;                // 스테이지의 모든 Enemy 오브젝트들 저장한 변수
    private bool enemiesMoving;                 //
    private bool isInitialized = false;         // InitGame() 함수가 호출되었는지 확인하는 함수
    // UI용 변수들
    private Text levelText;                     // 레벨 숫자를 표시할 텍스트 UI
    private GameObject levelImage;              // LevelImage UI의 레퍼런스
    private bool doingSetup;                    // 게임 보드를 만드는 중인지 확인하는 변수
    private GameObject restartButton;           // 시작 버튼 UI
    private Text restartText;                   // 시작 버튼에 들어가는 텍스트 UI
    private GameObject exitButton;              // 게임 종료 버튼 UI

    /* 유니티 API 함수들 */
    void Awake()
    {
        // 두 개의 GameManager가 생기지 않게 하기
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);              // 다음 Scene으로 넘어가도 GameManager가 삭제되지 않게 하기

        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        playerFoodPoints = 100;
        if (SceneManager.GetActiveScene().name == "MainScene")      // TitleScene을 거치지 않고 실행한다면
        {
            instance.level = 1;
            InitGame();                                             // 스테이지 생성
        }
    }

    /*// 새 레벨이 로드될 때마다 InitGame 함수 호출하는 함수
    private void OnLevelWasLoaded(int index)
    {
        level++;

        InitGame();

    }*/
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    // Scene 넘어가는 데 쓰이는 함수들
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (instance.isInitialized)         // 만약 이미 InitGame()이 실행되었다면
            return;                             // InitGame() 실행하지 않고 넘어가기
        if (instance.level == 0)            // 만약 재시작한 것이라면
            instance.playerFoodPoints = 100;    // 플레이어 체력을 100으로 만들기

        instance.level++;
        instance.InitGame();
    }

    /* private 함수들 */
    // BoardManager를 통해 스테이지를 생성하는 함수
    void InitGame()
    {
        doingSetup = true;                                              // 플레이어가 맵 로드될 동안 못 움직이게 하기

        // UI 띄우기
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        restartButton = GameObject.Find("RestartButton");
        restartText = GameObject.Find("RestartText").GetComponent<Text>();
        exitButton = GameObject.Find("ExitButton");

        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        restartButton.SetActive(false);
        exitButton.SetActive(false);
        Invoke("HideLevelImage", levelStartDelay);                      // levelStartDelay만큼 기다리고 다음 레벨 시작

        enemies.Clear();
        boardScript.SetupScene(level);
        isInitialized = true;
    }

    // 레벨이 다 로드되면 LevelImage UI 끄는 함수
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
    
    /* public 함수들 */
    // 각 Enemy 오브젝트가 자신을 매개변수로 호출하여 enemies 리스트에 넣는 함수
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
    // 게임 오버시 Player에 의해서 호출되어 GameManager를 비활성화시키는 함수
    public void GameOver()
    {
        restartText.text = "RESTART";
        levelText.text = "After " + level + "days, you starved.";   // 게임 오버 텍스트

        levelImage.SetActive(true);
        restartButton.SetActive(true);
        exitButton.SetActive(true);

        enabled = false;

        scores.Add(level);
        scores.Sort();
        if (scores.Count > 5)
            scores.RemoveAt(0);
        testScores = scores;
    }

    // 모든 Enemy가 한번에 이동하도록 하는 코루틴 함수
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
            yield return new WaitForSeconds(turnDelay);

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
    // 다음 레벨로 넘어가는 함수, Player가 호출함
    public void NextLevel()
    {
        isInitialized = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single); // 마지막으로 로드된 Scene을 다시 로드함.
    }
    // 레벨 1부터 재시작하는 함수
    public void RestartGame()
    {
        enabled = true;     // 꺼진 GameManager 오브젝트 다시 활성화
        instance.level = 0; // 레벨 초기화

        NextLevel();        // Scene 새로 불러오기
    }

}
