using UnityEngine;

public enum BallState
{
    NONE = 0,
    MOVE = 1,
    HOLD = 2
}

public struct BallProperty
{
    public float strengthGain;
}


public class BallBehavior : MonoBehaviour, IInteractibleObject
{
    [Header("Speed")]
    [SerializeField] private float m_minSpeed = 0;
    [SerializeField] private float m_maxSpeed = 0;


    [Header("Aim Assist Rebound")]
    public GameObject player;
    public bool isReboundAssistActive = false;
    public float cosValueLimitReboundAsssis = 0.45f;

    [Header("Aim Assist Trajectory")]
    [SerializeField] private float m_maxCosValue = .9f;
    [SerializeField] private float m_minCosValue = .9f;
    [SerializeField] private float m_magneticMultiplier = 1.0f;



    private BallProperty m_ballProperty;
    private float m_currentSpeed;
    private Vector3 m_currentDirection;
    private BallState m_ballState = BallState.NONE;
    private Rigidbody m_rbComp = null;

    private int m_reboundCount = 0;

    #region Unity Function
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_rbComp = GetComponent<Rigidbody>();
        player = GameManager.GetPlayer();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_ballState != BallState.MOVE) return;
        CheckCollisionBall();
        UpdateBallMovement();
    }
    #endregion;


    public void UpdateBallMovement()
    {
        Vector3 dirPlayer = player.transform.position - transform.position;
        float dot = Vector3.Dot(m_currentDirection.normalized, dirPlayer.normalized);
        float minDotValue = Mathf.Lerp(m_minCosValue , m_maxCosValue, dirPlayer.magnitude / 10.0f);
        if (dot > minDotValue)
        {
            m_currentDirection = Vector3.Lerp(m_currentDirection, dirPlayer.normalized, dot * m_magneticMultiplier);
        }

        m_currentSpeed = Mathf.Clamp(m_currentSpeed, m_minSpeed, m_maxSpeed);
        transform.position += m_currentDirection * m_currentSpeed * Time.fixedDeltaTime;

    }

    public void CheckCollisionBall()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, m_currentDirection, out hit, m_currentSpeed * 5.0f * Time.fixedDeltaTime))
        {
            if (hit.collider.tag != "Ball" || (hit.collider.tag == "Player" && m_reboundCount != 0))
            {
                IInteractibleObject interactiveObj = hit.collider.GetComponent<IInteractibleObject>();
                m_reboundCount++;
                if (interactiveObj != null)
                    interactiveObj.CollisionInteraction(this);
                m_currentDirection = Vector3.Reflect(m_currentDirection, hit.normal);
                if (isReboundAssistActive)
                {
                    Vector3 dirPlayer = player.transform.position - transform.position;
                    float dot = Vector3.Dot(m_currentDirection.normalized, dirPlayer.normalized);
                    if (dot > cosValueLimitReboundAsssis)
                    {
                        m_currentDirection = dirPlayer.normalized;
                    }
                }
            }
        }
    }

    public void ChangeState(BallState state)
    {
        m_ballState = state;
        switch (state)
        {
            case BallState.NONE:
                break;
            case BallState.MOVE:
                break;
            case BallState.HOLD:
                m_currentDirection = Vector3.zero;
                m_currentSpeed = 0.0f;
                m_rbComp.linearVelocity = Vector3.zero;
                m_rbComp.isKinematic = true;
                m_reboundCount = 0;
                break;
            default:
                break;
        }
    }

    public void IncreaseSpeed(float speedAdd)
    {
        m_currentSpeed += speedAdd;
        m_ballProperty.strengthGain += speedAdd;
    }

    public BallProperty RetrieveBallProperty()
    {
        BallProperty ballPropertyCurrent = m_ballProperty;
        m_ballProperty = new BallProperty();
        return ballPropertyCurrent;
    }

    public void ThrowBall(Vector3 direction, float strength)
    {
        m_currentSpeed = strength;
        m_currentDirection = direction;
        player = GameManager.GetPlayer();
        ChangeState(BallState.MOVE);
    }


    public float GetCurrentSpeed() { return m_currentSpeed; }
    public Vector3 GetCurrentDirection() { return m_currentDirection; }

    #region IInteractive Functions
    public void CollisionInteraction(IInteractibleObject Obj)
    {
        //
    }

    public GameObject GetGameObject() { return gameObject; }
    #endregion

    #region Collision Functions
    public void OnCollisionEnter(Collision collision)
    {
        IInteractibleObject interactible = collision.gameObject.GetComponent<IInteractibleObject>();
        if (interactible != null)
        {
            interactible.CollisionInteraction(this);
        }
    }

    #endregion

}
