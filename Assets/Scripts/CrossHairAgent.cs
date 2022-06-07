using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CrossHairAgent : Agent
{
    [SerializeField] Transform _target;

    Transform _transform;

    Rigidbody rb;
    [SerializeField] Transform moveZone;

    [SerializeField] float forceMultiplier = 10;

    public float currentScale;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        _transform = transform;

        _transform.localPosition = Vector3.zero;
    }

    public override void OnEpisodeBegin()
    {
        var colliderSize = moveZone.localScale / 2;
        var pos = _transform.localPosition;
        var parentPos = _transform.parent.position;

        if (Vector3.Distance(pos, _target.localPosition) > moveZone.localScale.x + 1)
        {
            rb.velocity = Vector2.zero;
            _transform.localPosition = Vector3.zero;
        }

        colliderSize -= Vector3.one / 3;

        _target.localPosition = new Vector3(Random.Range(-colliderSize.x, colliderSize.x),
                            Random.Range(-colliderSize.y, colliderSize.y), 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(_target.localPosition);
        sensor.AddObservation(_transform.localPosition);

        sensor.AddObservation(moveZone.localScale.x);

        //// Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];

        forceMultiplier = moveZone.localScale.x / 10f;

        rb.AddForce(controlSignal * forceMultiplier);

        //var colliderSize = moveZone.size / 2 + Vector2.one / 2;
        var pos = _transform.localPosition;

        // Rewards
        float distanceToTarget = Vector3.Distance(pos, _target.localPosition);

        // Reached target
        if (distanceToTarget < 0.3f)
        {
            AddReward(1.0f);
            EndEpisode();

            return;
        }

        //if (distanceToTarget > moveZone.localScale.x + 1)
        //{
        //    AddReward(-1.0f);
        //    EndEpisode();

        //    return;
        //}

        //if (distanceToTarget > 3)
        //{
        //    AddReward(-1);
        //    EndEpisode();
        //}
    }
}
