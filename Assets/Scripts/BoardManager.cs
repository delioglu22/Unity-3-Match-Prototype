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

    private void Awake() 
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

    }


    void Start()
    {
        board = new Piece[width, height];
        movesLeft = maxMoves;
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
    public void CheckForMatches()
    {
        bool matchFound = false;

        if(HasMatches()) matchFound = true;
        
        if(matchFound == true)
        {
            StartCoroutine(ProcessBoardRoutine());
        }
        else
        {
            if(movesLeft <= 0)
            {
                currentState = GameState.GameOver;
                Debug.Log("GAME OVER!");
            }
            else
            {
                currentState = GameState.Move;
            }
        }
    }
    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (board[i, j] != null && board[i, j].isMatched)
                {
                    Destroy(board[i, j].gameObject);
                    
                    board[i, j] = null;
                }
            }
        }
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
        CheckForMatches();
    }

    public IEnumerator CheckAndSwapBack(Piece p1, Piece p2)
    {
        currentState = GameState.Wait;
        yield return new WaitForSeconds(0.4f);
        
        if(HasMatches())
        {
            movesLeft--;
            Debug.Log("Moves left: " + movesLeft);
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
                        if(right1 != null && right2 != null)
                        {
                            if(currentPiece.myPiece == right1.myPiece && currentPiece.myPiece == right2.myPiece)
                            {
                                currentPiece.isMatched = true;
                                right1.isMatched = true;
                                right2.isMatched = true;
                                return true;
                            }
                        }
                    }

                    if(j < height - 2)
                    {
                        Piece up1 = board[i, j+1];
                        Piece up2 = board[i, j+2];
                        if(up1 != null && up2 != null)
                        {
                            if(currentPiece.myPiece == up1.myPiece && currentPiece.myPiece == up2.myPiece)
                            {
                                currentPiece.isMatched = true;
                                up1.isMatched = true;
                                up2.isMatched = true;
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
}
