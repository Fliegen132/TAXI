using System.Collections.Generic;
using UnityEngine;

public class TrafficGenerator : MonoBehaviour
{
    private float[] _lines = { -11.1f, -3.7f, 3.7f, 11.1f };
    private int _carCount = 4;
    private List<GameObject> _cars;
    private int _lastActiveIndex = 0;
    private float _lastZCoordinate = 100f;
    private float _lastSpawnPoint;
    private RoadGenerator _roadGenerator;
    private List<GameObject> _firstLineCars = new List<GameObject>();
    private List<GameObject> _secondLineCars = new List<GameObject>();
    private List<GameObject> _thirdLineCars = new List<GameObject>();
    private List<GameObject> _fourthLineCars = new List<GameObject>();
    private void Start()
    {
        _cars = StorageCars.IICars;
        _roadGenerator = ServiceLocator.current.Get<RoadGenerator>();
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

    private void GenerateCars()
    {
        for (int i = 0; i < _carCount; i++)
        {
            int currentIndex = (_lastActiveIndex + i) % _cars.Count;

            int randomLine = Random.Range(0, _lines.Length);

            float newZCoordinate = _lastZCoordinate + 10f;

            bool right;
            if (_roadGenerator.GetDistance() <= 7)
            {
                right = CheckDistance();
                if (right)
                {
                    randomLine = Random.Range(1, _lines.Length);
                }
            }

            bool occupied = false;
            foreach (var car in _cars)
            {
                if (car.activeInHierarchy && car.transform.position.x == _lines[randomLine] && Mathf.Abs(car.transform.position.z - newZCoordinate) < 10f)
                {
                    occupied = true;
                    break;
                }
            }

            if (!_cars[currentIndex].gameObject.activeInHierarchy && !occupied)
            {
                if (RoadStorage.BusStop.activeInHierarchy && randomLine == 3)
                    continue;
                float a = newZCoordinate;
                if (_lastSpawnPoint != _lines[randomLine])
                {
                    a = _lastZCoordinate - 10f;
                }
                _cars[currentIndex].transform.position = new Vector3(_lines[randomLine],
                    _cars[currentIndex].transform.position.y, a);

                _lastSpawnPoint = _lines[randomLine];
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

    private bool CheckDistance()
    {
        return _roadGenerator.GetRight();
    }
}
