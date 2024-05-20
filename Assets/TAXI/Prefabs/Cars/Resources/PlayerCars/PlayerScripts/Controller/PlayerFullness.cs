using UnityEngine;

public class PlayerFullness : MonoBehaviour
{
    public bool HavePassenger;
    private GameObject _passenger;

    public GameObject GetPassenger() => _passenger;

    public void SetPassenger(GameObject passenger)
    { 
        _passenger = passenger;
    }
}
