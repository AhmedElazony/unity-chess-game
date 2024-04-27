using UnityEngine;

public class Move : MonoBehaviour
{
   /* // Count The Move for square feedback.
    private int moveCount = 1;

    // Square Color after the move.
    private readonly Color feedbackColor = new Color32(244, 245, 149, 255);

    // Store the position of the selected Chess Piece, So you can move it.
    private Vector2Int selectedPiecePosition = new Vector2Int(-1, -1);*/


    /*private void MovePieceTO(Piece piece, int targetX, int targetY)
    {
        if (Board.boardPieces[piece.currentX, piece.currentY] == null) return; // No piece to move

        // Get The Squares in this move, so you can change their colors.
        Square previousSquare = Board.boardSquares[piece.currentX, piece.currentY];
        Square targetSquare = Board.boardSquares[targetX, targetY];

        // Validate move according to piece rules
        if (!piece.IsValidMove(targetX, targetY))
        {
            gameManager.PlayAudioClip(gameManager.notifyAudioClip);
            Board.ResetBoardColors(null, null);
            return;
        }

        // change turns of the movement.
        isWhiteTurn = !isWhiteTurn;

        // Remove the colors of available moves.
        Board.ResetBoardColors(previousSquare, targetSquare);

        // Is The Move an attack?
        bool isAttacking = false;

        // Is There a piece on the target position?
        if (Board.boardPieces[targetX, targetY] != null) // Target position is occupied
        {
            // Target Position is Occupied with a piece in the same team.
            if (Board.boardPieces[targetX, targetY].color == piece.color)
            {
                gameManager.PlayAudioClip(gameManager.notifyAudioClip);
                Board.ResetBoardColors(null, null);
                return;
            }
            else // Target Position is Occupied with an enemy piece, So eat it.
            {
                isAttacking = true;
                if (Board.boardPieces[targetX, targetY].color == PieceColor.White)
                    deadWhitePieces.Add(boardPieces[targetX, targetY]);
                else
                    deadBlackPieces.Add(boardPieces[targetX, targetY]);

                // Play Attacking Audio Clip.
                gameManager.PlayAudioClip(gameManager.captureAudioClip);

                this.PositionDeadPiece(boardPieces[targetX, targetY]);

                if (Board.boardPieces[targetX, targetY].type == PieceType.King)
                    CheckMate(piece.color);
            }
        }

        // Move the selected piece in the array.
        boardPieces[targetX, targetY] = piece;
        boardPieces[piece.currentX, piece.currentY] = null;

        // Position the Piece into the target Position.
        PositionSinglePiece(piece, targetX, targetY);

        // Play Movement Audio Clip.
        if (!isAttacking)
            gameManager.PlayAudioClip(gameManager.moveAudioClip);

        moveCount++;

        // Make A Square Feedback (Change its Color) for every move.
        this.ChangeMoveColors(previousSquare, targetSquare);
    }

    private Vector2Int GetClickPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));

        int xIndex = Mathf.RoundToInt(worldPosition.x / squareSize);
        int yIndex = Mathf.RoundToInt(worldPosition.y / squareSize);

        if (xIndex >= 0 && xIndex < CountSquaresX && yIndex >= 0 && yIndex < CountSquaresY)
        {
            return new Vector2Int(xIndex, yIndex);
        }

        return new Vector2Int(-1, -1);
    }

    private void ChangeMoveColors(Square previous, Square target)
    {
        previous.ObjectColor = feedbackColor;
        target.ObjectColor = feedbackColor;

        // Reset The Colors after the second move.
        if (moveCount == 2)
        {
            // Reset All Squares Except the two in this current move.
            ResetBoardColors(previous, target);
            moveCount = 1;
        }
    }*/
}
