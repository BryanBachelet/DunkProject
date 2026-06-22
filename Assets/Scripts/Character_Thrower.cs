using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Character_Thrower : MonoBehaviour, IInteractibleObject
{
    [SerializeField] private GameObject m_ballInHand;
    [SerializeField] private Transform m_ballHandPoint;
    private BallBehavior m_ballBehavior;
    private bool m_isBallHold = false;

    [SerializeField] private float m_throwStrength = 0;
    [SerializeField] private CameraBehavior m_camBehavior = null;

    private Rigidbody m_rbComp;
    private PlayerMovement m_playerMvt;

    public UnityEvent onBallCatch;

    [Header("Debugs")]
    [SerializeField] private bool m_activeDebugAimRebound;
    [SerializeField] private int m_maxDebugReboundProjection;

    #region Unity Engine
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_camBehavior = GetComponentInChildren<CameraBehavior>();
        m_rbComp = GetComponent<Rigidbody>();
        m_playerMvt = GetComponent<PlayerMovement>();
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

       if(m_ballBehavior == null) m_ballBehavior = m_ballInHand.GetComponent<BallBehavior>();
        Vector3 throwDir = m_camBehavior.GetCurrentAimPoint() - m_ballHandPoint.transform.position;

        m_ballBehavior.ThrowBall(throwDir.normalized, m_throwStrength);
        m_ballBehavior.transform.SetParent(null);
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
        if (Obj.GetGameObject().tag == "Ball" && !m_isBallHold)
        {
            m_ballInHand = Obj.GetGameObject();
           if(m_ballBehavior ==null) m_ballBehavior = m_ballInHand.GetComponent<BallBehavior>();

            m_playerMvt.AddImpulsion(m_ballBehavior.GetCurrentDirection().normalized ,m_ballBehavior.GetCurrentSpeed());
            m_ballBehavior.ChangeState(BallState.HOLD);
            m_isBallHold = true;
            m_ballBehavior.transform.SetParent(m_ballHandPoint);
            m_ballBehavior.transform.position = m_ballHandPoint.position;
            onBallCatch?.Invoke();
        }
    }
    public GameObject GetGameObject() { return gameObject; }

    #region Debug Functions

    public void OnDrawGizmos()
    {

        if (m_camBehavior == null || m_ballBehavior == null || m_ballHandPoint == null || !m_activeDebugAimRebound) return;

        Gizmos.color = Color.red;
        Vector3 throwDir = m_camBehavior.GetCurrentAimPoint() - m_ballHandPoint.transform.position;
        Vector3 startPoint = m_ballHandPoint.transform.position;
        List<Vector3> pointList = new List<Vector3>();
        pointList.Add(m_ballHandPoint.transform.position);
        pointList.Add(m_camBehavior.GetCurrentAimPoint());
        for (int i = 0; i < m_maxDebugReboundProjection; i++)
        {

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(startPoint, throwDir.normalized, out hit, m_camBehavior.GetMaxAimDistance()))
            {
                startPoint = hit.point;
                pointList.Add(hit.point);
                throwDir = Vector3.Reflect(throwDir.normalized, hit.normal);

                // Aim assist
                //Vector3 dirPlayer = transform.position- hit.point;
                //float dot = Vector3.Dot(throwDir.normalized, dirPlayer.normalized);
                //if (dot > m_ballBehavior.cosValueLimitReboundAsssis)
                //{
                //    throwDir = dirPlayer.normalized;
                //}

                IInteractibleObject interactiveObj = hit.collider.GetComponent<IInteractibleObject>();
                if (interactiveObj != null)
                {
                    if (interactiveObj.GetGameObject().tag == "Player")
                    {
                        Gizmos.color = Color.green;
                        break;
                    }
                }
            }
            else
            {
                pointList.Add(startPoint + throwDir.normalized * m_camBehavior.GetMaxAimDistance());
                break;
            }

        }

        for (int i = 0; i < pointList.Count - 1; i++)
        {
            Gizmos.DrawSphere(pointList[i], 0.1f);
            Gizmos.DrawLine(pointList[i], pointList[i + 1]);
        }



    }

    #endregion



}
