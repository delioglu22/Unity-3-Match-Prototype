using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] GameObject[] piecePrefabs;
    
    public Piece[,] board;    


    void Start()
    {
        board = new Piece[width, height];
        SetUpBoard();
    }
    void SetUpBoard()
    {
        var xOffset = (width - 1) / 2f;
        var yOffset = (height - 1) / 2f;
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
            }
        }
    }
}
