using UnityEngine;

public class CrossHairController : MonoBehaviour
{
    protected CrossHairActions playerInput;

    Transform _transform;


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

    private void OnDisable()
    {
        playerInput.Disable();
    }

    void Start()
    {
        _transform = transform;
        playerInput = new CrossHairActions();

        playerInput.Enable();
    }
}
