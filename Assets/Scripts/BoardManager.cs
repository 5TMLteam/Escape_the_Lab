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
    
    public int row =8;
    public int column = 8;
    //8*8
    public Count wallCount = new Count(6,10);
    public Count foodCount = new Count(0,2);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] enemyTiles;
    public GameObject[] foodTiles;
    public GameObject[] outerWallTiles;
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
        boardHolder = new GameObject ("Board").transform;
        for(int x=-1; x<column+1; x++){

            for(int y=-1; y<row + 1; y++){
                GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
                if (x == -1 || x==column || y==-1 || y == row)
                    toInstantiate = outerWallTiles[Random.Range (0,outerWallTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
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
        for (int x = -1; x < 7; x++)
        {
            for (int y = -1; y < 7; y++)
            {
                if (x == -1 || x == 6 || y == -1 || y == 6)
                {
                    Vector3 potentialPosition = new Vector3(x + 1, y + 1, 0f);
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
