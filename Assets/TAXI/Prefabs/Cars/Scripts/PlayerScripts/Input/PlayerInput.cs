using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerInput : MonoBehaviour
{
    private static readonly float m_turnForce = 7.4f;
    private static PlayerController m_Controller;

    [SerializeField] private Button leftBtn;
    [SerializeField] private Button rightBtn;
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

    public void Direct(bool right)
    { 
        m_Controller?.Direct(right);
    }

    public bool Brake() => true;
    
}
