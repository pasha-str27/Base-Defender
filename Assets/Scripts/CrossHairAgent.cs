using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CrossHairAgent : Agent
{
    [SerializeField] Transform _target;

    Transform _transform;

    Rigidbody2D rb;
    [SerializeField] BoxCollider2D moveZone;

    [SerializeField] float forceMultiplier = 10;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        _transform = transform;
    }

    public override void OnEpisodeBegin()
    {
        var colliderSize = moveZone.size / 2;
        var pos = _transform.localPosition;
        var parentPos = _transform.parent.position;

        if (Mathf.Abs(pos.y) > colliderSize.y || Mathf.Abs(pos.x) > colliderSize.x)
        {
            rb.velocity = Vector2.zero;
            _transform.position = parentPos;
        }

        colliderSize -= Vector2.one / 3;

        _target.position = new Vector3(Random.Range(parentPos.x - colliderSize.x, parentPos.x + colliderSize.x),
                            Random.Range(parentPos.y - colliderSize.y, parentPos.y + colliderSize.y), _target.position.z);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(_target.localPosition);
        sensor.AddObservation(_transform.localPosition);

        sensor.AddObservation(moveZone.size);

        //// Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.y = actionBuffers.ContinuousActions[1];
        rb.AddForce(controlSignal * forceMultiplier);

        var colliderSize = moveZone.size / 2 + Vector2.one / 2;
        var pos = _transform.localPosition;

        // Rewards
        float distanceToTarget = Vector3.Distance(pos, _target.localPosition);

        // Reached target
        if (distanceToTarget < 0.8f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        else if (Mathf.Abs(pos.y) > colliderSize.y || Mathf.Abs(pos.x) > colliderSize.x)
        {
            AddReward(-5.0f);
            EndEpisode();
        }
    }
}
