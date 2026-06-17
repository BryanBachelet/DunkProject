using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBehavior : MonoBehaviour
{

    [Header("Camera Orientation")]
    [SerializeField] private float m_angularSpeedX = 10;
    [SerializeField] private float m_minAngleX = -90;
    [SerializeField] private float m_maxAngleX = 90;
    private Vector2 m_mouseDelta;
    #region Unity Function

    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateCameraMouvement();
    }

    #endregion

    #region Input Functions

    public void OnMouseDelta(InputAction.CallbackContext ctx)
    {
        m_mouseDelta = ctx.ReadValue<Vector2>();
    }

    #endregion

    public void UpdateCameraMouvement()
    {
        Quaternion nextRotation = transform.rotation;
        float camAngleX = transform.rotation.eulerAngles.x + -m_mouseDelta.y * m_angularSpeedX * Time.deltaTime;
        camAngleX = camAngleX > 180 ? camAngleX - 360 : camAngleX;
        camAngleX = Mathf.Clamp(camAngleX, m_minAngleX, m_maxAngleX);
        nextRotation = Quaternion.Euler(camAngleX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        transform.rotation = nextRotation;

    }

}
