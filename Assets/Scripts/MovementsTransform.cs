using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementsTransform : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.0f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
    public void MoveTo(Vector3 diretion)
    {
        moveDirection = diretion;
    }
}
