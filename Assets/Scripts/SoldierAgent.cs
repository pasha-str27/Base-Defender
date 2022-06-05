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

    bool isTargetStollen = false;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        var camera = Camera.main;
        screenSize = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    public override void OnEpisodeBegin()
    {
        isTargetStollen = false;

        if (!IsAgentOnScreen())
        {
            var spawnPosition = GetPointOutOfScreen();

            transform.position = new Vector3(spawnPosition.x, spawnPosition.y, transform.position.z);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        var size = screenSize - new Vector3(2, 2, 0);

        target.position = new Vector3(Random.Range(size.x, -size.x), Random.Range(size.y, -size.y), target.position.z);
    }

    Vector2 GetPointOutOfScreen()
    {
        var spawnPosition = screenSize + new Vector3(1.5f, 1.5f, 0);

        switch (Random.Range(0, 4))
        {
            case 0:
                spawnPosition.y = Random.Range(-spawnPosition.y, spawnPosition.y);
                break;
            case 1:
                spawnPosition.x *= -1;
                spawnPosition.y = Random.Range(-spawnPosition.y, spawnPosition.y);
                break;
            case 2:
                spawnPosition.x = Random.Range(-spawnPosition.x, spawnPosition.x);
                break;
            case 3:
                spawnPosition.y *= -1;
                spawnPosition.x = Random.Range(-spawnPosition.x, spawnPosition.x);
                break;
        }

        return spawnPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
        sensor.AddObservation(isTargetStollen);
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

        controlSignal.Normalize();

        rb.AddForce(controlSignal * forceMultiplier, ForceMode.Impulse);

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        // Reached target
        if (distanceToTarget < 1)
        {
            if(!isTargetStollen)
            {
                isTargetStollen = true;

                var newTargePos = GetPointOutOfScreen();
                target.position = new Vector3(newTargePos.x, newTargePos.y, target.position.z);

                AddReward(1f);

                return;
            }


            AddReward(1f);
            EndEpisode();

            return;
        }
        
        if (!IsAgentOnScreen())
        {
            AddReward(isTargetStollen ? 0.1f : 0);
            EndEpisode();
        }
    }
}
