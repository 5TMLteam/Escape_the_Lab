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

    /* public ���� */
    public List<int> testScores;
    public static GameManager instance = null;          // GameManager�� �̱��� �������� ���� �� ����� ����
    public BoardManager boardScript;                    // ���������� ����� ������Ʈ
    public float levelStartDelay = 2f;                  // ������ ���۵Ǳ� ���� �ʴ����� ����� �ð�
    // Player�� ������
    public int playerFoodPoints = 100;                  // �÷��̾� ������
    [HideInInspector] public bool playersTurn = true;   // 
    // Enemy�� ������
    public float turnDelay = 0.1f;                      // �� �� ���� ���ð�

    /* private ���� */
    private int level = 0;                      // 
    private List<Enemy> enemies;                // ���������� ��� Enemy ������Ʈ�� ������ ����
    private bool enemiesMoving;                 //
    private bool isInitialized = false;         // InitGame() �Լ��� ȣ��Ǿ����� Ȯ���ϴ� �Լ�
    // UI�� ������
    private Text levelText;                     // ���� ���ڸ� ǥ���� �ؽ�Ʈ UI
    private GameObject levelImage;              // LevelImage UI�� ���۷���
    private bool doingSetup;                    // ���� ���带 ����� ������ Ȯ���ϴ� ����
    private GameObject restartButton;           // ���� ��ư UI
    private Text restartText;                   // ���� ��ư�� ���� �ؽ�Ʈ UI
    private GameObject exitButton;              // ���� ���� ��ư UI

    /* ����Ƽ API �Լ��� */
    void Awake()
    {
        // �� ���� GameManager�� ������ �ʰ� �ϱ�
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);              // ���� Scene���� �Ѿ�� GameManager�� �������� �ʰ� �ϱ�

        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        playerFoodPoints = 100;
        if (SceneManager.GetActiveScene().name == "MainScene")      // TitleScene�� ��ġ�� �ʰ� �����Ѵٸ�
        {
            instance.level = 1;
            InitGame();                                             // �������� ����
        }
    }

    /*// �� ������ �ε�� ������ InitGame �Լ� ȣ���ϴ� �Լ�
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

    // Scene �Ѿ�� �� ���̴� �Լ���
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (instance.isInitialized)         // ���� �̹� InitGame()�� ����Ǿ��ٸ�
            return;                             // InitGame() �������� �ʰ� �Ѿ��
        if (instance.level == 0)            // ���� ������� ���̶��
            instance.playerFoodPoints = 100;    // �÷��̾� ü���� 100���� �����

        instance.level++;
        instance.InitGame();
    }

    /* private �Լ��� */
    // BoardManager�� ���� ���������� �����ϴ� �Լ�
    void InitGame()
    {
        doingSetup = true;                                              // �÷��̾ �� �ε�� ���� �� �����̰� �ϱ�

        // UI ����
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        restartButton = GameObject.Find("RestartButton");
        restartText = GameObject.Find("RestartText").GetComponent<Text>();
        exitButton = GameObject.Find("ExitButton");

        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        restartButton.SetActive(false);
        exitButton.SetActive(false);
        Invoke("HideLevelImage", levelStartDelay);                      // levelStartDelay��ŭ ��ٸ��� ���� ���� ����

        enemies.Clear();
        boardScript.SetupScene(level);
        isInitialized = true;
    }

    // ������ �� �ε�Ǹ� LevelImage UI ���� �Լ�
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
    
    /* public �Լ��� */
    // �� Enemy ������Ʈ�� �ڽ��� �Ű������� ȣ���Ͽ� enemies ����Ʈ�� �ִ� �Լ�
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
    // ���� ������ Player�� ���ؼ� ȣ��Ǿ� GameManager�� ��Ȱ��ȭ��Ű�� �Լ�
    public void GameOver()
    {
        restartText.text = "RESTART";
        levelText.text = "After " + level + "days, you starved.";   // ���� ���� �ؽ�Ʈ

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

    // ��� Enemy�� �ѹ��� �̵��ϵ��� �ϴ� �ڷ�ƾ �Լ�
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
    // ���� ������ �Ѿ�� �Լ�, Player�� ȣ����
    public void NextLevel()
    {
        isInitialized = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single); // ���������� �ε�� Scene�� �ٽ� �ε���.
    }
    // ���� 1���� ������ϴ� �Լ�
    public void RestartGame()
    {
        enabled = true;     // ���� GameManager ������Ʈ �ٽ� Ȱ��ȭ
        instance.level = 0; // ���� �ʱ�ȭ

        NextLevel();        // Scene ���� �ҷ�����
    }

}
