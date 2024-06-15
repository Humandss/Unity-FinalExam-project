using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMedicBag : ItemBase
{
    [SerializeField]
    private GameObject hpEffectPrefab;
    [SerializeField]
    private int increaseHP = 30;
    [SerializeField]
    private float moveDistance = 0.2f;
    [SerializeField]
    private float pingpongSpeed = 0.5f;
    [SerializeField]
    private float rotaeSpeed = 50;

    private IEnumerator Start()
    {
        float y = transform.position.y;

        while(true)
        {
            transform.Rotate(Vector3.up * rotaeSpeed * Time.deltaTime);

            Vector3 position = transform.position;
            position.y = Mathf.Lerp(y, y+moveDistance, Mathf.PingPong(Time.time*pingpongSpeed,1));
            transform.position = position;

            yield return null;
        }
    }
    public override void Use(GameObject entity)
    {
        entity.GetComponent<MovementStatus>().IncreaseHP(increaseHP);

        Instantiate(hpEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
