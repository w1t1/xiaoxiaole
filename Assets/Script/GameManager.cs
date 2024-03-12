using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml.Linq;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public GameObject[,] elementPrefabs;
    public GameObject prefabToCreate;
    public int rows = 5;
    public int columns = 7;
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
    public Element startElements;
    public Element endElements;
    public Text scoreText;
    public Text addScoreText;
    public int currentScore = 0;
    public int addScore = 0;
    public float addScoreTime = 0.0f;
    public GameObject selection;
    bool isMoving = false;
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
        if (addScore > 0)
        {
            addScoreText.gameObject.SetActive(true);
            if (addScoreTime > 0.2f)
            {
                currentScore++;
                addScore--;
                scoreText.text = currentScore.ToString();
                addScoreText.text = "+" + addScore.ToString();
                addScoreTime = 0.0f;
            }
            else
            {
                addScoreTime += Time.deltaTime;
            }
        }
        else
        {
            addScoreText.gameObject.SetActive(false);
        }
    }

    void CreateBackground()
    {
        //elementPrefabs = new GameObject[rows, columns];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows + 1; j++)
            {
                Instantiate(prefabToCreate, GenElementPos(i, j, 0.0f), Quaternion.identity);
            }
        }
        StartCoroutine(AllFill());
    }

    public Vector3 GenElementPos(int col, int row, float depth)
    {
        return new Vector3(transform.position.x + col * 2.01f - 3.0f, transform.position.y + row * 2.1f - 1.0f, transform.position.z - depth);
    }

    public Element CreateNewElement(int col, int row, ElementType type)
    {
        GameObject newElement = Instantiate(elementPrefabDict[type], GenElementPos(col, row, 0.5f), Quaternion.identity);

        newElement.transform.parent = transform;
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
                element.textureComponent.SetTexture((TextureComponent.TextureType)UnityEngine.Random.Range(0, element.textureComponent.GetTypeCount()));
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

        //}
        bool needFill = true;
        while (needFill)
        {
            yield return new WaitForSeconds(0.5f);

            while (Fill())
            {
                yield return new WaitForSeconds(0.5f);

            }

            needFill = ClearAllMatchSweet();
        }
    }

    public bool IsAdjoin(Element element1, Element element2)
    {
        if (element1 == null || element2 == null)
        {
            return false;
        }

        if (Math.Abs(element1.X - element2.X) == 1 && Math.Abs(element1.Y - element2.Y) == 0)
        {
            return true;
        }

        if (Math.Abs(element1.X - element2.X) == 0 && Math.Abs(element1.Y - element2.Y) == 1)
        {
            return true;
        }
        return false;
    }

    public void StartMoving(Element element)
    {
        startElements = element;
        isMoving = true;
        selection.SetActive(true);
        selection.transform.position = element.transform.position;
    }

    public void MouseMoving(Element element)
    {
        if (isMoving)
        {
            if (IsAdjoin(startElements, element))
            {
                selection.SetActive(false);
                endElements = element;
                StartCoroutine(TryToSwapElement(startElements, element));
                isMoving = false;
            }
        }
    }

    public void EndMoving()
    {
        selection.SetActive(false);
        isMoving = false;
    }

    public IEnumerator TryToSwapElement(Element element1, Element element2)
    {
        int x1 = element1 == null ? 0 : element1.X;
        int y1 = element1 == null ? 0 : element1.Y;
        int x2 = element2 == null ? 0 : element2.X;
        int y2 = element2 == null ? 0 : element2.Y;
        elements[x2, y2] = element1;
        elements[x1, y1] = element2;
        element1.moveComponent.move(x2, y2, 0.15f);
        element2.moveComponent.move(x1, y1, 0.15f);
        List<Element> match1 = GetMatchSweet(element1);
        List<Element> match2 = GetMatchSweet(element2);
        yield return new WaitForSeconds(0.2f);
        if (match1 != null || match2 != null)
        {
            if (match1 != null)
            {
                HashSet<Element> set = new HashSet<Element>(match1);
                addScore += set.Count;
                foreach (var item in set)
                {
                    elements[item.X, item.Y] = null;
                    item.clearComponent.Clear();
                }
            }

            if (match2 != null)
            {
                HashSet<Element> set = new HashSet<Element>(match2);
                addScore += set.Count;
                foreach (var item in set)
                {
                    elements[item.X, item.Y] = null;
                    item.clearComponent.Clear();
                }
            }
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(AllFill());
        }
        else
        {
            elements[x1, y1] = element1;
            elements[x2, y2] = element2;
            element1.moveComponent.move(x1, y1, 0.15f);
            element2.moveComponent.move(x2, y2, 0.15f);
        }
    }

    public List<Element> GetMatchSweet(Element element)
    {
        List<Element> finalMatchElements = new List<Element>();
        List<Element> columnMatchElements = new List<Element>
        {
            element
        };
        List<Element> rowMatchElements = new List<Element>
        {
            element
        };
        for (int rowDistance = 1; rowDistance < columns; rowDistance++)
        {
            int x = element.X + rowDistance;
            if (x >= columns)
            {
                break;
            }
            if (element.textureComponent.textureType == elements[x, element.Y].textureComponent.textureType)
            {
                rowMatchElements.Add(elements[x, element.Y]);
            }
            else
            {
                break;
            }
        }

        for (int rowDistance = 1; rowDistance < columns; rowDistance++)
        {
            int x = element.X - rowDistance;
            if (x < 0)
            {
                break;
            }
            if (element.textureComponent.textureType == elements[x, element.Y].textureComponent.textureType)
            {
                rowMatchElements.Add(elements[x, element.Y]);
            }
            else
            {
                break;
            }
        }

        if (rowMatchElements.Count >= 3)
        {
            for (int i = 0; i < rowMatchElements.Count; i++)
            {
                finalMatchElements.Add(rowMatchElements[i]);
                for (int columnDistance = 1; columnDistance < rows; columnDistance++)
                {
                    int y = element.Y + columnDistance;
                    if (y > rows)
                    {
                        break;
                    }
                    if (element.textureComponent.textureType == elements[rowMatchElements[i].X, y].textureComponent.textureType)
                    {
                        columnMatchElements.Add(elements[rowMatchElements[i].X, y]);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int columnDistance = 1; columnDistance < rows; columnDistance++)
                {
                    int y = element.Y - columnDistance;
                    if (y < 0)
                    {
                        break;
                    }
                    if (element.textureComponent.textureType == elements[rowMatchElements[i].X, y].textureComponent.textureType)
                    {
                        columnMatchElements.Add(elements[rowMatchElements[i].X, y]);
                    }
                    else
                    {
                        break;
                    }
                }

                if (columnMatchElements.Count >= 3)
                {
                    for (int j = 0; j < columnMatchElements.Count; j++)
                    {
                        finalMatchElements.Add(columnMatchElements[j]);
                    }
                }
                columnMatchElements.Clear();
            }
        }
        if (finalMatchElements.Count >= 3)
        {
            return finalMatchElements;
        }

        columnMatchElements.Clear();
        rowMatchElements.Clear();
        finalMatchElements.Clear();


        columnMatchElements.Add(element);
        for (int columnDistance = 1; columnDistance < rows; columnDistance++)
        {
            int y = element.Y + columnDistance;
            if (y > rows)
            {
                break;
            }
            if (element.textureComponent.textureType == elements[element.X, y].textureComponent.textureType)
            {
                columnMatchElements.Add(elements[element.X, y]);
            }
            else
            {
                break;
            }
        }

        for (int columnDistance = 1; columnDistance < rows; columnDistance++)
        {
            int y = element.Y - columnDistance;
            if (y < 0)
            {
                break;
            }
            if (element.textureComponent.textureType == elements[element.X, y].textureComponent.textureType)
            {
                columnMatchElements.Add(elements[element.X, y]);
            }
            else
            {
                break;
            }
        }

        if (columnMatchElements.Count >= 3)
        {
            for (int i = 0; i < columnMatchElements.Count; i++)
            {
                finalMatchElements.Add(columnMatchElements[i]);
                for (int rowDistance = 1; rowDistance < columns; rowDistance++)
                {
                    int x = element.X + rowDistance;
                    if (x >= columns)
                    {
                        break;
                    }
                    if (element.textureComponent.textureType == elements[x, columnMatchElements[i].Y].textureComponent.textureType)
                    {
                        rowMatchElements.Add(elements[x, columnMatchElements[i].Y]);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int rowDistance = 1; rowDistance < columns; rowDistance++)
                {
                    int x = element.X - rowDistance;
                    if (x < 0)
                    {
                        break;
                    }
                    if (element.textureComponent.textureType == elements[x, columnMatchElements[i].Y].textureComponent.textureType)
                    {
                        rowMatchElements.Add(elements[x, columnMatchElements[i].Y]);
                    }
                    else
                    {
                        break;
                    }
                }

                if (rowMatchElements.Count >= 3)
                {
                    for (int j = 0; j < rowMatchElements.Count; j++)
                    {
                        finalMatchElements.Add(rowMatchElements[j]);
                    }
                }
                rowMatchElements.Clear();
            }
        }
        if (finalMatchElements.Count >= 3)
        {
            return finalMatchElements;
        }

        return null;
    }

    public bool ClearAllMatchSweet()
    {
        bool needFill = false;
        HashSet<Element> match = new HashSet<Element>();
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j <= rows; j++)
            {
                List<Element> match1 = GetMatchSweet(elements[i, j]);
                if (match1 != null)
                {

                    for (int k = 0; k < match1.Count; k++)
                    {
                        match.Add(match1[k]);
                    }
                }
            }
        }
        if (match.Count > 0)
        {
            addScore += match.Count;
            foreach (Element item in match)
            {

                elements[item.X, item.Y] = null;
                item.clearComponent.Clear();
            }

            needFill = true;
        }
        return needFill;
    }
}
