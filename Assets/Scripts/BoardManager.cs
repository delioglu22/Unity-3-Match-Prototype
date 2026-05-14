using System.Collections;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    
    public static BoardManager Instance { get; private set; }
    public enum GameState {Move, Wait, GameOver}
    public GameState currentState = GameState.Move;
    public int maxMoves = 15;

    [field: SerializeField] public int width { get; private set; }
    [field: SerializeField] public int height { get; private set; }
    [SerializeField] GameObject[] piecePrefabs;

    public Piece[,] board;
    private float xOffset;
    private float yOffset;

    private int movesLeft;

    public int currentScore = 0;
    private int scoreMultiplier = 1;
    private int baseScoreValue = 10;

    private void Awake() 
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        UIManager.Instance.UpdateScore(currentScore);

    }


    void Start()
    {
        board = new Piece[width, height];
        movesLeft = maxMoves;
        UIManager.Instance.UpdateMoves(movesLeft);

        SetUpBoard();
    }
    void SetUpBoard()
    {
        xOffset = (width - 1) / 2f;
        yOffset = (height - 1) / 2f;
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                int randomIndex = Random.Range(0, piecePrefabs.Length);
                Vector2 tempPosition = new Vector2(i - xOffset, j - yOffset);
                while ((i >= 2 && piecePrefabs[randomIndex].GetComponent<Piece>().myPiece == board[i-1, j].myPiece && piecePrefabs[randomIndex].GetComponent<Piece>().myPiece == board[i-2, j].myPiece) 
                || (j >= 2 && piecePrefabs[randomIndex].GetComponent<Piece>().myPiece == board[i, j-1].myPiece && piecePrefabs[randomIndex].GetComponent<Piece>().myPiece == board[i, j-2].myPiece)
                )
                {
                    randomIndex = Random.Range(0, piecePrefabs.Length);
                }
                GameObject piece = Instantiate(piecePrefabs[randomIndex], tempPosition, Quaternion.identity);
                board[i, j] = piece.GetComponent<Piece>();
                board[i, j].location = new Vector2Int(i,j);

            }
        }
    }
    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Piece currentPiece = board[i,j];
                if (currentPiece != null && currentPiece.isMatched)
                {
                    int pieceScore = baseScoreValue * scoreMultiplier;

                    if(currentPiece.isHorizontalMatch && currentPiece.isVerticalMatch)
                    {
                        int bonusScore = 50 * scoreMultiplier; 
                        pieceScore += bonusScore;
                    }

                    currentScore += pieceScore;

                    Destroy(currentPiece.gameObject);
                    
                    board[i, j] = null;
                }
            }
        }
        UIManager.Instance.UpdateScore(currentScore);
    }
    private void ApplyGravity()
    {
        for(int i = 0; i < width; i++)
        {
            int nullCount = 0;
            
            for(int j = 0; j < height; j++)
            {
                if(board[i,j] == null) nullCount++;
                else if(nullCount > 0)
                {
                    board[i, j].location.y = board[i, j].location.y - nullCount;
                    board[i, j - nullCount] = board[i, j];
                    board[i, j] = null;
                    board[i, j - nullCount].targetPosition = new Vector2(i - xOffset, j - nullCount - yOffset);
                }
            }
        }
    }
    private void RefillBoard()
    {
        for(int i = 0; i < width; i++)
        {            
            for(int j = 0; j < height; j++)
            {
                if(board[i, j] == null)
                {
                    int randomIndex = Random.Range(0, piecePrefabs.Length);

                    Vector2 finalPosition = new Vector2(i - xOffset, j - yOffset);
                    Vector2 spawnPosition = new Vector2(i - xOffset, j - yOffset + 5f);

                    GameObject piece = Instantiate(piecePrefabs[randomIndex], spawnPosition, Quaternion.identity);

                    board[i, j] = piece.GetComponent<Piece>();
                    board[i, j].location = new Vector2Int(i,j);

                    board[i, j].targetPosition = finalPosition;
                }
            }
        }
    }
    private IEnumerator ProcessBoardRoutine()
    {
        currentState = GameState.Wait;

        DestroyMatches();
        yield return new WaitForSeconds(0.3f);
        ApplyGravity();
        yield return new WaitForSeconds(0.3f);
        RefillBoard();
        yield return new WaitForSeconds(0.3f);
        
        if(HasMatches())
        {
            scoreMultiplier++;

            StartCoroutine(ProcessBoardRoutine());
        }
        else
        {
            if(movesLeft <= 0) currentState = GameState.GameOver;
            else currentState = GameState.Move;
        }
    }

    public IEnumerator CheckAndSwapBack(Piece p1, Piece p2)
    {
        currentState = GameState.Wait;
        yield return new WaitForSeconds(0.4f);
        
        if(HasMatches())
        {
            movesLeft--;
            UIManager.Instance.UpdateMoves(movesLeft);

            scoreMultiplier = 1;
            
            StartCoroutine(ProcessBoardRoutine());
        }
        else
        {
            (p1.targetPosition, p2.targetPosition) = (p2.targetPosition, p1.targetPosition);
            (board[p1.location.x, p1.location.y], board[p2.location.x, p2.location.y]) = (board[p2.location.x, p2.location.y], board[p1.location.x, p1.location.y]);
            (p1.location, p2.location) = (p2.location, p1.location);

            yield return new WaitForSeconds(0.4f);
            currentState = GameState.Move;
        }

    }
    private bool HasMatches()
    {
        bool matchFound = false;

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Piece currentPiece = board[i,j];
                if(currentPiece != null)
                {
                    if(i < width - 2)
                    {
                        Piece right1 = board[i+1, j];
                        Piece right2 = board[i+2, j];
                        if(right1 != null && right2 != null && 
                            right1.myPiece == currentPiece.myPiece && 
                            right2.myPiece == currentPiece.myPiece)
                        {
                            currentPiece.isMatched = true; currentPiece.isHorizontalMatch = true;
                            right1.isMatched = true; right1.isHorizontalMatch = true;
                            right2.isMatched = true; right2.isHorizontalMatch = true;

                            int nextX = i + 3;
                            
                            while (nextX < width && board[nextX, j] != null && board[nextX, j].myPiece == currentPiece.myPiece)
                            {
                                board[nextX, j].isMatched = true; board[nextX,j].isHorizontalMatch = true;
                                nextX++;
                            }
                            matchFound = true;
                        
                        }
                    }

                    if(j < height - 2)
                    {
                        Piece up1 = board[i, j+1];
                        Piece up2 = board[i, j+2];
                        if(up1 != null && up2 != null && 
                            up1.myPiece == currentPiece.myPiece && 
                            up2.myPiece == currentPiece.myPiece)
                        {
                            currentPiece.isMatched = true; currentPiece.isVerticalMatch = true;
                            up1.isMatched = true; up1.isVerticalMatch = true;
                            up2.isMatched = true; up2.isVerticalMatch = true;

                            int nextY = j + 3;
                            while (nextY < height && board[i, nextY] != null && board[i, nextY].myPiece == currentPiece.myPiece)
                            {
                                board[i, nextY].isMatched = true; board[i, nextY].isVerticalMatch = true;
                                nextY++;
                            }
                            matchFound = true;
                        
                        }
                    }
                }
            }
        }
        return matchFound;
    }
}
