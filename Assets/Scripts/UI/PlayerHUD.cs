using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Image m_aimCursor;

    [Header("Animation Aim cursor")]
    private bool m_catchAnimActive;
    [SerializeField] private float m_durationAnim;
    [SerializeField] private float m_sizeIncrease = 1.2f;
    private float m_currentTime;
    private Vector3 m_newSize;
    private Vector3 m_oldSize;

    #region Unity Function
    void Start()
    {
        GameManager.GetGameManager().OnPlayerSpawn.AddListener(SubscribeToPlayer);
        GameObject player = GameManager.GetPlayer();
        if (player != null)
        {
            SubscribeToPlayer(player);
        }
    }
    private void Update()
    {
        if (m_catchAnimActive)
        {
            m_aimCursor.rectTransform.localScale = Vector3.Lerp(m_newSize, m_oldSize, m_currentTime / m_durationAnim);
            m_currentTime += Time.deltaTime;
            if (m_currentTime > m_durationAnim)
            {
                m_aimCursor.rectTransform.localScale = m_oldSize;
                m_catchAnimActive = false;
                m_currentTime = 0.0f;
            }
        }

    }
    #endregion

    private void SubscribeToPlayer(GameObject Player)
    {
        Character_Thrower charaThrower = Player.GetComponent<Character_Thrower>();
        charaThrower.onBallCatch.AddListener(ActiveCatchFeedback);
    }

    private void ActiveCatchFeedback()
    {
        if (m_catchAnimActive)
        {
            m_currentTime = 0.0f;
            m_aimCursor.rectTransform.localScale = m_oldSize * m_sizeIncrease;
            return;
        }
        m_oldSize = m_aimCursor.rectTransform.localScale;
        m_aimCursor.rectTransform.localScale = m_aimCursor.rectTransform.localScale * m_sizeIncrease;
        m_newSize = m_aimCursor.rectTransform.localScale;
        m_catchAnimActive = true;
    }
}
