using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent :  Agent
{
    public Transform goal;
    public float moveSpeed = 3f;

    public Material winMaterial;
    public Material loseMaterial;
    public MeshRenderer[] floor;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(1f, 4f), .5f, Random.Range(1f, 3f));
        goal.localPosition = new Vector3(Random.Range(1f, 4f), .3f, Random.Range(5f, 9f));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goal.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void ChangeFloor(char mat) {
        foreach(MeshRenderer mr in floor) {
            if(mat == 'L')
            {
                mr.material = loseMaterial;
            }
            else if(mat == 'W')
            {
                mr.material = winMaterial;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            SetReward(1f);
            ChangeFloor('W');
            EndEpisode();
        }
        if (other.CompareTag("Wall"))
        {
            SetReward(-1f);
            ChangeFloor('L');
            EndEpisode();
        }
    }
}
