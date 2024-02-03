using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private CheckpointManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            manager.PlayerThrough(this, other.transform);
        }
    }

    public void SetCheckpoint(CheckpointManager m)
    {
        this.manager = m;  
    }
}
