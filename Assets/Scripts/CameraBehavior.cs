using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBehavior : MonoBehaviour
{

    [Header("Camera Orientation")]
    [SerializeField] private float m_angularSpeedX = 10;
    [SerializeField] private float m_minAngleX = -90;
    [SerializeField] private float m_maxAngleX = 90;
    private Vector2 m_mouseDelta;

    [Header("Aim")]
    [SerializeField] private float m_maxDistanceAimPoint;
    private Vector3 m_currentAimPoint = new Vector3();


    #region Unity Function

    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateCameraMouvement();
        UpdateAimPoint();
    }

    #endregion

    #region Input Functions

    public void OnMouseDelta(InputAction.CallbackContext ctx)
    {
        m_mouseDelta = ctx.ReadValue<Vector2>();
    }

    #endregion


    private void UpdateAimPoint()
    {
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(transform.position, transform.forward,out hit, m_maxDistanceAimPoint))
        {
            m_currentAimPoint = hit.point;
        }
        else
        {
            m_currentAimPoint = transform.position + transform.forward * m_maxDistanceAimPoint;
        }
    }

    public Vector3 GetCurrentAimPoint() { return m_currentAimPoint; }
    public float GetMaxAimDistance() { return m_maxDistanceAimPoint; }

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
