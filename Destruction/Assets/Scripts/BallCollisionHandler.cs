using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

public class BallCollisionHandler : MonoBehaviour
{
    public LayerMask shatterLayer;
    public Rigidbody thisRb;
    private SphereCollider trigger;

    private void Start()
    {
        thisRb= GetComponent<Rigidbody>();
        var sphereCol= GetComponents<SphereCollider>();
        trigger = sphereCol.FirstOrDefault(x => x.isTrigger);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (shatterLayer.value == other.gameObject.layer)
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            Rigidbody shatterRigidbody = other.GetComponent<Rigidbody>();
            if (shatterRigidbody != null)
            {
                shatterRigidbody.isKinematic = false;
                shatterRigidbody.velocity+= thisRb.velocity / (distance/2);
                if (shatterRigidbody.velocity.sqrMagnitude > thisRb.velocity.sqrMagnitude * 3 / 4)
                    shatterRigidbody.velocity = thisRb.velocity;
            }
        }
    }
}
