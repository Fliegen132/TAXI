using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TrafficRoad : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private RoadGenerator _roadGenerator;
    private List<GameObject> _roads;
    private readonly int _maxSpacing = -200;

    private PlayerController _playerController;
    
    private void Start()
    {
        _roadGenerator = new RoadGenerator(transform);
        _roads = _roadGenerator.Roads.ToList();
        _playerController = StorageCars.Player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _roads.Count; i++)
        {
            if (_roads[i].activeInHierarchy)
            {
                _roads[i].transform.position = new Vector3(_roads[i].transform.position.x,
                    _roads[i].transform.position.y, _roads[i].transform.position.z - _playerController.CurrentSpeed * Time.fixedDeltaTime);
                if (_roads[i].transform.localPosition.z <= _maxSpacing)
                {
                    _roads[i].SetActive(false);
                    _roadGenerator.SubtractDistance();
                    _roadGenerator.ReuseRoad();
                }
            }
        }
    }

    public void AddSpeed()
    {
        _playerController.AddMaxSpeed(10);
        SetText();
    } 

    public void RemoveSpeed()
    {
        _playerController.TakeMaxSpeed(10);
        SetText();
    }

    private void SetText()
    { 
        _text.text = _playerController.MaxSpeed.ToString();

    }
}
