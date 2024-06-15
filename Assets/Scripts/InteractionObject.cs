using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    [Header("Interaction Object")]
    [SerializeField]
    protected int maxHP = 100;
    protected int currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }
    // Start is called before the first frame update
    public abstract void TakeDamage(int damage);
}
