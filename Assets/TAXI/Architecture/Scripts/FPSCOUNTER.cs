using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCOUNTER : MonoBehaviour
{
    private int FPS;
    [SerializeField] private TextMeshProUGUI text;
    private void Awake()
    {
        Application.targetFrameRate = 260;
        StartCoroutine(Fps());
    }

    private IEnumerator Fps()
    {
        yield return new WaitForSeconds(0.5f);
        FPS = (int)(1f / Time.unscaledDeltaTime);
        text.text = $"FPS: {FPS}";
        StartCoroutine(Fps());
    }
}
