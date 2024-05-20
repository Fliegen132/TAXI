using UnityEngine;

public class PlayerCreator : Creator
{
    public override void CreateCar(string scriptaplePath, Transform parent = null)
    {
        var scriptableCars = Resources.Load<ScriptableCars>(scriptaplePath);
        PlayerController car = Object.Instantiate(scriptableCars.Prefab).AddComponent<PlayerController>();
        car.SetCar(scriptableCars);
        car.gameObject.AddComponent<PlayerFullness>();
        StorageCars.Player = car.gameObject;
    }
}
