using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureComponent : MonoBehaviour
{
    public enum TextureType
    {
        RED,
        GREEN,
        BLUE,
        YELLOW,
        PINK,
        BLACK,
        PURPLE,
        COUNT//标记类型
    }
    [System.Serializable]
    public struct TextureStruct
    {
        public TextureType type;
        public Texture2D texture;
    }
    public TextureStruct[] textureStructs;
    private Dictionary<TextureType, Texture2D> textureTypeDict;
    private MeshRenderer meshRenderer;
    private TextureType m_textureType;
    public TextureType textureType
    {
        get
        {
            return m_textureType;
        }

        set
        {
            m_textureType = value;
        }
    }

    public int GetTypeCount() { return textureStructs.Length; }

    private void Awake()
    {
        meshRenderer = transform.Find("plane").GetComponent<MeshRenderer>();
        textureTypeDict = new Dictionary<TextureType, Texture2D>();
        for (int i = 0; i < textureStructs.Length; i++)
        {
            if (!textureTypeDict.ContainsKey(textureStructs[i].type))
            {
                textureTypeDict.Add(textureStructs[i].type, textureStructs[i].texture);
            }
        }
    }

    public void SetTexture(TextureType type)
    {
        m_textureType = type;
        if (textureTypeDict.ContainsKey((TextureType)type))
        {
            meshRenderer.material.mainTexture = textureTypeDict[type];
        }
    }
}
