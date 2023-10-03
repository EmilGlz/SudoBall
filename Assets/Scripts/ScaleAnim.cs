using UnityEngine;

public class ScaleAnim : MonoBehaviour
{
    [SerializeField] private RectTransform animObject;
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;
    [SerializeField] private float inScaleSpeed;
    [SerializeField] private float outScaleSpeed;

    private bool grow = true;

    private void FixedUpdate()
    {
        if (grow)
        {
            animObject.localScale = new Vector3(animObject.localScale.x + outScaleSpeed, animObject.localScale.y + outScaleSpeed, 1);
            if (animObject.localScale.magnitude >= maxScale.magnitude) grow = false;
        }
        else
        {
            animObject.localScale = new Vector3(animObject.localScale.x - inScaleSpeed, animObject.localScale.y - inScaleSpeed, 1);
            if (animObject.localScale.magnitude <= minScale.magnitude) grow = true;
        }
    }
}
