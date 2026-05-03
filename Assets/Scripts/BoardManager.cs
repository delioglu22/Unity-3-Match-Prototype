using UnityEngine;

public class BoardManager : MonoBehaviour
{
    
    public static BoardManager Instance { get; private set; }

    private void Awake() 
    {
        Instance = this;    
    }

    [field: SerializeField] public int width { get; private set; }
    [field: SerializeField] public int height { get; private set; }
    [SerializeField] GameObject[] piecePrefabs;
    
    public Piece[,] board;    
    private float xOffset;
    private float yOffset;


    void Start()
    {
        board = new Piece[width, height];
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
                                matchFound = true;
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
                                matchFound = true;
                            }
                        }
                    }
                }
            }
        }
        if(matchFound == true)
        {
            DestroyMatches();
            ApplyGravity();
            RefillBoard();
            Debug.Log("DESTROY OBJECTS");

            CheckForMatches();
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
                    board[i, j - nullCount].transform.position = new Vector2(i - xOffset, j - nullCount - yOffset);
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
                    Vector2 tempPosition = new Vector2(i - xOffset, j - yOffset);
                    GameObject piece = Instantiate(piecePrefabs[randomIndex], tempPosition, Quaternion.identity);
                    board[i, j] = piece.GetComponent<Piece>();
                    board[i, j].location = new Vector2Int(i,j);
                }
            }
        }
    }
}
