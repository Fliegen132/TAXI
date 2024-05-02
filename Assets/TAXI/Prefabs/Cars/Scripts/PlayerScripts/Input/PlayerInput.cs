using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public sealed class PlayerInput : MonoBehaviour
{
    private static readonly float m_turnForce = 7.4f;
    private static PlayerController m_Controller;
    private static bool _brake;
    [SerializeField] private Button leftBtn;
    [SerializeField] private Button rightBtn;
    [SerializeField] private TextMeshProUGUI speedText;

    private void Start()
    {
        m_Controller = StorageCars.Player.GetComponent<PlayerController>();
        leftBtn.onClick.AddListener(() => Direct(false));
        rightBtn.onClick.AddListener(() => Direct(true));
    }

    public static float Turn(bool right)
    {
        float currentTurn;
        if (right)
            currentTurn = m_turnForce;
        else
            currentTurn = -m_turnForce;

        return currentTurn;
    }

    private void FixedUpdate()
    {
        speedText.text = $"{(int)m_Controller.CurrentSpeed} ÊÌ/×";
        if (_brake)
            m_Controller?.BrakeBtn();
    }

    private void Direct(bool right)
    { 
        m_Controller?.DirectBtns(right);
    }

    public static bool GetBrake() => _brake;

    public void DoBrake(bool enabled)
    {
        _brake = enabled;
    }
    
}
