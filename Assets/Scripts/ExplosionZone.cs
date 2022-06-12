using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionZone : MonoBehaviour
{
    [SerializeField] float dropForce = 2;
    [SerializeField] GameObject[] explosionCraters;

    private void OnEnable()
    {
        var spawnPosition = new Vector3(transform.position.x, -0.59f, transform.position.z);

        Instantiate(explosionCraters[Random.Range(0, explosionCraters.Length)], spawnPosition, Quaternion.Euler(90, 0, 0));
    }

    private void OnCollisionEnter(Collision collision) => DropGameObject(collision.gameObject.GetComponent<Rigidbody>());

    private void OnTriggerEnter(Collider collision) => DropGameObject(collision.gameObject.GetComponent<Rigidbody>());

    void DropGameObject(Rigidbody rb)
    {
        var forceDir = Random.onUnitSphere;
        forceDir.y = Mathf.Abs(forceDir.y);

        rb.AddForce(forceDir * dropForce, ForceMode.Impulse);
    }
}
