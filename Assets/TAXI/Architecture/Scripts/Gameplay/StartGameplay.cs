using UnityEngine;

public class StartGameplay : MonoBehaviour
{
    [SerializeField] private Transform trafficParent;
    private void Awake()
    {
        Creator playerCreator = new PlayerCreator();
        playerCreator.CreateCar("PlayerCars/SO/FirstCar");

        Creator trafficCreator = new TrafficCarsCreator();
        trafficCreator.CreateCar("IICars/Prefabs", trafficParent);
    }
}
