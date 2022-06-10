using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionZone : MonoBehaviour
{
    [SerializeField] float dropForce = 2;

    private void OnCollisionEnter(Collision collision) => DropGameObject(collision.gameObject.GetComponent<Rigidbody>());

    private void OnTriggerEnter(Collider collision) => DropGameObject(collision.gameObject.GetComponent<Rigidbody>());

    void DropGameObject(Rigidbody rb)
    {
        var forceDir = Random.onUnitSphere;
        forceDir.y = Mathf.Abs(forceDir.y);

        rb.AddForce(forceDir * dropForce, ForceMode.Impulse);
    }
}
