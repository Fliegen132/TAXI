using UnityEngine;

public class Road : MonoBehaviour
{
    private TrafficGenerator _trafficGenerator;
    private void Start()
    {
        _trafficGenerator = FindObjectOfType<TrafficGenerator>(); 
        if (_trafficGenerator == null)
        {
            Debug.LogError("TrafficGenerator not found in the scene!");
        }
    }

    private void OnEnable()
    {
        _trafficGenerator?.CheckCars();
    }
}
