using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform targetPosition;
    private Vector3 initialPosition;
    private float initialCameraSize;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (targetPosition == null)
        {
            targetPosition = transform;
        }

        initialPosition = targetPosition.position;
        initialCameraSize = cam.orthographicSize;
    }

    public void GoToTargetPosition(Transform target)
    {
        cam.transform.position = new Vector3(target.position.x, target.position.y, cam.transform.position.z);
        targetPosition = target;
    }

    public void SetCameraSize(float size)
    {
        if (size >= 2 && size <= 6)
        {
            cam.orthographicSize = size;
        }
    }

    public void AddCameraSize(float size)
    {
        size = Mathf.Abs(size);
        SetCameraSize(cam.orthographicSize + size);
    }

    public void DecreaseCameraSize(float size)
    {
        size = Mathf.Abs(size);
        SetCameraSize(cam.orthographicSize - size);
    }

    public void SetTargetPosition(Transform newTargetPosition)
    {
        targetPosition = newTargetPosition;
    }

    public void ResetCamera()
    {
        cam.orthographicSize = initialCameraSize;
        cam.transform.position = new Vector3(initialPosition.x, initialPosition.y, cam.transform.position.z);
    }

    void LateUpdate()
    {
        Vector3 camPosition = cam.transform.position;
        Vector3 targetPos = new Vector3(targetPosition.position.x, targetPosition.position.y, camPosition.z);
        cam.transform.position = Vector3.Lerp(camPosition, targetPos, Time.deltaTime * 5f);
    }
}
