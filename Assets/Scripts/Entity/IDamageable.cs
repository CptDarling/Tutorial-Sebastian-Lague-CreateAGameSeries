using UnityEngine;

public interface IDamageable
{

    void TakeDamage(float damage);
    void TakeHit(float damage, RaycastHit hit);

}
