using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameScoreBoard : MonoBehaviour
{
    [SerializeField] private Text _player1_Score;
    [SerializeField] private Text _player2_Score;

    public void SetPlayer1Socre(int score)
    {
        _player1_Score.text = score.ToString();
    }

    public void SetPlayer2Socre(int score)
    {
        _player2_Score.text = score.ToString();
    }
}

