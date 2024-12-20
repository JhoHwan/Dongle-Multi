using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviourSingleton<InGameUIManager>
{
    private List<InGamePlayerNameText> _playerNameTextList;
    [HideInInspector] public IngameTimer Timer;
    [HideInInspector] public InGameScoreBoard ScoreBoard;
    public List<InGameNextDongle> NextDongles;

    public override void Awake()
    {
        base.Awake();

        Timer = FindObjectOfType<IngameTimer>();
        ScoreBoard = FindObjectOfType<InGameScoreBoard>();
        _playerNameTextList = new List<InGamePlayerNameText>(FindObjectsOfType<InGamePlayerNameText>());
    }

    public void SetPlayerNameText(int playerNum, string name)
    {
        foreach (var player in _playerNameTextList)
        {
            if (player.PlayerNum == playerNum)
            {
                player.SetName(name);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
