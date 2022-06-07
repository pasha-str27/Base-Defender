using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrossHairController : MonoBehaviour
{
    protected CrossHairActions playerInput;

    Transform _transform;

    [SerializeField] float maxMoveZoneScale = 2.5f;
    [SerializeField] float minMoveZoneScale = 0.5f;

    [SerializeField] GameObject explosion;
    [SerializeField] Transform moveZone;

    float stepCoeficient = -1;

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

        if (dir != Vector2.zero)
        {
            if (moveZone.localScale.x < maxMoveZoneScale - Time.deltaTime * 2)
                moveZone.localScale += Vector3.one * Time.deltaTime * 2;
        }
        else
            stepCoeficient = -0.15f;

        bool firePressed = playerInput.Player.Fire.triggered;
        if (firePressed)
        {
            moveZone.localScale = Vector3.one * (maxMoveZoneScale - Time.deltaTime);

            explosion.transform.position = transform.GetChild(0).position;
            explosion.SetActive(true);

            //Ô²ÇÈÊÀ
            //transform.position += (Vector3)Random.insideUnitCircle * 3;

            print("Fire");
        }

        _transform.Translate(dir * Time.deltaTime);
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

        StartCoroutine(MoveZoneSizeChanger());
    }

    IEnumerator MoveZoneSizeChanger()
    {
        while (true)
        {
            yield return null;

            if (moveZone.localScale.x > minMoveZoneScale && moveZone.localScale.x < maxMoveZoneScale)
                moveZone.localScale += Vector3.one * Time.deltaTime * stepCoeficient;
        }
    }

    //IEnumerator MoveZoneSizeChanger()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1);

    //        moveZone.localScale += Vector3.one * Time.deltaTime * stepCoeficient;

    //        if (Mathf.Abs(moveZone.localScale.x - minMoveZoneScale) < 0.3f)
    //            stepCoeficient = 1;

    //        if (Mathf.Abs(moveZone.localScale.x - maxMoveZoneScale) < 0.3f)
    //            stepCoeficient = -1;
    //    }
    //}
}
