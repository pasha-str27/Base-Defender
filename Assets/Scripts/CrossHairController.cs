using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CrossHairController : Agent
{
    protected CrossHairActions playerInput;

    [SerializeField] Transform _target;

    Transform _transform;

    Rigidbody2D rb;

    [SerializeField] float forceMultiplier = 10;


    private void Update()
    {
        Vector2 movement = playerInput.Player.Move.ReadValue<Vector2>();

        Vector2 dir = Vector2.zero;

        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            dir = movement.x > 0 ? Vector2.right : Vector2.left;
        else
        {
            if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
                dir = movement.y > 0 ? Vector2.up : Vector2.down;
        }

        _transform.Translate(dir * Time.deltaTime * 5);
    }

    //private void OnDisable()
    //{
    //    playerInput.Disable();
    //}

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        _transform = transform;
        playerInput = new CrossHairActions();

        playerInput.Enable();
    }

    public override void OnEpisodeBegin()
    {
        //if (transform.position.y < 0)
        //{
        //    _transform.position = transform.parent.position/* + new Vector3(0, 0.5f, 0)*/;
        //    rb.velocity = Vector2.zero;
        //    //rb.angularVelocity = Vector2.zero;
        //}

        if (Mathf.Abs(_transform.localPosition.y) > 15 || Mathf.Abs(_transform.localPosition.x) > 15)
        {
            rb.velocity = Vector2.zero;
            _transform.position = Vector2.zero;
        }

        _target.position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), _target.position.z);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(_target.localPosition);
        sensor.AddObservation(_transform.localPosition);

        //// Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        Vector2 movement = playerInput.Player.Move.ReadValue<Vector2>();

        continuousActionsOut[0] = movement.x;
        continuousActionsOut[1] = movement.y;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.y = actionBuffers.ContinuousActions[1];
        rb.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(_transform.localPosition, _target.localPosition);

        // Reached target
        if (distanceToTarget < 0.5f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        else if (Mathf.Abs(_transform.localPosition.y) > 15 || Mathf.Abs(_transform.localPosition.x) > 15)
        {
            AddReward(-5.0f);
            EndEpisode();
        }
    }
}
