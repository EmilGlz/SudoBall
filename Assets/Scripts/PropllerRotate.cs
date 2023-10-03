using UnityEngine;

public class PropllerRotate : MonoBehaviour
{
    [SerializeField] GameObject[] propellers;
    private void FixedUpdate()
    {
        for(int i = 0; i<propellers.Length; i++)
        {
            propellers[i].transform.Rotate(Vector3.up, 50);
        }
    }
}
