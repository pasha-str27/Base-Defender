using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] bool needToDestroy = true;

    public void Hide()
    {
        if(needToDestroy)
        {
            Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false);
    }
}
