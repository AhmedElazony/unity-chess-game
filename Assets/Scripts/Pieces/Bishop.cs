using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] boardPieces)
    {
        validMoves = new List<Vector2Int>();

        // Top Right Diagonal
        for (int i = currentX + 1, j = currentY + 1; i < Board.CountSquaresX && j < Board.CountSquaresY; i++, j++)
        {
            if (Board.IsEmptySquare(boardPieces, i, j))
                validMoves.Add(new Vector2Int(i, j));
            else
            {
                if (IsEnemy(this, boardPieces[i, j]))
                    validMoves.Add(new Vector2Int(i, j));

                break;
            }
            
        }

        // Bottom Rigth Diagonal
        for (int i = currentX - 1, j = currentY - 1; i >= 0 && j >= 0; i--, j--)
    {
            if (Board.IsEmptySquare(boardPieces, i, j))
                validMoves.Add(new Vector2Int(i, j));
            else
            {
                if (IsEnemy(this, boardPieces[i, j]))
                    validMoves.Add(new Vector2Int(i, j));

                break;
            }
        }

        // Top Left Diagonal
        for (int i = currentX - 1, j = currentY + 1; i >= 0 && j < Board.CountSquaresY; i--, j++)
        {
            if (Board.IsEmptySquare(boardPieces, i, j))
                validMoves.Add(new Vector2Int(i, j));
            else
            {
                if (IsEnemy(this, boardPieces[i, j]))
                    validMoves.Add(new Vector2Int(i, j));

                break;
            }
        }

        // Bottom Left Diagonal
        for (int i = currentX + 1, j = currentY - 1; i < Board.CountSquaresX && j >= 0; i++, j--)
        {
            if (Board.IsEmptySquare(boardPieces, i, j))
                validMoves.Add(new Vector2Int(i, j));
            else
            {
                if (IsEnemy(this, boardPieces[i, j]))
                    validMoves.Add(new Vector2Int(i, j));

                break;
            }
        }

        return validMoves;
    }
}
