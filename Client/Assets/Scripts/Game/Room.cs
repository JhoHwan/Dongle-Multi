using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviourSingleton<Room>
{
    public int myPlayerID;
    public int player1_ID;
    public int roomID;

    public override void Awake()
    {
        base.Awake();
    }


}
