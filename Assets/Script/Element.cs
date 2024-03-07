using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    private int m_x;
    private int m_y;
    public int X
    {
        get
        {
            return m_x;
        }
        set
        {
            m_x = value;
        }
    }

    public int Y
    {
        get
        {
            return m_y;
        }
        set
        {
            m_y = value;
        }
    }

    private GameManager.ElementType m_type;
    public GameManager.ElementType Type
    {
        get
        {
            return m_type;
        }
    }
    [HideInInspector]
    private GameManager m_gameManager;
    public GameManager gameManager
    {
        get
        {
            return m_gameManager;
        }
    }

    private MoveComponent m_moveCompoent;
    public MoveComponent moveComponent
    {
        get
        {
            return m_moveCompoent;
        }
    }

    private TextureComponent m_textureCompoent;
    public TextureComponent textureComponent
    {
        get
        {
            return m_textureCompoent;
        }
    }

    public void Awake()
    {
        m_moveCompoent = GetComponent<MoveComponent>();
        m_textureCompoent = GetComponent<TextureComponent>();
    }

    public void Init(int col, int row, GameManager _gameManager, GameManager.ElementType type)
    {
        m_gameManager = _gameManager;
        m_type = type;
        m_x = col;
        m_y = row;
    }

}
