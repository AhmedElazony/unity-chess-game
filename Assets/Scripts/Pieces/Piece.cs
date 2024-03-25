using System.Diagnostics.Tracing;
using UnityEngine;

public enum PieceType
{
    None = 0,
    Rook = 1,
    Knight = 2,
    Bishop = 3,
    Queen = 4,
    King = 5,
    Pawn = 6
}

public enum PieceColor { White = 0, Black = 1 };

public class Piece : MonoBehaviour
{
    public PieceType type;
    public PieceColor color;

    public Sprite whiteSprite;
    public Sprite blackSprite;

    // Piece Position Coords.
    public int currentX;
    public int currentY;
    protected Vector3 desiredPosition;

    public void SetSprite(PieceColor color)
    {
        if (color == PieceColor.White)
        {
            this.GetComponent<SpriteRenderer>().sprite = whiteSprite;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().sprite = blackSprite;
        }
    }

    public void SetScale(float x, float y, float z = 0)
    {
        transform.localScale = new Vector3(x, y, z);
    }

    private void Update()
    {
        
    }
}
