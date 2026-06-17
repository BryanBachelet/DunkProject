using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum PlayerState
{
    NONE = 0,
    IDLE = 1,
    MOVE = 2,
    JUMP = 3,
    FALL = 4,
}


public class PlayerMovement : MonoBehaviour
{
    private Rigidbody m_rigidbody;

    [Header("Player Movement")]
    [SerializeField] private float m_mvtSpeed = 10;

    [Header("Player Rotation")]
    [SerializeField] private float m_angularSpeed = 10;

    [Header("Jump")]
    [SerializeField] private float m_jumpStrength = 10;

    private Vector2 m_inputDirection = Vector2.zero;
    private Vector2 m_inputMouseDelta = Vector2.zero;
    private bool m_onGround;
    private bool m_isInputJumpPerfom;

    private PlayerState m_currentPlayerState;
    public UnityEvent<PlayerState> OnPlayerStateChange;

    [Header("Debug Parameters")]
    [SerializeField] private bool m_activeDebugState;


    #region Unity Functions

    private void Start()
    {
        InitComponent();
    }

    private void Update()
    {
        OnGroundDetection();
    }

    public void FixedUpdate()
    {
        UpdateMovement();
    }
    #endregion

    #region Inputs Functions
    public void OnMovement(InputAction.CallbackContext ctx)
    {
        m_inputDirection = ctx.ReadValue<Vector2>();
    }

    public void OnJumpMovement(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            AddJump();
            ChangeState(PlayerState.JUMP);
            m_isInputJumpPerfom = true;
        }

        if (ctx.canceled)
        {
            m_isInputJumpPerfom = false;
        }
    }

    public void MouseDelta(InputAction.CallbackContext ctx)
    {
        m_inputMouseDelta = ctx.ReadValue<Vector2>();
    }
    #endregion

    private void InitComponent()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_rigidbody = GetComponent<Rigidbody>();
    }
    private void OnGroundDetection()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(m_rigidbody.position, Vector3.down, out hit, 2.5f))
        {
            Debug.Log("Hit Ray name " + hit.collider.name);
            m_onGround = hit.collider.tag == "Ground";
        }
        else
        {
            m_onGround = false;
        }
    }
    private void UpdateMovement()
    {
        if (m_onGround && m_inputDirection != Vector2.zero)
        {
            ChangeState(PlayerState.MOVE);
        }
        if (m_onGround && m_inputDirection == Vector2.zero)
        {
            ChangeState(PlayerState.IDLE);
        }

        if (!m_onGround && m_rigidbody.linearVelocity.y < 0)
        {
            ChangeState(PlayerState.FALL);
        }

        // Movement Player
        Vector3 nextMvtDirection = (transform.forward * m_inputDirection.y + transform.right * m_inputDirection.x) * m_mvtSpeed;
        Vector3 nextPosition = m_rigidbody.position + nextMvtDirection;

        // Rotation Player
        Quaternion nextRotation = transform.rotation;
        nextRotation *= Quaternion.Euler(0, m_inputMouseDelta.x * Time.deltaTime * m_angularSpeed, 0);

        transform.rotation = (nextRotation);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(m_rigidbody.position + Vector3.down *.9f, nextMvtDirection.normalized, out hit, 1.0f) )
        {
            if (hit.collider.tag == "Ground")
            {
                nextMvtDirection = Vector3.zero;
            }

        }

        m_rigidbody.AddForce(nextMvtDirection, ForceMode.Impulse);
        Vector3 vel = new Vector3(m_rigidbody.linearVelocity.x, 0, m_rigidbody.linearVelocity.z);
        vel = Vector3.ClampMagnitude(vel, m_mvtSpeed) * Time.fixedDeltaTime;


        float addGravity = 0.0f;
        if (m_rigidbody.linearVelocity.y < 0 || !m_isInputJumpPerfom && !m_onGround)
        {
            addGravity += Physics.gravity.y * Time.fixedDeltaTime;
        }

        m_rigidbody.linearVelocity = new Vector3(vel.x, m_rigidbody.linearVelocity.y + addGravity, vel.z);

    }
    private void AddJump()
    {
        if (!m_onGround) return;

        m_rigidbody.AddForce(Vector3.up * m_jumpStrength, ForceMode.Impulse);
    }

    #region State Inputs

    public void ChangeState(PlayerState nextState)
    {
        if (nextState == m_currentPlayerState) return;

        m_currentPlayerState = nextState;
        OnPlayerStateChange?.Invoke(nextState);

        if (m_activeDebugState)
            Debug.Log($"New Player State : {nextState}");

        switch (nextState)
        {
            case PlayerState.NONE:
                break;
            case PlayerState.IDLE:
                break;
            case PlayerState.MOVE:
                break;
            case PlayerState.JUMP:
                break;
            case PlayerState.FALL:
                break;
            default:
                break;
        }
    }

    #endregion

}
