using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /* public 변수 */
    public static GameManager instance = null;          // GameManager를 싱글톤 패턴으로 만들 때 사용할 변수
    public BoardManager boardScript;                    // 스테이지를 만드는 오브젝트
    public float levelStartDelay = 2f;                  // 레벨이 시작되기 전에 초단위로 대기할 시간
    // Player용 변수들
    public int playerFoodPoints = 100;                  // 플레이어 포만감
    [HideInInspector] public bool playersTurn = true;   // 
    // Enemy용 변수들
    public float turnDelay = 0.1f;                      // 각 턴 사이 대기시간

    /* private 변수 */
    private int level = 1;                      // Enemy가 log(level)만큼 나오므로 level의 시작을 3부터 함
    private List<Enemy> enemies;                // 스테이지의 모든 Enemy 오브젝트들 저장한 변수
    private bool enemiesMoving;                 //
    // UI용 변수들
    private Text levelText;                     // 레벨 숫자를 표시할 텍스트 UI
    private GameObject levelImage;              // LevelImage UI의 레퍼런스
    private bool doingSetup;                    // 게임 보드를 만드는 중인지 확인하는 변수
    private GameObject restartButton;             // 시작 버튼 UI
    private Text restartText;                     // 시작 버튼에 들어가는 텍스트 UI
    private GameObject exitButton;              // 게임 종료 버튼 UI

    private HashSet<Vector2> occupiedPositions = new HashSet<Vector2>(); // 적들이 현재 점유하고 있는 위치를 추적하는 HashSet

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
        InitGame();                                 // 스테이지 생성
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
        instance.level++;
        if (instance.level == 1)
            instance.playerFoodPoints = 100;
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
        boardScript.SetupScene(level);//
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
    }

     // 모든 Enemy가 한번에 이동하도록 하는 코루틴 함수
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        occupiedPositions.Clear();  // 각 턴마다 점유된 위치 초기화

        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
            yield return new WaitForSeconds(turnDelay);

        for (int i = 0; i < enemies.Count; i++)
        {
            // 현재 적의 위치와 다음 이동할 위치 계산
            Vector2 currentPosition = enemies[i].transform.position;
            Vector2 newPosition = enemies[i].GetNextMove();

            // 새 위치가 다른 적에 의해 점유되지 않았다면 이동 실행
            if (!occupiedPositions.Contains(newPosition))
            {
                occupiedPositions.Remove(currentPosition);  // 현재 위치에서 제거
                occupiedPositions.Add(newPosition);         // 새 위치 추가
                enemies[i].MoveEnemy();                     // 적 이동 실행
            }
            // 그렇지 않으면 이동하지 않음 (현재 위치 유지)

            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
    
    // 특정 위치가 적에 의해 점유되었는지 확인하는 메서드
    public bool IsPositionOccupied(Vector2 position)
    {
        return occupiedPositions.Contains(position);
    }

    public void RestartGame()
    {
        enabled = true;
        instance.level = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single); // 마지막으로 로드된 Scene을 다시 로드함.
    }

}
