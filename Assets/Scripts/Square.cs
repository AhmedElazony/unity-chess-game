using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private Color baseColor;
    [SerializeField] private Color oddColor;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public Vector3 Position
    {
        get { return transform.position; }
    }

    public Color ObjectColor
    { 
        get { return spriteRenderer.color; } 
        set { spriteRenderer.color = value; }
    }
    

    // Initialize Color
    public void Initialize(bool isOdd)
    {
        spriteRenderer.color = isOdd ? oddColor : baseColor;
    }
}
