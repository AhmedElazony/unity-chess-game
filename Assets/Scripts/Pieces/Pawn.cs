using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Pawn : Piece
{
    private const int WhiteFirstYPosition = 1;
    private const int BlackFirstYPosition = 6;

    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] boardPieces)
    {
        validMoves = new List<Vector2Int>();

        int direction = (this.color == PieceColor.White) ? 1 : -1;

        // Regular Movement.
        if(Board.IsEmptySquare(boardPieces, currentX, currentY + direction))
        {
            // Move One Square Forward.
            validMoves.Add(new Vector2Int(currentX, currentY + direction));

            // Move Two Squares Forward, if First Rank.
            if (AvailableDoubleForwardMove(boardPieces, currentX, currentY + direction * 2))
            {
                validMoves.Add(new Vector2Int(currentX, currentY + direction * 2));
            }
        }

        // Capture Movement:

        // Attack The Right Enemy.
        if (currentX != Board.CountSquaresX - 1) // Current x Position is Not in The Last Square in the Right.
        {
            if (CheckRightEnemyAttack(boardPieces, currentX + 1, currentY + direction))
                validMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
        }
        // Attack The Left Enemy.
        if (currentX != 0) // Current x Position is Not in the First Square in the Left.
        {
            if (CheckLeftEnemyAttack(boardPieces, currentX - 1, currentY + direction))
                validMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
        }

        return validMoves;
    }

    private bool AvailableDoubleForwardMove(Piece[,] boardPieces, int targetXPosition, int targetYPosition)
    {
        return currentY == ((this.color == PieceColor.White) ? WhiteFirstYPosition : BlackFirstYPosition)
                && Board.IsEmptySquare(boardPieces, targetXPosition, targetYPosition);
    }

    private bool CheckRightEnemyAttack(Piece[,] boardPieces, int targetX, int targetY)
    {
        return !(Board.IsEmptySquare(boardPieces, targetX, targetY)) 
            && IsEnemy(this, boardPieces[targetX, targetY]);
    }

    private bool CheckLeftEnemyAttack(Piece[,] boardPieces, int targetX, int targetY)
    {
        return !(Board.IsEmptySquare(boardPieces, targetX, targetY)) 
            && IsEnemy(this, boardPieces[targetX, targetY]);
    }

    private void Update()
    {
        // If Pawn Gets to the Last Square (Vertically), Turn it into Queen.
        if (type == PieceType.Pawn && currentY == ((color == PieceColor.White) ? Board.CountSquaresY - 1 : 0))
        {
            Debug.Log("Yes");
            TurnIntoQueen(this);
        }
    }

}
