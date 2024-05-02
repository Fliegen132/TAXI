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
            lossWindow.SetActive(true);

            foreach (GameObject go in _otherWindow)
            {
                go.SetActive(false);
            }

            _end = true;
        }
    }

    public static bool GetEnd() => _end;
}
