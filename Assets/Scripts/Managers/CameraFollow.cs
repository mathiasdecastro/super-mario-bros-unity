using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float yOffset = 2f;
    [SerializeField] float leftLimit = 0f;
    [SerializeField] float rightLimit = 200f;
    [SerializeField] bool neverGoBack = true;

    float maxX;

    void Start()
    {
        maxX = transform.position.x;   
    }

    
    void LateUpdate()
    {
        if (target == null) return;

        float x = target.position.x;
        if (neverGoBack) x = Mathf.Max(x, maxX);
        x = Mathf.Clamp(x, leftLimit, rightLimit);
        maxX = x;

        transform.position = new Vector3(x, target.position.y + yOffset, -10f);
    }
}
