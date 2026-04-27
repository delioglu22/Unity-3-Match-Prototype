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
    
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;

    private void OnMouseDown() 
    {
        firstTouchPosition = Input.mousePosition;
    }
    private void OnMouseUp() {
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
        if(swipeAngle > -45 && swipeAngle <= 45) Debug.Log("Swiped Right");
        else if(swipeAngle > 45 && swipeAngle <= 135) Debug.Log("Swiped Up");
        else if(swipeAngle < -45 && swipeAngle >= -135) Debug.Log("Swiped Down");
        else Debug.Log("Swiped Left");
    }
}
