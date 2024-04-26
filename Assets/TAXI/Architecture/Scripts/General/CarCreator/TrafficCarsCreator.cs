using UnityEngine;

public class TrafficCarsCreator : Creator
{
    public override void CreateCar(string scriptaplePath, Transform parent = null)
    {
        var cars = Resources.LoadAll<GameObject>(scriptaplePath);
        for (int i = 0; i < cars.Length; i++)
        {
            GameObject car = Object.Instantiate(cars[i]);
            car.transform.SetParent(parent);
            StorageCars.IICars.Add(car);
            car.SetActive(false);
            car.AddComponent<IIController>();
        }
    }
}
