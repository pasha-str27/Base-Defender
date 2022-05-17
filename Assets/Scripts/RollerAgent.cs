using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    Rigidbody rb;

    public float forceMultiplier = 10;
    public Transform cubeTarget;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        print(89);

        if (transform.position.y < 0)
        {
            transform.position = transform.parent.position + new Vector3(0, 0.5f, 0);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        cubeTarget.position = new Vector3(Random.Range(transform.parent.position.x - 4f, transform.parent.position.x + 4f), 0.5f, Random.Range(transform.parent.position.z - 4f, transform.parent.position.z + 4f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(cubeTarget.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rb.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, cubeTarget.localPosition);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        else if (this.transform.localPosition.y < 0)
        {
            SetReward(-10.0f);
            EndEpisode();
        }
    }
}
