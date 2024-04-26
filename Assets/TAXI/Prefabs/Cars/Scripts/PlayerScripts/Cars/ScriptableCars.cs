using UnityEngine;
[CreateAssetMenu(fileName ="NewCar", menuName ="Cars")]
public class ScriptableCars : ScriptableObject
{
    public string Id;
    public float MaxSpeed;
    public float Acceleration;
    public GameObject Prefab;
}
