using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaomondRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 5;
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, rotateSpeed);
    }
}
