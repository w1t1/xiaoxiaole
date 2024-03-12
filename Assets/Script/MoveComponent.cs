using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    private Element element;
    private IEnumerator monveCoroutine;

    public void Awake()
    {
        element = GetComponent<Element>();
    }

    public void move(int col, int row, float time)
    {
        if (monveCoroutine != null)
        {
            StopCoroutine(monveCoroutine);
        }
        monveCoroutine = MoveCoroutine(col, row, time);
        StartCoroutine(monveCoroutine);
    }

    private IEnumerator MoveCoroutine(int col, int row, float time)
    {
        element.X = col;
        element.Y = row;

        Vector3 startPos = transform.position;
        Vector3 endPos = element.gameManager.GenElementPos(col, row, 0.5f);
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            element.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return null;
        }
        element.transform.position = endPos;
    }
}
