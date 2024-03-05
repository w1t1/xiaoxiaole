using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[,] elementPrefabs;
    public GameObject prefabToCreate;
    public int rows = 7;
    public int columns = 8;
    public enum ElementType
    {
        EMPTY,
        Normal,
        RED,
        YELLOW,
        BLUE,
        BLACK,
        COUNT//标记类型
    }

    [System.Serializable]
    public struct ElementPrefab
    {
        public GameObject elementObject;
        public ElementType type;
    }
    public ElementPrefab[] elementTypePrefabs;
    public Dictionary<ElementType, GameObject> elementPrefabDict;
    // Start is called before the first frame update
    void Start()
    {
        elementPrefabDict = new Dictionary<ElementType, GameObject>();
        for (int i = 0; i < elementTypePrefabs.Length; i++)
        {
            if (!elementPrefabDict.ContainsKey(elementTypePrefabs[i].type))
            {
                elementPrefabDict.Add(elementTypePrefabs[i].type, elementTypePrefabs[i].elementObject);
            }
        }
        CreateBackground();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateBackground()
    {
        //elementPrefabs = new GameObject[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Instantiate(prefabToCreate, new Vector3(transform.position.x + i * 2 - 0.5f, transform.position.y + j * 2 - 0.5f, transform.position.z), Quaternion.identity);

                    GameObject x = Instantiate(elementPrefabDict[ElementType.Normal], GenElementPos(i, j), Quaternion.identity);
                    x.GetComponent<textureComponent>().SetTexture((textureComponent.TextureType)UnityEngine.Random.Range(0, 2));
                //elementPrefabs[i][j] = Instantiate(prefabToCreate, GenElementPos(i, j), Quaternion.identity);
            }
        }
    }

    public Vector3 GenElementPos(int col, int row)
    {
        return new Vector3(transform.position.x + col * 2 - 0.5f, transform.position.y + row * 2 - 0.5f, transform.position.z - 0.5f);
    }
}
