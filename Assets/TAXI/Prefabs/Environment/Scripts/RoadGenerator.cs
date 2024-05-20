using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : IService
{
    private List<GameObject> _roads = new List<GameObject>();
    private GameObject _busStop;
    private readonly Transform _roadsParent;
    private float _distance = 80;

    private readonly int _minPassengerDistance = 15;
    private readonly int _maxPassengerDistance = 25;
    private int _currentPassengerDistance;
    private int _currentPassIndex;
    public List<GameObject> Roads = new List<GameObject>();
    private bool _right;
    private PassengerRoad _passengerRoad;
    public RoadGenerator(Transform parent)
    {
        _roadsParent = parent;
        Init();
        ReuseRoad();
    }

    public void RecalculateDistanceToPass()
    {
        if (_passengerRoad == null)
        {
            _passengerRoad = _busStop.GetComponent<PassengerRoad>();
        }
        if (!_busStop.gameObject.activeInHierarchy)
        {
            int a = Random.Range(0, 2);

            if (a == 0)
            {
                _right = true;
            }
            else
            {
                _right = false;
            }
            _passengerRoad.SetRight(_right);
            _currentPassengerDistance = Random.Range(_minPassengerDistance, _maxPassengerDistance);
            Debug.Log("Пересчиталось");
        }
    }

    public bool GetRight() => _right;

    public int GetDistance()
    {
        int result = _currentPassengerDistance - _currentPassIndex;
        return result;
    }


    private void Init()
    {
        for (int i = 0; i < 5; i++)
        {
            var loadGo = Resources.Load<GameObject>("Road");
            GameObject go = Object.Instantiate(loadGo, _roadsParent);
            go.SetActive(false);
            _roads.Add(go);
        }
        var loadBusStop = Resources.Load<GameObject>("Stop");
        _busStop = Object.Instantiate(loadBusStop, _roadsParent);
        _busStop.SetActive(false);
        RoadStorage.BusStop = _busStop;
        ServiceLocator.current.Register<PassengerRoad>(_busStop.GetComponent<PassengerRoad>());
        Roads.AddRange(_roads);
        Roads.Add(_busStop);
        RecalculateDistanceToPass();
    }

    public void SubtractDistance()
    {
        if (LossGame.GetEnd())
            return;
        _distance -= 80;
    }

    public void ReuseRoad()
    {
        for (int i = 0;i < _roads.Count; i++) 
        {
            if (_currentPassIndex - 5 >= _currentPassengerDistance)
            {
                if (!_busStop.activeInHierarchy)
                {
                    RecalculateDistanceToPass();
                    _busStop.SetActive(true);
                    _busStop.transform.position = new Vector3(_busStop.transform.position.x,
                        _busStop.transform.position.y, _busStop.transform.position.z + _distance); 
                    _distance += 80;
                    _currentPassIndex = 0;
                }
            }

            else 
            {
                if (!_roads[i].activeInHierarchy)
                {
                    _roads[i].SetActive(true);
                    _roads[i].transform.position = new Vector3(_roads[i].transform.position.x,
                        _roads[i].transform.position.y, _roads[i].transform.position.z + _distance);
                   
                    _currentPassIndex++;
                    _distance += 80;
                }
            }
        }
    }
}
