using UnityEngine;
using UnityEngine.InputSystem;

public class Character_Thrower : MonoBehaviour, IInteractibleObject
{
    [SerializeField] private GameObject m_ballInHand;
    [SerializeField] private Transform m_ballHandPoint;
    private bool m_isBallHold = false;

    [SerializeField] private float m_throwStrength = 0;

    #region Unity Engine
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Input Functions
    public void OnThrow(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            ThrowBall();
        }
    }
    #endregion

    public void ThrowBall()
    {
        if (!m_isBallHold || m_ballInHand == null) return;

        BallBehavior ballBehavior = m_ballInHand.GetComponent<BallBehavior>();
        ballBehavior.ThrowBall(transform.forward, m_throwStrength);
        ballBehavior.transform.SetParent(null);
        m_isBallHold = false;
    }


    public void OnTriggerEnter(Collider other)
    {
        IInteractibleObject interactible = other.GetComponent<IInteractibleObject>();
        if (interactible != null)
        {
            CollisionInteraction(interactible);
        }
    }

    public void CollisionInteraction(IInteractibleObject Obj)
    {
        if (m_ballInHand.tag == "Ball")
        {
            m_ballInHand = Obj.GetGameObject();
            BallBehavior ballBehavior = m_ballInHand.GetComponent<BallBehavior>();
            ballBehavior.ChangeState(BallState.HOLD);
            m_isBallHold = true;
            ballBehavior.transform.SetParent(m_ballHandPoint);
            ballBehavior.transform.position = m_ballHandPoint.position;
        }
    }
    public GameObject GetGameObject() { return gameObject; }



}
