using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiveBarrel : InteractionObject
{
    [Header("Destrutable Barrel")]
    [SerializeField]
    private GameObject destrutableBarrelPieces;

    private bool isDestroyed = false;

    public override void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0 && isDestroyed == false)
        {
            isDestroyed = true;
            Instantiate(destrutableBarrelPieces, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
