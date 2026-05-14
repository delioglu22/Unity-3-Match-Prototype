using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } 

    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI scoreText;    

    void Awake()
    {
        Instance = this;
    }

    public void UpdateMoves(int movesLeft)
    {
        movesText.text = "Moves: " + movesLeft;
    }
    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }
}
