using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public new int Value { get => PieceValue.Knight; }

    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] boardPieces)
    {
        validMoves.Clear();

        // Top Right
        int newX = currentX + 1, newY = currentY + 2;
        if (newX < Board.CountSquaresX && newY < Board.CountSquaresY
            && (Board.IsEmptySquare(boardPieces, newX, newY) || IsEnemy(this, boardPieces[newX, newY])))
            validMoves.Add(new Vector2Int(newX, newY));

        newX = currentX + 2;
        newY = currentY + 1;
        if (newX < Board.CountSquaresX && newY < Board.CountSquaresY
            && (Board.IsEmptySquare(boardPieces, newX, newY) || IsEnemy(this, boardPieces[newX, newY])))
            validMoves.Add(new Vector2Int(newX, newY));

        // Top Left
        newX = currentX - 1;
        newY = currentY + 2;
        if (newX >= 0 && newY < Board.CountSquaresY
            && (Board.IsEmptySquare(boardPieces, newX, newY) || IsEnemy(this, boardPieces[newX, newY])))
            validMoves.Add(new Vector2Int(newX, newY));

        newX = currentX - 2;
        newY = currentY + 1;
        if (newX >= 0 && newY < Board.CountSquaresY
            && (Board.IsEmptySquare(boardPieces, newX, newY) || IsEnemy(this, boardPieces[newX, newY])))
            validMoves.Add(new Vector2Int(newX, newY));

        // Down Left
        newX = currentX - 1;
        newY = currentY - 2;
        if (newX >= 0 && newY >= 0
            && (Board.IsEmptySquare(boardPieces, newX, newY) || IsEnemy(this, boardPieces[newX, newY])))
            validMoves.Add(new Vector2Int(newX, newY));

        newX = currentX - 2;
        newY = currentY - 1;
        if (newX >= 0 && newY >= 0
            && (Board.IsEmptySquare(boardPieces, newX, newY) || IsEnemy(this, boardPieces[newX, newY])))
            validMoves.Add(new Vector2Int(newX, newY));

        // Down Right
        newX = currentX + 2;
        newY = currentY - 1;
        if (newX < Board.CountSquaresX && newY >= 0
            && (Board.IsEmptySquare(boardPieces, newX, newY) || IsEnemy(this, boardPieces[newX, newY])))
            validMoves.Add(new Vector2Int(newX, newY));

        newX = currentX + 1;
        newY = currentY - 2;
        if (newX < Board.CountSquaresX && newY >= 0
            && (Board.IsEmptySquare(boardPieces, newX, newY) || IsEnemy(this, boardPieces[newX, newY])))
            validMoves.Add(new Vector2Int(newX, newY));

        return validMoves;
    }
}
