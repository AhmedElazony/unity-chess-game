using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class King : Piece
{
    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] boardPieces)
    {
        validMoves.Clear();

        // Right.
        if (currentX != Board.CountSquaresX - 1 && (Board.IsEmptySquare(boardPieces, currentX + 1, currentY) || IsEnemy(this, boardPieces[currentX + 1, currentY])))
            validMoves.Add(new Vector2Int(currentX + 1, currentY));

        // Left.
        if (currentX != 0 && (Board.IsEmptySquare(boardPieces, currentX - 1, currentY) || IsEnemy(this, boardPieces[currentX - 1, currentY])))
            validMoves.Add(new Vector2Int(currentX - 1, currentY));

        // Up.
        if (currentY != Board.CountSquaresY - 1 && (Board.IsEmptySquare(boardPieces, currentX, currentY + 1) || IsEnemy(this, boardPieces[currentX, currentY + 1])))
            validMoves.Add(new Vector2Int(currentX, currentY + 1));

        // Down.
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
        specialMoves.Clear();
        
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

    public static Vector2Int FindKingPosition(Piece[,] boardPieces, bool isWhiteTurn)
    {
        // Find King Position
        for (int i = 0; i < Board.CountSquaresX; i++)
        {
            for (int j = 0; j < Board.CountSquaresY; j++)
            {
                Piece piece = boardPieces[i, j];

                if (piece != null && piece.type == PieceType.King && piece.color == (isWhiteTurn ? PieceColor.White : PieceColor.Black))
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return Vector2Int.zero;
    }

    public static bool IsInCheck(ref Piece[,] boardPieces, Vector2Int kingPosition, bool isWhiteTurn)
    {
        foreach (var piece in boardPieces)
        {
            if (piece != null && (piece.color == (isWhiteTurn ? PieceColor.Black : PieceColor.White)))
            {
                List<Vector2Int> moves = piece.GetValidMoves(ref boardPieces);
                
                if (moves.Contains(kingPosition))
                    return true; // king is in check;
            }
        }
        return false;
    }
}
