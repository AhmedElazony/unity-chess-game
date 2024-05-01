using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluation
{
    public static int GetScore(Piece[,] boardPieces)
    {
        int whiteScore = 0;
        int blackScore = 0;
        for (int i = 0; i < Board.CountSquaresX; i++)
        {
            for (int j = 0; j < Board.CountSquaresY; j++)
            {
                if (boardPieces[i, j] == null) continue;

                Piece piece = boardPieces[i, j];
                if (piece.color == PieceColor.White)
                {
                    whiteScore += piece.Value - GetMobilityPenalty(boardPieces, i, j);
                    whiteScore += GetPawnStructureBonus(boardPieces, i, j);
                    //whiteScore += piece.currentX;
                }
                else
                {
                    blackScore += piece.Value - GetMobilityPenalty(boardPieces, i, j);
                    blackScore += GetPawnStructureBonus(boardPieces, i, j);
                    //blackScore += piece.currentX;
                }
            }
        }

        Vector2Int whiteKingPosition = King.FindKingPosition(boardPieces, true);
        Vector2Int blackKingPosition = King.FindKingPosition(boardPieces, false);

        if (King.IsInCheck(ref boardPieces, whiteKingPosition, false))
        {
            whiteScore -= 5;
            blackScore += 5;
        }

        if (King.IsInCheck(ref boardPieces, blackKingPosition, true))
        {
            blackScore -= 5;
            whiteScore += 5;
        }


        int eval = whiteScore - blackScore;

        return eval;
    }

    private static int GetMobilityPenalty(Piece[,] board, int x, int y)
    {
        int penalty = 0;
        Piece piece = board[x, y];
        List<Vector2Int> validMoves = piece.GetValidMoves(ref board);

        for (int i = 0; i < validMoves.Count; i++)
        {
            Vector2Int move = validMoves[i];
            if (!IsGoodMove(board, piece.color, x, y, move.x, move.y)) // Check if friendly piece blocks
            {
                penalty += 1; // Adjust penalty value based on importance
            }
        }

        return penalty;
    }
    private static bool IsGoodMove(Piece[,] board, PieceColor color, int fromX, int fromY, int toX, int toY)
    {
        // Check if destination is empty or occupied by an enemy piece
        if (board[toX, toY] == null || board[toX, toY].color != color)
        {
            return true;
        }
        return false; // Destination occupied by friendly piece
    }

    private static int GetPawnStructureBonus(Piece[,] board, int x, int y)
    {
        int bonus = 0;
        Piece piece = board[x, y];

        if (piece.type == PieceType.Pawn)
        {
            // Check for isolated pawns (no pawns on neighboring files)
            if (!IsPawnNeighbor(board, piece.color, x, y, -1) && !IsPawnNeighbor(board, piece.color, x, y, 1))
            {
                bonus -= 1; // Adjust bonus value based on importance
            }

            // Check for passed pawns (no enemy pawns in front on the same file)
            if (!IsEnemyPawnBlocking(board, piece.color, x, y))
            {
                bonus += 2; // Adjust bonus value based on importance
            }
        }

        return bonus;
    }
    private static bool IsEnemyPawnBlocking(Piece[,] board, PieceColor color, int x, int y)
    {
        PieceColor enemyColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        for (int i = x + 1; i < Board.CountSquaresX; i++)
        {
            if (board[i, y] != null)
            {
                // Check if the piece blocking is an enemy pawn
                if (board[i, y].color == enemyColor && board[i, y].type == PieceType.Pawn)
                {
                    return true;
                }
                else
                {
                    // Stop iterating if another piece (not a pawn) is encountered
                    break;
                }
            }
        }
        return false;
    }

    private static bool IsPawnNeighbor(Piece[,] board, PieceColor color, int x, int y, int offset)
    {
        int neighborX = x + offset;
        if (neighborX >= 0 && neighborX < Board.CountSquaresX && board[neighborX, y] != null && board[neighborX, y].color == color && board[neighborX, y].type == PieceType.Pawn)
        {
            return true;
        }
        return false;
    }
}
