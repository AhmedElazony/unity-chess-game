using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    public new int Value { get => PieceValue.Queen; }

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
