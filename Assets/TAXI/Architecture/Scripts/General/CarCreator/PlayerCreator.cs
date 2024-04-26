using UnityEngine;

public class PlayerCreator : Creator
{
    public override void CreateCar(string scriptaplePath, Transform parent = null)
    {
        var scriptableCars = Resources.Load<ScriptableCars>(scriptaplePath);
        PlayerController car = Object.Instantiate(scriptableCars.Prefab).AddComponent<PlayerController>();
        car.SetCar(scriptableCars);
        StorageCars.Player = car.gameObject;
    }
}
