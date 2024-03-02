using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[,] prefabVec;
    public GameObject prefabToCreate;
    public int rows = 7;
    public int columns = 8;
    public enum SweetsType
    {
        EMPTY,
        GREEN,
        RED,
        YELLOW,
        BLUE,
        BLACK,
        COUNT//标记类型
    }

    public Dictionary<SweetsType, GameObject> sweetPrefabDict;
    [System.Serializable]
    public struct ElementPrefab{
        public GameObject elementObject;
        public SweetsType type;
    }
    public ElementPrefab[] elementPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        CreateBackground();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateBackground()
    {
        // 获取当前对象的Transform
        Transform myTransform = transform;
        prefabVec = new GameObject[rows, columns];
        for(int i=0;i<rows;i++)
        {
            for(int j=0;j<columns;j++)
            {
                prefabVec[i, j] = Instantiate(prefabToCreate, new Vector3(myTransform.position.x + i*2 - 0.5f, myTransform.position.y + j*2 - 0.5f, myTransform.position.z), Quaternion.identity);
            }
        }
    }
}
