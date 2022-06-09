using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SoldierAgent : Agent
{
    [SerializeField] float rotateDumping = 1;
    [SerializeField] Transform takenBoxPos;

    Rigidbody rb;

    public float forceMultiplier = 10;
    Transform currentBox;
    Transform target;

    Vector2 screenSize;

    bool isTargetStollen = false;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        screenSize = new Vector2(15, 7);

        FindNewTarget();

        target.gameObject.GetComponent<AgentTarget>().SubscribeOnTargetTaken(this);

        //var camera = Camera.main;
        //screenSize = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    public override void OnEpisodeBegin()
    {
        isTargetStollen = false;

        if (!IsAgentOnScreen())
        {
            var spawnPosition = GetPointOutOfScreen();
            ///&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            transform.localPosition = new Vector3(spawnPosition.x, 0.5f, spawnPosition.y);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        //var size = screenSize - new Vector2(2, 2);

        //target.localPosition = new Vector3(Random.Range(-size.x, size.x), 0.5f, Random.Range(-size.y, size.y));
    }

    Vector2 GetPointOutOfScreen()
    {
        var spawnPosition = screenSize - new Vector2(1.5f, 1.5f);

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
        sensor.AddObservation(rb.velocity.z);
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
        return transform.localPosition.y > -2;
        //return transform.localPosition.x > -screenSize.x - 2 && transform.localPosition.x < screenSize.x + 2
        //            && transform.localPosition.y > -screenSize.y - 2 && transform.localPosition.y < screenSize.y + 2;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];

        rb.AddForce(controlSignal * forceMultiplier);

        var rotation = Quaternion.LookRotation(controlSignal);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateDumping);

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        // Reached target
        if (distanceToTarget < 1.5f)
        {
            if (!isTargetStollen)
            {
                PickUpBox();

                //target.localPosition = new Vector3(newTargePos.x, target.localPosition.y, newTargePos.y);

                AddReward(1f);

                return;
            }

            AddReward(1f);

            //забрати коробку

            Destroy(gameObject);

            //target = BoxesContainer.GetInstance().GetRandomBox();
            return;
            //EndEpisode();
        }

        else if (transform.localPosition.y < -2)
        {
            AddReward(isTargetStollen ? 1f : -3);

            Destroy(gameObject);
            return;

            //EndEpisode();
        }
    }

    void PickUpBox()
    {
        GetComponent<Animator>().SetTrigger("PickUpBox");

        isTargetStollen = true;

        var newTargePos = GetPointOutOfScreen();

        var agentTargetScript = target.gameObject.GetComponent<AgentTarget>();

        agentTargetScript.UnSubscribeOnTargetTaken(this);
        agentTargetScript.TargetTaken();

        var targetExitPoint = Instantiate(target.gameObject, new Vector3(newTargePos.x, target.localPosition.y, newTargePos.y), target.rotation).transform;
        targetExitPoint.GetComponent<MeshRenderer>().enabled = false;
        target = targetExitPoint;

        BoxesContainer.GetInstance().RemoveBox(currentBox);

        forceMultiplier /= 4;

        AddBoxToBody(currentBox);
    }

    public void FindNewTarget()
    {
        //якщо немає таргетів на сцені то зарандомити!!!

        target = BoxesContainer.GetInstance().GetRandomBox();
        currentBox = target;
    }

    void AddBoxToBody(Transform box)
    {
        box.parent = gameObject.transform;
        box.position = takenBoxPos.position;
        Destroy(box.GetComponent<Rigidbody>());
        box.GetComponent<BoxCollider>().enabled = false;
        //Time.timeScale = 0;
    }

    public void KillSoldier()
    {
        BoxesContainer.GetInstance().AddBox(currentBox);

        //забрати коробку

        currentBox.gameObject.AddComponent<Rigidbody>();
        currentBox.gameObject.GetComponent<BoxCollider>().enabled = true;

        currentBox.parent = null;

        Destroy(gameObject);
    }
}
