using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3 offset = new Vector3(0, -0.2f, 1f);

    void LateUpdate()
    {
        if (cameraTransform == null) return;
        transform.position = cameraTransform.position + 
            cameraTransform.TransformDirection(offset);
        transform.rotation = cameraTransform.rotation;
    }
}