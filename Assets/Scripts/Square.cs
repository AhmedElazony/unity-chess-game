using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] public Color baseColor;
    [SerializeField] public Color offsetColor;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public Color ObjectColor
    { 
        get { return spriteRenderer.color; } 
        set { spriteRenderer.color = value; }
    }
    

    // Initialize Color
    public void Initialize(bool isOffset)
    {
        spriteRenderer.color = isOffset ? offsetColor : baseColor;
    }
}
