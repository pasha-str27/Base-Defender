using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrossHairController : MonoBehaviour
{
    protected CrossHairActions playerInput;

    [SerializeField] float maxMoveZoneScale = 2.5f;
    [SerializeField] float minMoveZoneScale = 0.5f;

    [SerializeField] float rechargeTime = 1.5f;
    [SerializeField] float rechargeSpeed = 1f;
    [SerializeField] float movementSpeed = 1;

    [SerializeField] Slider rechargeIndicator;
    [SerializeField] Rigidbody2D agentRB;
    [SerializeField] Transform crosshairWallls;

    [SerializeField] GameObject explosion;
    [SerializeField] Transform moveZone;

    [SerializeField] Transform explosionZone;

    bool canFire = true;
    float stepCoeficient = -1;
    bool canCheckVelocity = true;
    bool changedOffset = false;

    Rigidbody2D rb;

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

        if (canFire && playerInput.Player.Fire.triggered)
            Fire();

        if (dir != Vector2.zero) 
        {
            rb.simulated = true;
            agentRB.simulated = false;      

            if(!changedOffset)
                GetComponent<BoxCollider2D>().offset = agentRB.transform.localPosition;

            changedOffset = true;
            rb.velocity = dir * movementSpeed;
            crosshairWallls.position = transform.position;

            return;
        }

        changedOffset = false;

        rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, Time.deltaTime * 3);

        if (canCheckVelocity && rb.velocity.magnitude == 0)
        {
            rb.simulated = false;
            agentRB.simulated = true;
        }
    }

    void Fire()
    {
        agentRB.simulated = false;
        rb.simulated = true;

        GetComponent<BoxCollider2D>().offset = agentRB.transform.localPosition;
        moveZone.localScale = Vector3.one * (maxMoveZoneScale - Time.deltaTime);

        var explosionPosition = transform.GetChild(0).position;

        explosion.transform.position = explosionPosition;
        explosion.SetActive(true);

        explosionZone.position = new Vector3(explosionPosition.x, 0, explosionPosition.y);
        explosionZone.gameObject.SetActive(true);

        Invoke("DeactivateExplosionZone", 0.1f);

        rechargeIndicator.value = 0;
        canFire = false;

        var forceDirection = Random.insideUnitCircle.normalized;

        rb.AddForce(forceDirection * Random.Range(200, 650));

        canCheckVelocity = false;

        Invoke("LetCheckVelocity", 0.5f);

        StartCoroutine(Recharge());
    }

    void DeactivateExplosionZone() => explosionZone.gameObject.SetActive(false);

    void LetCheckVelocity() => canCheckVelocity = true;

    IEnumerator Recharge()
    {
        float time = 0;

        while (time < rechargeTime) 
        {
            time += Time.deltaTime * rechargeSpeed;
            rechargeIndicator.value = time / rechargeTime;

            yield return new WaitForSeconds(Time.deltaTime * rechargeSpeed);
        }

        canFire = true;
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    void Start()
    {
        playerInput = new CrossHairActions();
        rb = gameObject.GetComponent<Rigidbody2D>();

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
}
