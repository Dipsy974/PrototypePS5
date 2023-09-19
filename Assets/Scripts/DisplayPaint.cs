using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DisplayPaint : MonoBehaviour
{
    [SerializeField] GameObject decalProjectorPrefab;
    [SerializeField] GameObject effectCollider;
    List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>(); 
    }
    private void OnParticleCollision(GameObject other)
    {

        ParticlePhysicsExtensions.GetCollisionEvents(other.GetComponent<ParticleSystem>(), gameObject, collisionEvents);
        PaintOn(collisionEvents[0]);
    }
   

    private void PaintOn(ParticleCollisionEvent particleCollisionEvent)
    {
        Debug.Log(particleCollisionEvent.intersection); 
        var decal = Instantiate(decalProjectorPrefab, particleCollisionEvent.intersection, Quaternion.LookRotation(-particleCollisionEvent.normal)); 
        var effect = Instantiate(effectCollider, particleCollisionEvent.intersection, Quaternion.LookRotation(Vector3.zero));
        effect.transform.rotation = Quaternion.FromToRotation(effect.transform.up, particleCollisionEvent.normal); 
    }

}
