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
    [SerializeField] GameObject tempPosPrefab;

    Rigidbody rb;

    public float forceMultiplier = 10;
    Transform currentBox;
    Transform target;
    Transform tempTarget;

    Vector2 screenSize;

    bool isTargetStollen = false;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        var collider = gameObject.GetComponent<BoxCollider>();
        collider.material.frictionCombine = PhysicMaterialCombine.Minimum;

        screenSize = new Vector2(15, 7);

        tempTarget = Instantiate(tempPosPrefab, Vector3.zero, Quaternion.identity).transform;

        InvokeRepeating("FindNewTarget", 0, 3);
    }

    public override void OnEpisodeBegin()
    {
        isTargetStollen = false;

        if (!IsAgentOnScreen())
        {
            var spawnPosition = GetPointOutOfScreen();
            transform.localPosition = new Vector3(spawnPosition.x, 0.5f, spawnPosition.y);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    Vector2 GetPointOutOfScreen()
    {
        var spawnPosition = screenSize + new Vector2(1.5f, 1.5f);

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
        sensor.AddObservation(target == null ? Vector3.zero : target.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rb ? rb.velocity.x : 0);
        sensor.AddObservation(rb ? rb.velocity.z : 0);
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
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (target == null)
            return;

        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];

        rb.AddForce(controlSignal * forceMultiplier, ForceMode.Impulse);

        var rotation = Quaternion.LookRotation(controlSignal);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateDumping);

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        // Reached target
        if (distanceToTarget < 1f && target.gameObject.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            if (!isTargetStollen)
            {
                BoxesContainer.GetInstance().RemoveBox(currentBox);
                PickUpBox();

                AddReward(1f);

                return;
            }

            AddReward(1f);

            Destroy(gameObject);

            return;
            //EndEpisode();
        }

        else if (!IsAgentOnScreen())
        {
            AddReward(isTargetStollen ? 1f : -3);

            BoxesContainer.GetInstance().DeleteBox();
            Destroy(target.gameObject);
            Destroy(gameObject);

            return;

            //EndEpisode();
        }
    }

    void PickUpBox()
    {
        CancelInvoke();

        GetComponent<Animator>().SetTrigger("PickUpBox");

        isTargetStollen = true;

        var newTargePos = GetPointOutOfScreen();

        var agentTargetScript = target.gameObject.GetComponent<AgentTarget>();

        agentTargetScript.UnSubscribeOnTargetTaken(this);
        agentTargetScript.TargetTaken();

        var targetExitPoint = Instantiate(target.gameObject, new Vector3(newTargePos.x, target.localPosition.y, newTargePos.y), target.rotation).transform;
        targetExitPoint.GetComponent<MeshRenderer>().enabled = false;
        target = targetExitPoint;

        forceMultiplier /= 2;

        AddBoxToBody(currentBox);
    }

    public void FindNewTarget()
    {
        //якщо немає таргетів на сцені то зарандомити!!!
        if (BoxesContainer.GetInstance().BoxesCount() == 0)
        {
            tempTarget.position = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-5f, 5f));
            target = tempTarget;
            return;
        }

        if (target != null)
            target.gameObject.GetComponent<AgentTarget>()?.UnSubscribeOnTargetTaken(this);

        target = BoxesContainer.GetInstance().GetNearestBox(transform.position);
        currentBox = target;

        if(target)
            target.gameObject.GetComponent<AgentTarget>().SubscribeOnTargetTaken(this);
    }

    void AddBoxToBody(Transform box)
    {
        box.parent = gameObject.transform;
        box.position = takenBoxPos.position;
        Destroy(box.GetComponent<Rigidbody>());
        box.GetComponent<BoxCollider>().enabled = false;
    }

    public void KillSoldier()
    {
        currentBox.GetComponent<AgentTarget>().UnSubscribeOnTargetTaken(this);
        BoxesContainer.GetInstance().AddBox(currentBox);

        currentBox.gameObject.AddComponent<Rigidbody>();
        currentBox.gameObject.GetComponent<BoxCollider>().enabled = true;

        currentBox.parent = null;

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Explosion"))
            KillSoldier();
    }
}
