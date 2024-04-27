using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class King : Piece
{
    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] boardPieces)
    {
        validMoves = new List<Vector2Int>();

        // Right.
        if (currentX != Board.CountSquaresX - 1 && (Board.IsEmptySquare(boardPieces, currentX + 1, currentY) || IsEnemy(this, boardPieces[currentX + 1, currentY])))
            validMoves.Add(new Vector2Int(currentX + 1, currentY));

        // Left.
        if (currentX != 0 && (Board.IsEmptySquare(boardPieces, currentX - 1, currentY) || IsEnemy(this, boardPieces[currentX - 1, currentY])))
            validMoves.Add(new Vector2Int(currentX - 1, currentY));

        // Up.
        if (currentY != Board.CountSquaresY - 1 && (Board.IsEmptySquare(boardPieces, currentX, currentY + 1) || IsEnemy(this, boardPieces[currentX, currentY + 1])))
            validMoves.Add(new Vector2Int(currentX, currentY + 1));

        // Up.
        if (currentY != 0 && (Board.IsEmptySquare(boardPieces, currentX, currentY - 1) || IsEnemy(this, boardPieces[currentX, currentY - 1])))
            validMoves.Add(new Vector2Int(currentX, currentY - 1));

        // Up Right.
        if (currentY != Board.CountSquaresY - 1 && currentX != Board.CountSquaresX - 1 && (Board.IsEmptySquare(boardPieces, currentX + 1, currentY + 1) || IsEnemy(this, boardPieces[currentX + 1, currentY + 1])))
            validMoves.Add(new Vector2Int(currentX + 1, currentY + 1));

        // Up Left
        if (currentY != Board.CountSquaresY - 1 && currentX != 0 && (Board.IsEmptySquare(boardPieces, currentX - 1, currentY + 1) || IsEnemy(this, boardPieces[currentX - 1, currentY + 1])))
            validMoves.Add(new Vector2Int(currentX - 1, currentY + 1));

        // Down Right
        if (currentY != 0 && currentX != Board.CountSquaresX - 1 && (Board.IsEmptySquare(boardPieces, currentX + 1, currentY - 1) || IsEnemy(this, boardPieces[currentX + 1, currentY - 1])))
            validMoves.Add(new Vector2Int(currentX + 1, currentY - 1));

        // Down Left
        if (currentY != 0 && currentX != 0 && (Board.IsEmptySquare(boardPieces, currentX - 1, currentY - 1) || IsEnemy(this, boardPieces[currentX - 1, currentY - 1])))
            validMoves.Add(new Vector2Int(currentX - 1, currentY - 1));

        return validMoves;
    }
    
    // Add Castling Mov
    public override List<Vector2Int> GetSpecialMoves(ref Piece[,] boardPieces)
    {
        specialMoves = new List<Vector2Int>();
        
        // Right Rook.
        for (int i = currentX + 1; i < Board.CountSquaresX; i++)
        {
            if (Board.IsEmptySquare(boardPieces, i, currentY)) continue;
            else if (!Board.IsEmptySquare(boardPieces, i, currentY) && boardPieces[i, currentY].type == PieceType.Rook)
            {
                Piece rook = boardPieces[i, currentY];

                if (!rook.hasMoved && !this.hasMoved)
                    specialMoves.Add(new Vector2Int(i - 1, currentY));
            }
            else break;
        }

        // Left Rook.
        for (int i = currentX - 1; i >= 0; i--)
        {
            if (Board.IsEmptySquare(boardPieces, i, currentY)) continue;
            else if (!Board.IsEmptySquare(boardPieces, i, currentY) && boardPieces[i, currentY].type == PieceType.Rook)
            {
                Piece rook = boardPieces[i, currentY];

                if (!rook.hasMoved && !this.hasMoved)
                    specialMoves.Add(new Vector2Int(i + 2, currentY));
            }
            else break;
        }

        return specialMoves;
    }
}
