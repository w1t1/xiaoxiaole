using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml.Linq;

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
    public Element[,] elements;
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
        elements = new Element[columns, rows + 1];
        CreateBackground();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateBackground()
    {
        //elementPrefabs = new GameObject[rows, columns];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows + 1; j++)
            {
                Instantiate(prefabToCreate, new Vector3(transform.position.x + i * 2 - 0.5f, transform.position.y + j * 2 - 0.5f, transform.position.z), Quaternion.identity);
            }
        }
        //for (int j = 0; j < rows + 1; j++)
        //{

        //    for (int i = 0; i < columns; i++)
        //    {
        //        Element newElement = CreateNewElement(i, rows, ElementType.Normal);
        //        newElement.textureComponent.SetTexture((TextureComponent.TextureType)UnityEngine.Random.Range(0, 2));
        //    }
        //}
        StartCoroutine(AllFill());
    }

    public Vector3 GenElementPos(int col, int row)
    {
        return new Vector3(transform.position.x + col * 2 - 0.5f, transform.position.y + row * 2 - 0.5f, transform.position.z - 0.5f);
    }

    public Element CreateNewElement(int col, int row, ElementType type)
    {
        GameObject newElement = Instantiate(elementPrefabDict[type], GenElementPos(col, row), Quaternion.identity);

        //newElement.GetComponent<TextureComponent>().SetTexture((TextureComponent.TextureType)UnityEngine.Random.Range(0, 2));
        elements[col, row] = newElement.GetComponent<Element>();
        elements[col, row].Init(col, row, this, type);
        return elements[col, row];
    }

    public bool Fill()
    {
        bool notFinish = false;
        for (int j = 1; j < rows; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                Element element = elements[i, j];
                if (element == null)
                {
                    continue;
                }
                Element downElement = elements[i, j - 1];
                if (downElement == null)
                {
                    elements[i, j - 1] = element;
                    elements[i, j] = null;
                    element.moveComponent.move(i, j - 1, 0.5f);
                    notFinish = true;
                }
            }
        }
        for (int i = 0; i < columns; i++)
        {
            Element element = elements[i, rows];
            if (element == null)
            {
                element = CreateNewElement(i, rows, ElementType.Normal);
                element.textureComponent.SetTexture((TextureComponent.TextureType)UnityEngine.Random.Range(0, 2));
            }
            Element downElement = elements[i, rows - 1];
            if (downElement == null)
            {
                elements[i, rows - 1] = element;
                elements[i, rows] = null;
                element.moveComponent.move(i, rows - 1, 0.5f);
                notFinish = true;
            }
        }
        return notFinish;
    }

    public IEnumerator AllFill()
    {
        //bool needFill = true;
        //while (needFill)
        //{
        yield return new WaitForSeconds(0.5f);

        //}
        while (Fill())
        {
            yield return new WaitForSeconds(0.5f);

        }
    }
}
