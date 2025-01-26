using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossCollider : MonoBehaviour
{
    public event Action<Collision> onCollisionEnter, onCollisionStay, onCollisionExit;
    public event Action<Collider> onTriggerEnter, onTriggerStay, onTriggerExit;

    private void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter?.Invoke(collision);
    }

    private void OnCollisionStay(Collision collision) 
    {
        onCollisionStay?.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        onCollisionExit?.Invoke(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        onTriggerStay?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }
}
