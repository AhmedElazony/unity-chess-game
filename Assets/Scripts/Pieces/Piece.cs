using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

public abstract class Piece : MonoBehaviour
{
    public PieceType type;
    public PieceColor color;

    public Sprite whiteSprite;
    public Sprite blackSprite;

    // Piece Position Coords.
    public int currentX;
    public int currentY;
    protected Vector3 desiredPosition;
    protected List<Vector2Int> validMoves;

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

    public virtual List<Vector2Int> GetAvailableMoves(ref Piece[,] boardPieces)
    {
        List<Vector2Int> validMoves = new()
        {
            new(3,3),
            new(3,4),
            new(3,5),
        };

        return validMoves;
    }
    public bool IsValidMove(int targetX, int targetY)
    {
        return validMoves.Contains(new Vector2Int(targetX, targetY));
    }

    public static bool IsEnemy(Piece attackingPiece, Piece attackedPiece)
    {
        return attackingPiece.color != attackedPiece.color;
    }

    protected static void TurnIntoQueen(Piece piece)
    {
        Piece queen = Board.SpawnSinglePiece(PieceType.Queen, piece.color);
        queen.currentX = piece.currentX;
        queen.currentY = piece.currentY;
        
        Board.PositionSinglePiece(queen, queen.currentX, queen.currentY);
        Board.boardPieces[piece.currentX, piece.currentY] = queen;
        
        Destroy(piece.gameObject);
    }
}
