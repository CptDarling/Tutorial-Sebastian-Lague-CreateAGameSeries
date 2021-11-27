using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] LayerMask collisionMask;
    [SerializeField] float lifeTime = 3f;

    float speed = 10f;
    float damage = 1f;
    float skinWidth = 0.1f; // Used to compensate for movement of collider plus movement of projectile.

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0) OnHitOject(initialCollisions[0]);
    }

    internal void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    private void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitOject(hit);
        }
    }

    void OnHitOject(RaycastHit hit)
    {
        IDamageable damageable = hit.collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject);
    }

    void OnHitOject(Collider c)
    {
        IDamageable damageable = c.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        GameObject.Destroy(gameObject);
    }

}
