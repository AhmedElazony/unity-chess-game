using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChessAI
{
    public static PieceMove FindBestMove(Piece[,] boardPieces, int depth, bool isWhiteTurn)
    {
        //Piece[,] simBoard = Board.DeepCopy(boardPieces);
        int bestScore = isWhiteTurn ? int.MinValue : int.MaxValue;
        Piece bestPieceToMove = null;
        Vector2Int bestMove = Vector2Int.zero;
        int score = 0;

        List<PieceMove> possibleMoves = GetAllPossibleMoves(boardPieces, isWhiteTurn ? PieceColor.White : PieceColor.Black);
        foreach (var move in possibleMoves)
        {
            //Vector2Int piecePosition = new(move.Piece.currentX, move.Piece.currentY);
            
            Piece[,] simBoard = SimulateMove(boardPieces, move.Piece, move.To);

            /*move.Piece.currentX = piecePosition.x;
            move.Piece.currentY = piecePosition.y;*/

            score = Minimax(simBoard, depth - 1, !isWhiteTurn);

            if ((isWhiteTurn && score > bestScore) || (!isWhiteTurn && score < bestScore))
            {
                bestScore = score;
                bestPieceToMove = move.Piece;
                bestMove = move.To;
            }
        }

        return new PieceMove(bestPieceToMove, new(bestPieceToMove.currentX, bestPieceToMove.currentY), bestMove, score);
    }

    public static int Minimax(Piece[,] boardPieces, int depth, bool maximizingPlayer)
    {
        if (depth == 0 || Board.gameEnded)
            return Evaluation.GetScore(boardPieces);

        if (maximizingPlayer)
        {
            int maxValue = int.MinValue;

            List<PieceMove> possibleMoves = GetAllPossibleMoves(boardPieces, PieceColor.White);
            foreach (var move in possibleMoves)
            {
                Piece[,] simBaord = SimulateMove(boardPieces, move.Piece, move.To);
                int score = Minimax(simBaord, depth - 1, false);
                maxValue = Mathf.Max(maxValue, score);
            }
            return maxValue;
        }
        else
        {
            int minValue = int.MaxValue;

            List<PieceMove> possibleMoves = GetAllPossibleMoves(boardPieces, PieceColor.White);
            foreach (var move in possibleMoves)
            {
                Piece[,] simBaord = SimulateMove(boardPieces, move.Piece, move.To);
                int score = Minimax(simBaord, depth - 1, true);
                minValue = Mathf.Min(minValue, score);
            }
            return minValue;
        }
    }

    public static Piece[,] SimulateMove(Piece[,] board, Piece piece, Vector2Int move)
    {
        Piece[,] newBoard = Board.DeepCopy(board);

        // Save the current values, to reset after simulation
        int actualX = piece.currentX;
        int actualY = piece.currentY;

        // simulate the move.
        int simX = move.x;
        int simY = move.y;

        // Simulate Move
        newBoard[actualX, actualY] = null;
        //piece.currentX = simX;
        //piece.currentY = simY;
        newBoard[simX, simY] = piece;

        return newBoard;
    }

    public static void MakeMove(int depth)
    {
        PieceMove pieceMove = FindBestMove(Board.boardPieces, depth, Board.isWhiteTurn);
        Board.MovePieceTo(pieceMove.Piece, pieceMove.To.x, pieceMove.To.y);
    }

    private static List<PieceMove> GetAllPossibleMoves(Piece[,] boardPieces, PieceColor color)
    {
        // Implement a function to generate all possible moves for a given player color
        // Consider generating moves for all pieces of the given color on the board
        // Return a list of PieceMove objects representing the possible moves

        List<PieceMove> possibleMoves = new List<PieceMove>();

        Vector2Int kingPosition = King.FindKingPosition(boardPieces, (color == PieceColor.White));

        for (int x = 0; x < Board.CountSquaresX; x++)
        {
            for (int y = 0; y < Board.CountSquaresY; y++)
            {
                Piece piece = boardPieces[x, y];
                if (piece != null && piece.color == color)
                {
                    List<Vector2Int> moves = piece.GetValidMoves(ref boardPieces);
                    // Remove moves that could make the checkmate.
                    Board.SimulateMoveForSinglePiece(piece, ref moves, kingPosition);

                    for (int i = 0; i < moves.Count; i++)
                    {
                        possibleMoves.Add(new PieceMove(piece, new Vector2Int(x, y), moves[i]));
                    }
                }
            }
        }

        return possibleMoves;
    }
}