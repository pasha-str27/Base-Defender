using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SoldierAgent : Agent
{
    Rigidbody rb;

    public float forceMultiplier = 10;
    public Transform target;

    Vector3 screenSize;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        var camera = Camera.main;
        screenSize = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    public override void OnEpisodeBegin()
    {
        if (!IsAgentOnScreen())
        {
            transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, transform.position.z);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        var size = screenSize - new Vector3(2, 2, 0);

        target.position = new Vector3(Random.Range(size.x, -size.x), Random.Range(size.y, -size.y), target.position.z);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    bool IsAgentOnScreen()
    {
        return transform.localPosition.x > -screenSize.x - 2 && transform.localPosition.x < screenSize.x + 2
                    && transform.localPosition.y > -screenSize.y - 2 && transform.localPosition.y < screenSize.y + 2;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.y = actionBuffers.ContinuousActions[1];

        rb.AddForce(controlSignal * forceMultiplier, ForceMode.Impulse);

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        // Reached target
        if (distanceToTarget < 1)
        {
            AddReward(1.0f);
            EndEpisode();
        }
        else if (!IsAgentOnScreen())
        {
            SetReward(-0.1f);
            EndEpisode();
        }
    }
}
