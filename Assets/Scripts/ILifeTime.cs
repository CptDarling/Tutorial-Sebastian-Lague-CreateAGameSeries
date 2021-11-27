using System.Collections;
using UnityEngine;

public interface ILifeTime
{
    public event System.Action OnBirth;
    public event System.Action OnDeath;

    void Die();

}