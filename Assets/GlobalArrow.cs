using UnityEngine;
using UnityEngine.UI;

public class GlobalArrow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Sprite arrowSprite;      // Sprite used when target is off-screen
    [SerializeField] private Sprite locationSprite;   // Sprite used when target is on-screen
    [SerializeField] private GameObject arrowObject;  // UI arrow object
    [SerializeField] private Camera cam;              // Main camera reference

    private Transform targetPosition;
    private RectTransform arrowRect;
    private Image arrowImage;

    private float border = 50f;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        arrowRect = arrowObject.GetComponent<RectTransform>();
        arrowImage = arrowObject.GetComponent<Image>();
    }

    private void Update()
    {
        if (targetPosition == null) return;

        // Convert world position to screen space
        Vector3 screenPoint = cam.WorldToScreenPoint(targetPosition.position);

        // Check if target is behind the camera
        bool isBehind = screenPoint.z < 0;
        if (isBehind)
        {
            screenPoint.x = Screen.width - screenPoint.x;
            screenPoint.y = Screen.height - screenPoint.y;
        }

        // Determine if on-screen
        bool isOnScreen =
            screenPoint.z > 0 &&
            screenPoint.x > border &&
            screenPoint.x < Screen.width - border &&
            screenPoint.y > border &&
            screenPoint.y < Screen.height - border;

        if (!isOnScreen)
        {
            // ---- OFF-SCREEN BEHAVIOR ----
            arrowImage.sprite = arrowSprite;
            MoveToEdge(screenPoint);
            RotateArrow(screenPoint);
        }
        else
        {
            // ---- ON-SCREEN BEHAVIOR ----
            arrowImage.sprite = locationSprite;
            GoToTopOfTarget(screenPoint);
        }
    }

    private void MoveToEdge(Vector3 screenPoint)
    {
        // Clamp the arrow position to the screen edges with border padding
        float clampedX = Mathf.Clamp(screenPoint.x, border, Screen.width - border);
        float clampedY = Mathf.Clamp(screenPoint.y, border, Screen.height - border);
        arrowRect.position = new Vector3(clampedX, clampedY, 0f);
    }

    private void RotateArrow(Vector3 screenPoint)
    {
        // Find the center of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Direction from screen center to target position
        Vector3 dir = (screenPoint - screenCenter).normalized;

        // Calculate angle in degrees
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Apply rotation (sprite should face up ↑ by default)
        arrowRect.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void GoToTopOfTarget(Vector3 screenPoint)
    {
        // Position the arrow slightly above the target (in screen space)
        arrowRect.position = screenPoint + Vector3.up * 80f;
        arrowRect.rotation = Quaternion.identity;
    }

    public void Hide()
    {
        arrowObject.SetActive(false);
    }

    public void Show()
    {
        arrowObject.SetActive(true);
    }

    public void SetTarget(Transform position)
    {
        targetPosition = position;
        Show();
    }
}
