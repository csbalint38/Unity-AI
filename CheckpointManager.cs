using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public EventHandler OnPlayerCorrectCheckoint;
    public EventHandler OnPlayerWrongCheckoint;
    public List<Transform> cars;

    private List<Checkpoint> checkpointList = new List<Checkpoint>();
    private List<int> nextCheckpointIndex;

    private void Awake()
    {
        Transform checkpoints = transform.Find("Checkpoints");

        foreach(Transform checkpoint in checkpoints)
        {
            Checkpoint cp = checkpoint.GetComponent<Checkpoint>();
            cp.SetCheckpoint(this);
            checkpointList.Add(cp);
        }

        nextCheckpointIndex = new List<int>();

        foreach(Transform car in cars)
        {
            nextCheckpointIndex.Add(0);
        }
    }

    public void PlayerThrough(Checkpoint cp, Transform car)
    {
        int nextIndex = nextCheckpointIndex[cars.IndexOf(car)];

        if(checkpointList.IndexOf(cp) == nextIndex)
        {
            nextCheckpointIndex[cars.IndexOf(car)] = (nextIndex + 1) % checkpointList.Count;
            OnPlayerCorrectCheckoint?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnPlayerWrongCheckoint?.Invoke(this, EventArgs.Empty);
        }
    }
}
