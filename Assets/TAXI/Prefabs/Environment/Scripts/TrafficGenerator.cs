using System.Collections.Generic;
using UnityEngine;

public class TrafficGenerator : MonoBehaviour
{
    private float[] _lines = { -11.1f, -3.7f, 3.7f, 11.1f };
    private int _carCount = 4;
    private List<GameObject> _cars;
    private int _index = 0;

    private void Start()
    {
        _cars = StorageCars.IICars;
    }

    public void CheckCars()
    {
        bool canSpawn = true;
        foreach (var car in _cars)
        {
            if (car.activeInHierarchy)
            {
                canSpawn = false;
            }
        }

        if (canSpawn)
            GenerateCars();

    }

    private int _lastActiveIndex = 0;
    private float _lastZCoordinate = 100f; // начальная координата Z

    private void GenerateCars()
    {
        for (int i = 0; i < _carCount; i++)
        {
            int currentIndex = (_lastActiveIndex + i) % _cars.Count;

            int randomLine = Random.Range(0, _lines.Length);

            float newZCoordinate = _lastZCoordinate + 10f; 

            if (!_cars[currentIndex].gameObject.activeInHierarchy)
            {
                _cars[currentIndex].transform.position = new Vector3(_lines[randomLine], _cars[currentIndex].transform.position.y, newZCoordinate);
                _cars[currentIndex].SetActive(true);
                _lastActiveIndex = currentIndex;
                _lastZCoordinate = newZCoordinate; 
            }
        }
        _lastZCoordinate = 100;
        if (_carCount >= 20) 
            return;
        _carCount++;
    }
}
