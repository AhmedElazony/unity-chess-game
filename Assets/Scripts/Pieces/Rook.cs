using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public new int Value { get => PieceValue.Rook; }

    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] boardPieces)
    {
        validMoves.Clear();

        // Check For Up Moves
        for (int i = currentY + 1; i < Board.CountSquaresY; i++)
        {
            if (Board.IsEmptySquare(boardPieces, currentX, i))
                validMoves.Add(new Vector2Int(currentX, i));
            else
            {
                if (IsEnemy(this, boardPieces[currentX, i]))    
                    validMoves.Add(new Vector2Int(currentX, i));

                break;
            }

        }

        // Check For Down Moves
        for (int i = currentY - 1; i >= 0; i--)
        {
            if (Board.IsEmptySquare(boardPieces, currentX, i))
                validMoves.Add(new Vector2Int(currentX, i));
            else
            {
                if (IsEnemy(this, boardPieces[currentX, i]))            
                    validMoves.Add(new Vector2Int(currentX, i));
                    
                break;
            }
        }

        // Check For Right Moves
        for (int i = currentX + 1; i < Board.CountSquaresX; i++)
        {
            if (Board.IsEmptySquare(boardPieces, i, currentY))
                validMoves.Add(new Vector2Int(i, currentY));
            else
            {
                if (IsEnemy(this, boardPieces[i, currentY]))
                    validMoves.Add(new Vector2Int(i, currentY));
                    
                break;
            }
        }

        // Check For Left Moves
        for (int i = currentX - 1; i >= 0; i--)
        {
            if (Board.IsEmptySquare(boardPieces, i, currentY))
                validMoves.Add(new Vector2Int(i, currentY));
            else
            {
                if (IsEnemy(this, boardPieces[i, currentY]))
                    validMoves.Add(new Vector2Int(i, currentY));
                    
                break;
            }
        }

        return validMoves;
    }
}
