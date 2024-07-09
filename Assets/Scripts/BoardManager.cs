using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
public class BoardManager : MonoBehaviour{
    [Serializable] //����ȭ
    public class Count{
        public int minimum;
        public int maximum;

        public Count (int min, int max){ 
            minimum = min;
            maximum = max;
        }
    }
    
    public int row =7;
    public int column = 7;
    //8*8
    public Count wallCount = new Count(6,10);
    public Count foodCount = new Count(0,2);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] enemyTiles;
    public GameObject[] foodTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] outerWallVert;
    public GameObject[] outerWallHorz;
    public GameObject[] outerWallEdgeTiles_new;
    public GameObject[] poisonTiles;

    private Transform boardHolder;//���� ������, ��� ������Ʈ�� �θ�
    private List <Vector3> gridPositions =  new List<Vector3>(); //������Ʈ  ��ġ ����
    private int formerFoodNum = -1;

    //gridPositon �ʱ�ȭ
    void InitializeList(){
        gridPositions.Clear();
        for(int x=1; x<column-1; x++){
            for(int y=1; y<row-1; y++){
                gridPositions.Add(new Vector3(x,y,0f));
            }
        }
    }

    //Floor, outerwall ���� 
    void BoardSetup(){
        boardHolder = new GameObject ("Board").transform;                                       // ��� ��� Object�� �θ�� �� ������Ʈ

        // Floor �����ϱ�
        for (int x = -1; x < column + 1; x++)
        {
            for (int y = -1; y < row + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                //if (x == -1 || x == column || y == -1 || y == row)
                //  toInstantiate = outerWallTiles[Random.Range (0,outerWallTiles.Length)];
                createInstance(toInstantiate, new Vector3(x, y, 1f));
            }
        }

        // �����ڸ� ������ �����ϱ�
        createInstance(outerWallEdgeTiles_new[3], new Vector3(-1, -1, 0f));
        createInstance(outerWallEdgeTiles_new[2], new Vector3(column, -1, 0f));
        createInstance(outerWallEdgeTiles_new[1], new Vector3(column, row, 0f));
        createInstance(outerWallEdgeTiles_new[0], new Vector3(-1, row, 0f));


        // ������ �Ա� �����ϱ�
        // ����
        int top = 0, bottom = outerWallVert.Length - 1;
        createInstance(outerWallVert[top], new Vector3(-1, row - 3, 0f));
        createInstance(outerWallVert[top], new Vector3(column, row - 3, 0f));
        createInstance(outerWallVert[bottom], new Vector3(-1, 2, 0f));
        createInstance(outerWallVert[bottom], new Vector3(column, 2, 0f));
        // ����
        top = 0; bottom = outerWallHorz.Length - 1;
        createInstance(outerWallHorz[bottom], new Vector3(column - 3, -1, 0f));
        createInstance(outerWallHorz[bottom], new Vector3(column - 3, row, 0f));
        createInstance(outerWallHorz[top], new Vector3(2, -1, 0f));
        createInstance(outerWallHorz[top], new Vector3(2, row, 0f));

        // ������ ���� �����ϱ�
        for (int i = 3; i < column - 3; i++)
        {
            createInstance(outerWallVert[Random.Range(1, outerWallVert.Length-1)], new Vector3(-1, i, 0f));
            createInstance(outerWallVert[Random.Range(1, outerWallVert.Length-1)], new Vector3(column, i, 0f));
            createInstance(outerWallHorz[Random.Range(1, outerWallHorz.Length-1)], new Vector3(i, -1, 0f));
            createInstance(outerWallHorz[Random.Range(1, outerWallHorz.Length-1)], new Vector3(i, column, 0f));
        }
    }
    void createInstance(GameObject toInstantiate, Vector3 position){
        GameObject instance = Instantiate(toInstantiate, position, Quaternion.identity);
        instance.transform.SetParent(boardHolder);
    }


        // ���� ��ġ
        Vector3 RandomPosition(){
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    //wall, food ���� ����
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimun, int maximum){
        int objectCount = Random.Range(minimun, maximum + 1);                       //�ּ�~�ִ�
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();                              
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];   
            Instantiate(tileChoice, randomPosition, Quaternion.identity);         
        }
    }

     // �� Ÿ���� 7x7 �ܰ��� ��ġ�ϴ� �޼ҵ�
    void LayoutPoisonTiles()
    {
        List<Vector3> poisonPositions = new List<Vector3>();

        // 7x7 ������ �ܰ� ��ǥ�� ����Ʈ�� �߰�
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                if (x == 0 || x == column - 1 || y == 0 || y == row - 1)
                {
                    Vector3 potentialPosition = new Vector3(x, y, 0f);
                    // ����, �ⱸ ��ġ�� ����
                    if (potentialPosition != new Vector3(column - 1, row - 1, 0f) && potentialPosition != new Vector3(0, 0, 0f) )
                    {
                        poisonPositions.Add(potentialPosition);
                    }
                }
            }
        }

    // �����ϰ� �� Ÿ�� ��ġ
    int poisonCount = Random.Range(1, 5); // 1���� 5�� ���̷� ���� ����
    for (int i = 0; i < poisonCount && poisonPositions.Count > 0; i++)
    {
        int index = Random.Range(0, poisonPositions.Count);
        Vector3 position = poisonPositions[index];
        GameObject tileChoice = poisonTiles[Random.Range(0, poisonTiles.Length)];
        Instantiate(tileChoice, new Vector3(position.x, position.y, 0f), Quaternion.identity);
        poisonPositions.RemoveAt(index);
    }
}


    //gameMnager�� ȣ��
     public void SetupScene(int level){
        BoardSetup();
        InitializeList();

        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        if (formerFoodNum == 0)
            formerFoodNum = 1;
        else
            formerFoodNum = foodCount.minimum;
        LayoutObjectAtRandom(foodTiles, formerFoodNum, foodCount.maximum);

        int enemyCount = (int)Mathf.Log(level, 2f);                     
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

         // �� Ÿ�� ��ġ
        LayoutPoisonTiles();

        // Exit ����
        Instantiate(exit, new Vector3(column - 1, row - 1, 0f), Quaternion.identity);
    }


}
