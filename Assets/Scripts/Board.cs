using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Reference The Square Graphics.
    [Header("Board Squares")]
    [SerializeField] private Square squarePrefab;
    public static readonly float squareSize = 2f;

    public Square[,] boardSquares = new Square[8, 8];
    private Color[,] squareColors = new Color[8, 8];

    // Square Color after the move (feedback). and square color for available move.
    private readonly Color feedbackColor1 = new Color32(244, 245, 149, 255);
    private readonly Color feedbackColor2 = new Color32(209, 210, 98, 255);

    // Store The Chess Pieces Prefabs.
    [Header("Board Pieces")]
    [SerializeField] private GameObject[] pieces;

    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    // Store the Board Pieces Positions.
    public static Piece[,] boardPieces;

    // Store The Dead Pieces.
    public static List<Piece> deadWhitePieces = new List<Piece>();
    public static List<Piece> deadBlackPieces = new List<Piece>();

    // Store the position of the selected Chess Piece, So you can move it.
    private Vector2Int selectedPiecePosition = new Vector2Int(-1, -1);

    // Store the Available Moves for the selected piece.
    public List<Vector2Int> pieceAvailableMoves = new List<Vector2Int>();

    // Store Special Moves Like Castling.
    private List<Vector2Int> pieceSpecialMoves = new List<Vector2Int>();

    // Store the Move Count, So you can change the Squares Colors after the second move.
    private int moveCount = 1;

    // Store the moves list for the game.
    public static List<List<Vector2Int>> movesList = new();

    // Store The White Turn to play, then you can control turns for players.
    public static bool isWhiteTurn;

    public static bool gameEnded = false;
    public static bool gameStarted = false;

    public static bool playingWithAI = false;
    public static List<Piece> AITeamPieces = new();

    // Count the X, Y Coords (Board Width and Height).
    public const int CountSquaresX = 8;
    public const int CountSquaresY = 8;

    public static Board board;

    void Awake()
    {
        board = this;
        isWhiteTurn = true;

        this.GenerateBoard();

        this.SpawnPieces();

        this.PositionPieces();

        for (int i = 0; i < CountSquaresX; i++)
        {
            for (int j = 0; j < CountSquaresY; j++)
            {
                if (boardPieces[i, j] != null && boardPieces[i, j].color == PieceColor.Black)
                    AITeamPieces.Add(boardPieces[i, j]);
            }
        }
    }

    private void Update()
    {
        Vector2Int kingPosition = King.FindKingPosition(boardPieces, isWhiteTurn);

        if (King.IsInCheck(ref boardPieces, kingPosition, isWhiteTurn))
        {
            board.boardSquares[kingPosition.x, kingPosition.y].ObjectColor = new Color32(224, 20, 0, 255);
        }

        if (gameEnded || !gameStarted) return;

        if (playingWithAI && !isWhiteTurn)
        {
            AIPlayer.PlayMove();
            return;
        }

        // Move Chess Pieces With Mouse Button.
        if (Input.GetMouseButtonDown(0))
        {
            int xIndex = GetClickPosition().x;
            int yIndex = GetClickPosition().y;

            if (xIndex >= 0 && yIndex >= 0)
            {
                if (selectedPiecePosition.x == -1 && selectedPiecePosition.y == -1)
                {
                    // First click - select piece
                    if (boardPieces[xIndex, yIndex] != null)
                    {
                        // Is It Player Turn?
                        if ((boardPieces[xIndex, yIndex].color == PieceColor.White && isWhiteTurn) || (boardPieces[xIndex, yIndex].color == PieceColor.Black && !isWhiteTurn))
                        {
                            selectedPiecePosition = new Vector2Int(xIndex, yIndex);

                            // Highlight The Square Clicked.
                            boardSquares[xIndex, yIndex].ObjectColor = feedbackColor1;

                            // Get Available Moves for the selected piece.
                            pieceAvailableMoves = boardPieces[xIndex, yIndex].GetAvailableMoves(ref boardPieces);
                            pieceSpecialMoves = boardPieces[xIndex, yIndex].GetSpecialMoves(ref boardPieces);
                            
                            if (pieceSpecialMoves != null)
                                pieceAvailableMoves.AddRange(pieceSpecialMoves);

                            // Check if the king is in check.
                            PreventCheckMate(isWhiteTurn);

                            // Highlight The Availble Moves Squares.
                            this.HighlightAvailableMovesSquares();
                        }
                    }
                    
                }
                else // Second click - move piece
                {
                    Piece piece = boardPieces[selectedPiecePosition.x, selectedPiecePosition.y];
                    MovePieceTo(piece, xIndex, yIndex);

                    selectedPiecePosition = new Vector2Int(-1, -1); // Reset selected piece
                }
            }
        }
    }

    // Create The Chess Board Squares.
    private void GenerateBoard()
    {
        for (int i = 0; i < CountSquaresX; i++)
        {
            for (int j = 0; j < CountSquaresY; j++)
            {
                Square square = Instantiate(squarePrefab, new Vector3(i * squareSize, j * squareSize), Quaternion.identity, transform);
                boardSquares[i, j] = square;

                bool isOdd = (i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0);

                square.Initialize(isOdd);
                squareColors[i, j] = square.ObjectColor;
            }
        }
    }

    // Spawn Chess Pieces.
    private void SpawnPieces()
    {
        boardPieces = new Piece[CountSquaresX, CountSquaresY];

        PieceType[] pieceTypes = new PieceType[] { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };
        
        // White Pieces.
        this.FillPiecesArray(pieceTypes, PieceColor.White);
        
        // Black Pieces.
        this.FillPiecesArray(pieceTypes, PieceColor.Black);
    }
    public static Piece SpawnSinglePiece(PieceType type, PieceColor color)
    {
        GameObject pieceType = board.pieces[(int)type - 1];
        Piece piece = Instantiate(pieceType).GetComponent<Piece>();

        piece.type = type;
        piece.color = color;
        piece.SetSprite(color);

        return piece;
    }
    private void FillPiecesArray(PieceType[] pieceTypes,PieceColor color)
    {
        if (color == PieceColor.White)
        {
            for (int i = 0; i < pieceTypes.Length; i++)
            {
                boardPieces[i, 0] = SpawnSinglePiece(pieceTypes[i], color);
            }

            for (int i = 0; i < 8; i++)
            {
                boardPieces[i, 1] = SpawnSinglePiece(PieceType.Pawn, PieceColor.White);
            }
        }
        else
        {
            for (int i = 0; i < pieceTypes.Length; i++)
            {
                boardPieces[i, 7] = SpawnSinglePiece(pieceTypes[i], color);
            }

            for (int i = 0; i < 8; i++)
            {
                boardPieces[i, 6] = SpawnSinglePiece(PieceType.Pawn, PieceColor.Black);
            }
        }
    }

    // Position Chess Pieces On The Board.
    private void PositionPieces()
    {
        // Position Pieces On the Board after the game starts.
        for (int xCoord = 0; xCoord < CountSquaresX; xCoord++)
        {
            for (int yCoord = 0; yCoord < CountSquaresY; yCoord++)
            {
                if (boardPieces[xCoord, yCoord] != null)
                {
                    PositionSinglePiece(boardPieces[xCoord, yCoord], xCoord, yCoord);
                }
            }
        }
    }
    public static void PositionSinglePiece(Piece piece, int xCoord, int yCoord)
    {
        piece.currentX = xCoord;
        piece.currentY = yCoord;
        piece.transform.position = new Vector3(xCoord * squareSize, yCoord * squareSize, -1);
    }
    protected void PositionDeadPiece(Piece deadPiece)
    {
        int pieceIndex = 0;

        if (deadPiece.color == PieceColor.White)
        {
            pieceIndex = deadWhitePieces.IndexOf(deadPiece);
            deadPiece.transform.position = new Vector3(8 * squareSize, pieceIndex, -1);
        }
        else
        {
            pieceIndex = deadBlackPieces.IndexOf(deadPiece);
            deadPiece.transform.position = new Vector3(-1 * squareSize, 7 * squareSize - pieceIndex, -1);
        }

        deadPiece.SetScale(0.3f, 0.3f);
    }

    // Move Pieces
    public static void MovePieceTo(Piece piece, int targetX, int targetY)
    {
        if (boardPieces[piece.currentX, piece.currentY] == null) return; // No piece to move

        // Get The Squares in this move, so you can change their colors.
        Square previousSquare = board.boardSquares[piece.currentX, piece.currentY];
        Square targetSquare = board.boardSquares[targetX, targetY];

        // Get The Previous position for the piece.
        Vector2Int previousPosition = new Vector2Int(piece.currentX, piece.currentY);

        // Validate move according to piece rules
        if (!piece.IsValidMove(targetX, targetY))
        {
            board.gameManager.PlayAudioClip(board.gameManager.notifyAudioClip);
            board.ResetBoardColors(null, null);
            return;
        }

        // Remove the colors of available moves.
        board.ResetBoardColors(previousSquare, targetSquare);

        // change turns of the movement.
        isWhiteTurn = !isWhiteTurn;

        // Is The Move an attack?
        bool isAttacking = false;

        // Is There a piece on the target position?
        if (boardPieces[targetX, targetY] != null) // Target position is occupied
        {
            // Target Position is Occupied with a piece in the same team.
            if (boardPieces[targetX, targetY].color == piece.color)
            {
                board.gameManager.PlayAudioClip(board.gameManager.notifyAudioClip);
                board.ResetBoardColors(null, null);
                return;
            }
            else // Target Position is Occupied with an enemy piece, So eat it.
            {
                isAttacking = true;
                if (boardPieces[targetX, targetY].color == PieceColor.White)
                    deadWhitePieces.Add(boardPieces[targetX, targetY]);
                else
                    deadBlackPieces.Add(boardPieces[targetX, targetY]);

                boardPieces[targetX, targetY].isDead = true;

                // Play Attacking Audio Clip.
                board.gameManager.PlayAudioClip(board.gameManager.captureAudioClip);

                board.PositionDeadPiece(boardPieces[targetX, targetY]);
            }
        }

        // Check For King Castling
        if (piece.type == PieceType.King && board.pieceSpecialMoves != null && board.pieceSpecialMoves.Contains(new Vector2Int(targetX, targetY)))
        {
            Piece rook = (targetX > piece.currentX) ? boardPieces[targetX + 1, targetY] : boardPieces[targetX - 2, targetY];
            piece.CastleKing(rook, ref boardPieces);
        }

        // Move the selected piece in the array.
        boardPieces[targetX, targetY] = piece;
        boardPieces[piece.currentX, piece.currentY] = null;

        // Check For Promoting Pawn.
        if (piece.type == PieceType.Pawn && targetY == ((piece.color == PieceColor.White) ? Board.CountSquaresY - 1 : 0))
        {
            Piece.TurnIntoQueen(piece, targetX, targetY);
        }

        piece.hasMoved = true;

        // Position the Piece into the target Position.
        PositionSinglePiece(piece, targetX, targetY);

        // Play Movement Audio Clip.
        if (!isAttacking)
            board.gameManager.PlayAudioClip(board.gameManager.moveAudioClip);

        board.moveCount++;

        // Add this move to the movesList.
        movesList.Add(new List<Vector2Int>() { previousPosition, new(targetX, targetY) });

        // Make A Square Feedback (Change its Color) for every move.
        board.ChangeMoveColors(previousSquare, targetSquare);

        if (IsCheckMate() == 1)
            CheckMate(piece.color);
        
        
        if (IsDraw())
            CheckMate(piece.color, true);
    }

    private void ChangeMoveColors(Square previous, Square target)
    {
        previous.ObjectColor = feedbackColor1;
        target.ObjectColor = feedbackColor2;
       
        // Reset The Colors after the second move.
        if (moveCount == 2)
        {
            // Reset All Squares Except the two in this current move.
            ResetBoardColors(previous, target);
            moveCount = 1;
        }
    }

    private void HighlightAvailableMovesSquares()
    {
        for (int i = 0; i < this.pieceAvailableMoves.Count; i++)
        {
            if (!IsEmptySquare(boardPieces, pieceAvailableMoves[i].x, pieceAvailableMoves[i].y) && Piece.IsEnemy(boardPieces[selectedPiecePosition.x, selectedPiecePosition.y], boardPieces[pieceAvailableMoves[i].x, pieceAvailableMoves[i].y]))
            {
                boardSquares[this.pieceAvailableMoves[i].x, this.pieceAvailableMoves[i].y].ObjectColor = new Color32(148, 42, 42, 255);
                continue;
            }
            boardSquares[this.pieceAvailableMoves[i].x, this.pieceAvailableMoves[i].y].ObjectColor = new Color32(116, (byte)(255 - i * 5), 35, 255);
        }

        if (pieceSpecialMoves != null)
        {
            for (int i = 0; i < this.pieceSpecialMoves.Count; i++)
            {
                boardSquares[pieceSpecialMoves[i].x, pieceSpecialMoves[i].y].ObjectColor = new Color32(255, 178, 0, 255);
            }
        }
    }
    protected void ResetBoardColors(Square first, Square second)
    {
        // Reset the colors of the squares
        for (int i = 0; i < CountSquaresX; i++)
        {
            for (int j = 0; j < CountSquaresY; j++)
            {
                if (boardSquares[i, j] != first && boardSquares[i, j] != second && boardSquares[i, j].ObjectColor != squareColors[i, j])
                {
                    boardSquares[i, j].ObjectColor = squareColors[i, j];
                }
            }
        }
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

    public static bool IsEmptySquare(Piece[,] board, int xIndex, int yIndex)
    {
        if (xIndex < 0 || yIndex < 0 || xIndex >= Board.CountSquaresX || yIndex >= Board.CountSquaresY)
            Debug.Log($"xIndex: {xIndex}, yIndex: {yIndex}");
        
        return board[xIndex, yIndex] == null;
    }

    private static void CheckMate(PieceColor color, bool draw = false)
    {
        gameEnded = true;
        if (draw == false)
            board.gameManager.EndGame((int)color + 1);
        else
            board.gameManager.EndGame(3);
    }

    public void PreventCheckMate(bool isWhiteTurn)
    {
        Vector2Int kingPosition = King.FindKingPosition(boardPieces, isWhiteTurn);

        SimulateMoveForSinglePiece(boardPieces[selectedPiecePosition.x, selectedPiecePosition.y], ref pieceAvailableMoves, kingPosition);
    }

    public static void SimulateMoveForSinglePiece(Piece piece, ref List<Vector2Int> moves, Vector2Int kingPosition)
    {
        // Save the current values, to reset after simulation
        int actualX = piece.currentX;
        int actualY = piece.currentY;
        bool actualMoveState = piece.hasMoved;
        bool actualDeadState = piece.isDead;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();

        // Iterate thrwo all moves, simulate them and check if we are in Check.
        for (int i = 0; i < moves.Count; i++)
        {
            int simX = moves[i].x;
            int simY = moves[i].y;

            Vector2Int kingPositionInSimulation = new Vector2Int(kingPosition.x, kingPosition.y);

            // Change King position if we simulated its move.
            if (piece.type == PieceType.King)
                kingPositionInSimulation = new Vector2Int(simX, simY);

            // Copy the Board [,] into simulationBoard
            Piece[,] simBoard = new Piece[CountSquaresX, CountSquaresY];
            List<Piece> simulationAttackingPieces = new List<Piece>();
            for (int x = 0; x < CountSquaresX; x++)
            {
                for (int y = 0; y < CountSquaresY; y++)
                {
                    if (boardPieces[x, y] != null)
                    {
                        simBoard[x, y] = boardPieces[x, y];

                        if (simBoard[x, y].color != piece.color)
                            simulationAttackingPieces.Add(simBoard[x, y]);
                    }
                }
            }

            // Simulate Move
            simBoard[actualX, actualY] = null;
            piece.currentX = simX;
            piece.currentY = simY;
            simBoard[simX, simY] = piece;

            // did one of the pieces got captured during the simulation?
            var deadPiece = simulationAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY);
            if (deadPiece != null)
                simulationAttackingPieces.Remove(deadPiece);
            
            // Get All the simulated attaking pieces moves
            List<Vector2Int> simulationMoves = new List<Vector2Int>();
            for (int x = 0; x < simulationAttackingPieces.Count; x++)
            {
                var pieceMoves = simulationAttackingPieces[x].GetValidMoves(ref simBoard);
                for (int y = 0; y < pieceMoves.Count; y++)
                {
                    simulationMoves.Add(pieceMoves[y]);
                }
            }

            // Check if the king in check after the simulation move.
            if (simulationMoves.Contains(kingPositionInSimulation))
            {
                movesToRemove.Add(moves[i]); // Remove this move from the valid moves for the piece.
            }
            
            // Reset the piece position.
            piece.currentX = actualX;
            piece.currentY = actualY;
        }

        // Remove from the current available move list.
        for (int i = 0; i < movesToRemove.Count; i++)
        {
            if (moves.Contains(movesToRemove[i]))
                moves.Remove(movesToRemove[i]);
            
            if (board.pieceSpecialMoves != null && board.pieceSpecialMoves.Contains(movesToRemove[i]))
                board.pieceSpecialMoves.Remove(movesToRemove[i]);
        }
    }

    public static int IsCheckMate()
    {
        PieceColor targetColor = isWhiteTurn ? PieceColor.White : PieceColor.Black;

        List<Piece> attackingPieces = new();
        List<Piece> defendingPieces = new();
        Piece king = null;

        for (int i = 0; i < CountSquaresX; i++)
        {
            for (int j = 0; j < CountSquaresY; j++)
            {
                if (boardPieces[i, j] != null)
                {
                    if (boardPieces[i, j].color == targetColor)
                    {
                        defendingPieces.Add(boardPieces[i, j]);
                        
                        if (boardPieces[i, j].type == PieceType.King)
                        {
                            king = boardPieces[i, j];
                        }
                    }
                    else
                    {
                        attackingPieces.Add(boardPieces[i, j]);
                    }
                }
            }
        }

        // Is the king Is attacked?
        if (King.IsInCheck(ref boardPieces, new(king.currentX, king.currentY), isWhiteTurn))
        {
            // King is under attack, Can we Block the check by Defending piece?
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetValidMoves(ref boardPieces);
                SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, new(king.currentX, king.currentY));

                if (defendingMoves.Count != 0)
                    return 0;
            }

            // if there are defending moves, and not deleted, this means, king is in checkmate.    
            return 1;
        }
        else
        {
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetValidMoves(ref boardPieces);
                SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, new(king.currentX, king.currentY));
                
                if (defendingMoves.Count != 0)
                    return 0;
            }

            // staleMate Condition.
            return 2;
        }
    }

    public static bool IsDraw()
    {
        List<Piece> whitePieces = new List<Piece>();
        List<Piece> blackPieces = new List<Piece>();

        for (int i = 0; i < CountSquaresX; i++)
        {
            for (int j = 0; j < CountSquaresY; j++)
            {
                if (boardPieces[i, j] != null)
                {
                    if (boardPieces[i, j].color == PieceColor.White)
                        whitePieces.Add(boardPieces[i, j]);
                    else
                        blackPieces.Add(boardPieces[i, j]);
                }
            }
        }

        Vector2Int kingPosition = King.FindKingPosition(boardPieces, isWhiteTurn);
        List<Vector2Int> pieceMoves = new();
        List<List<Vector2Int>> moves = new();

        if (isWhiteTurn)
        {
            for (int i = 0; i < whitePieces.Count; i++)
            {
                pieceMoves = whitePieces[i].GetValidMoves(ref boardPieces);
                SimulateMoveForSinglePiece(whitePieces[i], ref pieceMoves, kingPosition);
                moves.Add(pieceMoves);
            }
        } 
        else
        {
            for (int i = 0; i < blackPieces.Count; i++)
            {
                pieceMoves = blackPieces[i].GetValidMoves(ref boardPieces);
                SimulateMoveForSinglePiece(blackPieces[i], ref pieceMoves, kingPosition);
                moves.Add(pieceMoves);
            }
        }

        int movesCount = 0;
        for (int i = 0; i < moves.Count; i++)
        {
            movesCount += moves[i].Count;
        }

        if (!King.IsInCheck(ref boardPieces, kingPosition, isWhiteTurn) && movesCount == 0) return true;

        if (whitePieces.Count == 1 && blackPieces.Count == 1) return true;
        if (whitePieces.Count + blackPieces.Count == 3)
        {
            if (whitePieces.Count == 2)
            {
                for (int i = 0; i < whitePieces.Count; i++)
                {
                    if (whitePieces[i].type == PieceType.Bishop)
                        return true;
                    else if (whitePieces[i].type == PieceType.Knight)
                        return true;
                }
            }
            else if (blackPieces.Count == 2)
            {
                for (int i = 0; i < blackPieces.Count; i++)
                {
                    if (blackPieces[i].type == PieceType.Bishop)
                        return true;
                    else if (blackPieces[i].type == PieceType.Knight)
                        return true;
                }
            }
        }

        if (whitePieces.Count + blackPieces.Count == 5)
        {
            int bishop = 0;
            whitePieces.AddRange(blackPieces);
            for (int i = 0; i < whitePieces.Count; i++)
            {
                if (whitePieces[i].type == PieceType.Bishop)
                    bishop++;
            }
            if (bishop == 3) return true;
        }

        return false;
    }

    public static Piece[,] DeepCopy(Piece[,] original)
    {
        Piece[,] copy = new Piece[original.GetLength(0), original.GetLength(1)];
        for (int i = 0; i < original.GetLength(0); i++)
        {
            for (int j = 0; j < original.GetLength(1); j++)
            {
                if (original[i, j] != null)
                {
                    copy[i, j] = original[i, j];
                }
            }
        }
        return copy;
    }
}
