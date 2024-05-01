using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    private void Update()
    {
        Vector2Int kingPosition = King.FindKingPosition(Board.boardPieces, Board.isWhiteTurn);
        if (King.IsInCheck(ref Board.boardPieces, kingPosition, Board.isWhiteTurn))
        {
            Board.board.boardSquares[kingPosition.x, kingPosition.y].ObjectColor = new Color32(224, 20, 0, 255);
        }
    }
    public static void PlayMove()
    {
        Thread.Sleep(200);

        if (!Board.isWhiteTurn && Board.playingWithAI)
        {
            ChessAI.MakeMove(3);
        }
    }
}
