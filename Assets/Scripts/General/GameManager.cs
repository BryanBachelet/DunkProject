using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;

    public InputActionAsset m_inputActionAsset;

    private InputAction m_ballSpawmInput;
    private InputAction m_spawnPlayerInput;

    [Header("Reference")]
    [SerializeField] private GameObject m_playerPrefab;
    [SerializeField] private GameObject m_ballPrefab;

    [Header("Scene Object")]
    [SerializeField] private Transform m_playerSpawn;
    [SerializeField] private Transform m_ballSpawn;

    [HideInInspector] public UnityEvent<GameObject> OnPlayerSpawn;
    [HideInInspector] public UnityEvent<GameObject> OnBallSpawn;

    private GameObject m_currentPlayer;
    private GameObject m_currentBall;


    #region Unity Functions

    public void Awake()
    {
        if (m_instance == null)
            m_instance = this;

#if UNITY_EDITOR
        InputActionMap map = m_inputActionAsset.FindActionMap("Debug");
        map.Enable();
        m_ballSpawmInput = map.FindAction("SpawnBall");
        m_spawnPlayerInput = map.FindAction("SpawnPlayer");  
#endif
    }

    public void Start()
    {
        LaunchScene();
    }

    public void Update()
    {
        if (m_ballSpawmInput.WasPressedThisFrame())
        {
            SpawnBall();
        }


        if(m_spawnPlayerInput.WasPressedThisFrame())
        {
            SpawnPlayer();
        }

    }

    #endregion

    public void LaunchScene()
    {
        SpawnPlayer();
        SpawnBall();
    }

    public void SpawnPlayer()
    {
        if (m_currentPlayer)
        {
            Destroy(m_currentPlayer);
        }
        m_currentPlayer = GameObject.Instantiate(m_playerPrefab, m_playerSpawn.position, m_playerSpawn.rotation);
        OnPlayerSpawn?.Invoke(m_currentPlayer);
    }

    public void SpawnBall()
    {
        if(m_currentBall)
        {
            Destroy(m_currentBall);
        }

        m_currentBall = GameObject.Instantiate(m_ballPrefab, m_ballSpawn.position, m_ballSpawn.rotation);
        OnBallSpawn?.Invoke(m_currentBall);
    }

    #region Static Functions

    public static GameManager GetGameManager()
    {
        if (m_instance == null)
            return null;

        return m_instance;
    }
    public static GameObject GetPlayer()
    {
        if (m_instance == null)
            return null;

        return m_instance.m_currentPlayer;
    }

    public static GameObject GetBall()
    {
        if (m_instance == null)
            return null;

        return m_instance.m_currentBall;
    }

    #endregion
}
