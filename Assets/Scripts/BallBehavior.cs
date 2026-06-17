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

    private BallProperty m_ballProperty;
    private float m_currentSpeed;
    private Vector3 m_currentDirection;
    private BallState m_ballState = BallState.NONE;

    #region Unity Function
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (m_ballState != BallState.MOVE) return;
        CheckCollisionBall();
        UpdateBallMovement();
    }
    #endregion;

    public void UpdateBallMovement()
    {
        m_currentSpeed = Mathf.Clamp(m_currentSpeed, m_minSpeed, m_maxSpeed);
        transform.position += m_currentDirection * m_currentSpeed * Time.deltaTime;
    }

    public void CheckCollisionBall()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, m_currentDirection, out hit, m_currentSpeed * 2.0f * Time.deltaTime)
        {
            if (hit.collider.tag != "Ball")
            {
                IInteractibleObject interactiveObj = hit.collider.GetComponent<IInteractibleObject>();
                interactiveObj.CollisionInteraction(interactiveObj);
                m_currentDirection = Vector3.Reflect(m_currentDirection, hit.normal);
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
        ChangeState(BallState.MOVE);
    }

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
