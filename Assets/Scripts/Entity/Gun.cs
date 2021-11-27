using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    [SerializeField] Transform muzzle;
    [SerializeField] Projectile projectile;
    [SerializeField] float msBetweenShots = 100f;
    [SerializeField] float muzzleVelocity = 35f;

    float nextShotTime;

    private void Start()
    {
        nextShotTime = Time.time;
    }

    internal void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile bullet = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            bullet.SetSpeed(muzzleVelocity);
        }
    }
}
