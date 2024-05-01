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

public struct PieceValue
{
    public static int Pawn = 1;
    public static int Bishop = 3;
    public static int Knight = 3;
    public static int Rook = 5;
    public static int Queen = 9;
}

public class PieceData
{
    public PieceType type;
    public PieceColor color;

    public PieceValue value;

    public int currentX;
    public int currentY;
    public bool hasMoved;
    public bool isDead;
}

public class PieceMove
{
    public Piece Piece { get; set; }
    public Vector2Int From { get; set; }
    public Vector2Int To { get; set; }
    public int Score { get; set; }

    public PieceMove(Piece piece, Vector2Int from, Vector2Int to, int score = 0)
    {
        Piece = piece;
        From = from;
        To = to;
        Score = score;
    }
}

public enum PieceColor { White = 0, Black = 1 };

public class Piece : MonoBehaviour
{
    public PieceType type;
    public PieceColor color;

    public int Value { get => 0; }

    public Sprite whiteSprite;
    public Sprite blackSprite;

    // Piece Position Coords.
    public int currentX;
    public int currentY;
    public bool hasMoved;
    public bool isDead;
    protected List<Vector2Int> validMoves = new();
    protected List<Vector2Int> specialMoves = new();
    
    void Awake()
    {
        hasMoved = false;
    }

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
        /*bool result = false;

        if (specialMoves != null && specialMoves.Contains(new Vector2Int(targetX, targetY)))
            result = true;

        if (validMoves != null &&  validMoves.Contains(new Vector2Int(targetX, targetY)))
            result = true;*/
        Vector2Int kingPosition = King.FindKingPosition(Board.boardPieces, Board.isWhiteTurn);
        List<Vector2Int> validMoves = GetValidMoves(ref Board.boardPieces);
        Board.SimulateMoveForSinglePiece(this, ref validMoves, kingPosition);

        if (validMoves != null && validMoves.Contains(new(targetX, targetY)))
            return true;

        return false;
        //return result;
    }

    public static bool IsEnemy(Piece attackingPiece, Piece attackedPiece)
    {
        return attackingPiece.color != attackedPiece.color;
    }

    public static void TurnIntoQueen(Piece piece, int targetX, int targetY)
    {
        Piece queen = Board.SpawnSinglePiece(PieceType.Queen, piece.color);
        queen.currentX = targetX;
        queen.currentY = targetY;
        
        Board.PositionSinglePiece(queen, queen.currentX, queen.currentY);
        Board.boardPieces[queen.currentX, queen.currentY] = queen;
        
        Destroy(piece.gameObject);
    }

    public virtual List<Vector2Int> GetSpecialMoves(ref Piece[,] boardPieces)
    {
        return null;
    }

    public List<Vector2Int> GetValidMoves(ref Piece[,] boardPieces)
    {
        List<Vector2Int> moves = GetAvailableMoves(ref boardPieces);
        
        if (GetSpecialMoves(ref boardPieces) != null && GetSpecialMoves(ref boardPieces).Count > 0)
            moves.AddRange(GetSpecialMoves(ref boardPieces));

        return moves;
    }

    public virtual void CastleKing(Piece rook, ref Piece[,] boardPieces)
    {
        int rookPreviousXPosition = rook.currentX;
        
        // Right Rook
        if (rookPreviousXPosition == 7)
        {
            rook.currentX = 5;
        }
        else // Left Rook
        {
            rook.currentX = 3;
        }
        
        boardPieces[rook.currentX, rook.currentY] = rook;
        boardPieces[rookPreviousXPosition, rook.currentY] = null;

        rook.hasMoved = true;

        Board.PositionSinglePiece(rook, rook.currentX, rook.currentY);
    }
}
