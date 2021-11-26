using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] float lifeTime = 3f;

    float speed = 10f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    internal void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

}
