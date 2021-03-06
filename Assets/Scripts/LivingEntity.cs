using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable, ILifeTime
{

    [SerializeField] float startingHealth;
    [SerializeField] float lifeTime;

    public event Action OnBirth;
    public event Action OnDeath;

    // Protected so that classes that derive from this class 
    // can get to health but other classes and the inspector can't.
    protected float health;
    protected bool dead;

    float birthTime;

    protected virtual void Start()
    {
        health = startingHealth;
        dead = false;
        birthTime = Time.time;
        OnBirth?.Invoke();
    }

    protected virtual void Update()
    {
        if (!dead && lifeTime > 0 && (Time.time >= birthTime + lifeTime))
        {
            Die();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    public void Die()
    {
        dead = true;
        OnDeath?.Invoke();
        GameObject.Destroy(gameObject);
    }
}
