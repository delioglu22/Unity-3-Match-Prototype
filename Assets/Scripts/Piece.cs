using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Vector2Int location;
    public PieceType myPiece;

    public enum PieceType
    {
        Blue,
        Green,
        Red,
        Yellow,
        Purple,
        Pink,
        Box,
    }

    [SerializeField] float moveSpeed = 10f;

    public bool isMatched = false;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public Vector2 targetPosition; 

    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;
    
    private void Awake()
    {
        targetPosition = transform.position;
    }
    
    private void Update() 
    {
        if((Vector2)transform.position != targetPosition)
        {
           transform.position = Vector2.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime); 
        }
    }


    private void OnMouseDown() 
    {
        if(BoardManager.Instance.currentState != BoardManager.GameState.Move) return;

        firstTouchPosition = Input.mousePosition;
    }
    private void OnMouseUp() {
        if(BoardManager.Instance.currentState != BoardManager.GameState.Move) return;
        
        finalTouchPosition = Input.mousePosition;
        Vector2 direction = finalTouchPosition - firstTouchPosition;
        if(direction.magnitude > swipeResist)
        {
            swipeAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            CalculateAngle();
        }

    }

    private void CalculateAngle()
    {
        if(swipeAngle > -45 && swipeAngle <= 45) MovePiece(Vector2Int.right);
        else if(swipeAngle > 45 && swipeAngle <= 135) MovePiece(Vector2Int.up);
        else if(swipeAngle < -45 && swipeAngle >= -135) MovePiece(Vector2Int.down);
        else MovePiece(Vector2Int.left);
    }

    private void MovePiece(Vector2Int direction)
    {
        int targetX = location.x + direction.x;
        int targetY = location.y + direction.y;

        if(targetX < BoardManager.Instance.width && targetX >= 0 && targetY < BoardManager.Instance.height && targetY >= 0)
        {
            Piece otherPiece = BoardManager.Instance.board[targetX, targetY];
            
            (targetPosition, otherPiece.targetPosition) = (otherPiece.targetPosition, targetPosition);

            (BoardManager.Instance.board[location.x, location.y], BoardManager.Instance.board[otherPiece.location.x, otherPiece.location.y]) = 
            (BoardManager.Instance.board[otherPiece.location.x, otherPiece.location.y], BoardManager.Instance.board[location.x, location.y]);

            (location, otherPiece.location) = (otherPiece.location, location);

            StartCoroutine(BoardManager.Instance.CheckAndSwapBack(this, otherPiece));
            Debug.Log($"My piece: {myPiece}, Swap piece: {otherPiece.myPiece}");
        }
    }
}
