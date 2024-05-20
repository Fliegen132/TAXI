using System;
using UnityEngine;

public class LossGame : MonoBehaviour
{
    [SerializeField] private GameObject lossWindow;
    [SerializeField] private GameObject[] _otherWindow;
    private static bool _end;
    public static Action Loss;
    private void Awake()
    {
        lossWindow.SetActive(false);
        _end = false;
    }

    private void OnEnable()
    {
        Loss += EndGame;
    }

    private void OnDisable()
    {
        Loss -= EndGame;
    }

    public void EndGame()
    {
        if (!_end)
        { 
            _end = true;
            Invoke(nameof(ViewLoss), 2f);
        }
    }

    private void ViewLoss()
    {
        lossWindow.SetActive(true);

        foreach (GameObject go in _otherWindow)
        {
            go.SetActive(false);
        }
    }

    public static bool GetEnd() => _end;
}
