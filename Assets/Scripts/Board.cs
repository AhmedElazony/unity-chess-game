using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
    [Header("Board Squares")]
    // Reference The Square Graphics.
    [SerializeField] private Square squarePrefab;
    public static readonly float squareSize = 2f;

    private Square[,] boardSquares = new Square[8, 8];
    private Color[,] squareColors = new Color[8, 8];

    [Header("Board Pieces")]
    // Store The Chess Pieces Prefabs.
    [SerializeField] private GameObject[] pieces;

    // Store the Board Pieces Positions.
    private Piece[,] boardPieces;

    // Store the position of the selected Chess Piece, So you can move it.
    private Vector2Int selectedPiecePosition = new Vector2Int(-1, -1);

    // Store the Move Count, So you can change the Squares Colors after the second move.
    private int moveCount = 1;

    // Count the X, Y Coords (Board Width and Height).
    private const int CountPiecesX = 8;
    private const int CountPiecesY = 8;

    void Awake()
    {
        this.GenerateBoard();

        this.SpawnPieces();

        this.PositionPieces();
    }

    private void Update()
    {
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
                        selectedPiecePosition = new Vector2Int(xIndex, yIndex);
                    }
                }
                else
                {
                    // Second click - move piece
                    MovePieceTo(selectedPiecePosition.x, selectedPiecePosition.y, xIndex, yIndex);
                    selectedPiecePosition = new Vector2Int(-1, -1); // Reset selected piece
                }
            }
        }
    }

    // Create The Chess Board Squares.
    private void GenerateBoard()
    {
        for (int i = 0; i < CountPiecesX; i++)
        {
            for (int j = 0; j < CountPiecesY; j++)
            {
                Square square = Instantiate(squarePrefab, new Vector3(i * squareSize, j * squareSize), Quaternion.identity, transform);
                boardSquares[i, j] = square;

                bool isOffset = (i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0);

                square.Initialize(isOffset);
                squareColors[i, j] = square.ObjectColor;
            }
        }
    }

    // Spawn Chess Pieces.
    private void SpawnPieces()
    {
        boardPieces = new Piece[CountPiecesX, CountPiecesY];

        PieceType[] pieceTypes = new PieceType[] { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };
        
        // White Pieces.
        this.FillPiecesArray(pieceTypes, PieceColor.White);
        
        // Black Pieces.
        this.FillPiecesArray(pieceTypes, PieceColor.Black);
    }
    private Piece SpawnSinglePiece(PieceType type, PieceColor color)
    {
        GameObject pieceType = pieces[(int)type - 1];
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

    // Position Chess Pieces On The Board for the first time.
    private void PositionPieces()
    {
        for (int xCoord = 0; xCoord < CountPiecesX; xCoord++)
        {
            for (int yCoord = 0; yCoord < CountPiecesY; yCoord++)
            {
                if (boardPieces[xCoord, yCoord] != null)
                {
                    PositionSinglePiece(boardPieces[xCoord, yCoord], xCoord, yCoord);
                }
            }
        }
    }
    private void PositionSinglePiece(Piece piece, int xCoord, int yCoord)
    {
        piece.currentX = xCoord;
        piece.currentY = yCoord;
        piece.transform.position = new Vector3(xCoord * squareSize, yCoord * squareSize, -1);
    }

    // Move Pieces
    private void MovePieceTo(int previousX, int previousY, int targetX, int targetY)
    {
        if (boardPieces[previousX, previousY] == null) return; // No piece to move
        
        if (boardPieces[targetX, targetY] != null) return; // Target position is occupied

        Piece piece = boardPieces[previousX, previousY];

        Square previousSquare = boardSquares[previousX, previousY];
        Square targetSquare = boardSquares[targetX, targetY];

        // Validate move according to piece rules
        //if (!piece.IsValidMove(previousX, previousY, targetX, targetY)) return;

        boardPieces[targetX, targetY] = piece;
        boardPieces[previousX, previousY] = null;

        // Position the Piece into the target Position.
        this.PositionSinglePiece(piece, targetX, targetY);

        // Increment The Move Count.
        moveCount++;

        // Make A Square Feedback (Change its Color) for every move.
        this.ChangeMoveColors(previousSquare, targetSquare);
    }

    private void ChangeMoveColors(Square previous, Square target)
    {
        previous.ObjectColor = Color.gray;
        target.ObjectColor = Color.gray;

        // Reset The Colors after the second move.
        if (moveCount == 2)
        {
            // Reset All Squares Except the two in this current move.
            ResetBoardColors(previous, target);
            moveCount = 1;
        }
    }

    private void ResetBoardColors(Square first, Square second)
    {
        // Reset the colors of the squares
        for (int i = 0; i < CountPiecesX; i++)
        {
            for (int j = 0; j < CountPiecesY; j++)
            {
                if (boardSquares[i, j] != first && boardSquares[i, j] != second && boardSquares[i, j].ObjectColor == Color.gray)
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

        if (xIndex >= 0 && xIndex < CountPiecesX && yIndex >= 0 && yIndex < CountPiecesY)
        {
            return new Vector2Int(xIndex, yIndex);
        }

        return new Vector2Int(-1, -1);
    }
}
