using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Input")]
    public bool requireMouseButton = true;     // true = click-and-drag, false = free look
    public int mouseButton = 0;                // 0 = left, 1 = right
    public float sensitivity = 3f;
    public bool invertY = false;

    [Header("Limits")]
    public float minPitch = -89f;
    public float maxPitch = 89f;

    [Header("Utility")]
    public KeyCode resetKey = KeyCode.R;

    private float yaw;
    private float pitch;

    void Start()
    {
        Vector3 e = transform.localEulerAngles;
        yaw = e.y;
        pitch = e.x > 180f ? e.x - 360f : e.x;
    }

    void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            yaw = 0f;
            pitch = 0f;
        }

        if ((!requireMouseButton || Input.GetMouseButton(mouseButton)) && !OKSField.PointerOverPanel)
        {
            float dx = Input.GetAxis("Mouse X") * sensitivity;
            float dy = Input.GetAxis("Mouse Y") * sensitivity;

            yaw += dx;
            pitch += invertY ? dy : -dy;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // Yaw and pitch only; head roll stays at zero so it doesn't confound the OKS roll
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}