using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    Queue<Action> jobQueue = new Queue<Action>();
    [field: SerializeField] public ushort PlayerID { get; set; }

    [HideInInspector] public Room Room = null;

    public int Seed { get; private set; }

    public override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

    public void CreateJob(Action job)
    {
        jobQueue.Enqueue(job);
    }

    private void Update()
    {
        while (jobQueue.Count > 0)
        {
            Action action = jobQueue.Dequeue();
            action();
        }
    }
}
