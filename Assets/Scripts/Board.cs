using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.GraphicsBuffer;

public class Board : MonoBehaviour
{
    // Reference The Square Graphics.
    [Header("Board Squares")]
    [SerializeField] private Square squarePrefab;
    public static readonly float squareSize = 2f;

    protected Square[,] boardSquares = new Square[8, 8];
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
    protected List<Piece> deadWhitePieces = new List<Piece>();
    protected List<Piece> deadBlackPieces = new List<Piece>();

    // Store the position of the selected Chess Piece, So you can move it.
    private Vector2Int selectedPiecePosition = new Vector2Int(-1, -1);
    
    // Store the Available Moves for the selected piece.
    private List<Vector2Int> pieceAvailableMoves = new List<Vector2Int>();

    // Store Special Moves Like Castling.
    private List<Vector2Int> pieceSpecialMoves = new List<Vector2Int>();

    // Store the Move Count, So you can change the Squares Colors after the second move.
    private int moveCount = 1;

    // Store The White Turn to play, then you can control turns for players.
    private bool isWhiteTurn;

    private bool gameEnded = false;

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
    }

    private void Update()
    {
        if (gameEnded) return;

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

                            // Highlight The Availble Moves Squares.
                            this.HighlightAvailableMovesSquares();
                        }
                    }
                    
                }
                else // Second click - move piece
                {
                    Piece piece = boardPieces[selectedPiecePosition.x, selectedPiecePosition.y];
                    this.MovePieceTo(piece, xIndex, yIndex);

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
    private void MovePieceTo(Piece piece, int targetX, int targetY)
    {
        if (boardPieces[piece.currentX, piece.currentY] == null) return; // No piece to move

        // Get The Squares in this move, so you can change their colors.
        Square previousSquare = boardSquares[piece.currentX, piece.currentY];
        Square targetSquare = boardSquares[targetX, targetY];

        // Validate move according to piece rules
        if (!piece.IsValidMove(targetX, targetY))
        {
            gameManager.PlayAudioClip(gameManager.notifyAudioClip);
            ResetBoardColors(null, null);
            return;
        }

        // change turns of the movement.
        isWhiteTurn = !isWhiteTurn;

        // Remove the colors of available moves.
        this.ResetBoardColors(previousSquare, targetSquare);

        // Is The Move an attack?
        bool isAttacking = false;

        // Is There a piece on the target position?
        if (boardPieces[targetX, targetY] != null) // Target position is occupied
        {
            // Target Position is Occupied with a piece in the same team.
            if (boardPieces[targetX, targetY].color == piece.color)
            {
                gameManager.PlayAudioClip(gameManager.notifyAudioClip);
                this.ResetBoardColors(null, null);
                return;
            }
            else // Target Position is Occupied with an enemy piece, So eat it.
            {
                isAttacking = true;
                if (boardPieces[targetX, targetY].color == PieceColor.White)
                    deadWhitePieces.Add(boardPieces[targetX, targetY]);
                else
                    deadBlackPieces.Add(boardPieces[targetX, targetY]);

                // Play Attacking Audio Clip.
                gameManager.PlayAudioClip(gameManager.captureAudioClip);

                this.PositionDeadPiece(boardPieces[targetX, targetY]);

                if (boardPieces[targetX, targetY].type == PieceType.King)
                    CheckMate(piece.color);
            }
        }

        // Check For King Castling
        if (piece.type == PieceType.King && pieceSpecialMoves.Contains(new Vector2Int(targetX, targetY)))
        {
            Piece rook = (targetX > piece.currentX) ? boardPieces[targetX + 1, targetY] : boardPieces[targetX - 2, targetY];
            piece.CastleKing(rook, ref boardPieces);
        }

        // Move the selected piece in the array.
        boardPieces[targetX, targetY] = piece;
        boardPieces[piece.currentX, piece.currentY] = null;
        
        piece.hasMoved = true;
        
        // Position the Piece into the target Position.
        PositionSinglePiece(piece, targetX, targetY);

        // Play Movement Audio Clip.
        if (!isAttacking)
            gameManager.PlayAudioClip(gameManager.moveAudioClip);

        moveCount++;

        // Make A Square Feedback (Change its Color) for every move.
        this.ChangeMoveColors(previousSquare, targetSquare);
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
        return board[xIndex, yIndex] == null;
    }

    private void CheckMate(PieceColor color)
    {
        gameEnded = true;
        gameManager.EndGame((int)color + 1);
    }
}
