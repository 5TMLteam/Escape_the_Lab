using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /* public ���� */
    public static GameManager instance = null;          // GameManager�� �̱��� �������� ���� �� ����� ����
    public BoardManager boardScript;                    // ���������� ����� ������Ʈ
    public float levelStartDelay = 2f;                  // ������ ���۵Ǳ� ���� �ʴ����� ����� �ð�
    // Player�� ������
    public int playerFoodPoints = 100;                  // �÷��̾� ������
    [HideInInspector] public bool playersTurn = true;   // 
    // Enemy�� ������
    public float turnDelay = 0.1f;                      // �� �� ���� ���ð�

    /* private ���� */
    private int level = 1;                      // Enemy�� log(level)��ŭ �����Ƿ� level�� ������ 3���� ��
    private List<Enemy> enemies;                // ���������� ��� Enemy ������Ʈ�� ������ ����
    private bool enemiesMoving;                 //
    // UI�� ������
    private Text levelText;                     // ���� ���ڸ� ǥ���� �ؽ�Ʈ UI
    private GameObject levelImage;              // LevelImage UI�� ���۷���
    private bool doingSetup;                    // ���� ���带 ����� ������ Ȯ���ϴ� ����
    private GameObject restartButton;             // ���� ��ư UI
    private Text restartText;                     // ���� ��ư�� ���� �ؽ�Ʈ UI
    private GameObject exitButton;              // ���� ���� ��ư UI

    private HashSet<Vector2> occupiedPositions = new HashSet<Vector2>(); // ������ ���� �����ϰ� �ִ� ��ġ�� �����ϴ� HashSet

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
        InitGame();                                 // �������� ����
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
        instance.level++;
        if (instance.level == 1)
            instance.playerFoodPoints = 100;
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
        boardScript.SetupScene(level);//
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
    }

     // ��� Enemy�� �ѹ��� �̵��ϵ��� �ϴ� �ڷ�ƾ �Լ�
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        occupiedPositions.Clear();  // �� �ϸ��� ������ ��ġ �ʱ�ȭ

        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
            yield return new WaitForSeconds(turnDelay);

        for (int i = 0; i < enemies.Count; i++)
        {
            // ���� ���� ��ġ�� ���� �̵��� ��ġ ���
            Vector2 currentPosition = enemies[i].transform.position;
            Vector2 newPosition = enemies[i].GetNextMove();

            // �� ��ġ�� �ٸ� ���� ���� �������� �ʾҴٸ� �̵� ����
            if (!occupiedPositions.Contains(newPosition))
            {
                occupiedPositions.Remove(currentPosition);  // ���� ��ġ���� ����
                occupiedPositions.Add(newPosition);         // �� ��ġ �߰�
                enemies[i].MoveEnemy();                     // �� �̵� ����
            }
            // �׷��� ������ �̵����� ���� (���� ��ġ ����)

            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
    
    // Ư�� ��ġ�� ���� ���� �����Ǿ����� Ȯ���ϴ� �޼���
    public bool IsPositionOccupied(Vector2 position)
    {
        return occupiedPositions.Contains(position);
    }

    public void RestartGame()
    {
        enabled = true;
        instance.level = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single); // ���������� �ε�� Scene�� �ٽ� �ε���.
    }

}
