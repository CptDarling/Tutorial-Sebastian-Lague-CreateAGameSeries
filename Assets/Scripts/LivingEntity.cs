using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{

    [SerializeField] float startingHealth;

    // Protected so that classes that derive from this class 
    // can get to health but other classes and the inspector can't.
    protected float health;
    protected bool dead;

    protected virtual void Start()
    {
        health = startingHealth;
        dead = false;
    }


    public void TakeHit(float damage, RaycastHit hit)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    private void Die()
    {
        dead = true;
        GameObject.Destroy(gameObject);
    }
}
