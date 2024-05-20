using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TrafficRoad : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI textScore;
    private int _currentScore;
    private RoadGenerator _roadGenerator;
    private List<GameObject> _roads;
    private readonly int _maxSpacing = -220;
    
    private PlayerController _playerController;
    private void Awake()
    {
        _roadGenerator = new RoadGenerator(transform);
        ServiceLocator.current.Register<RoadGenerator>(_roadGenerator);
    }

    private void Start()
    {
        _roads = _roadGenerator.Roads.ToList();
        _playerController = StorageCars.Player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (LossGame.GetEnd())
            return;
        Move();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            AddSpeed();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RemoveSpeed();
        }
    }

    private void Move()
    {
        for (int i = 0; i < _roads.Count; i++)
        {
            if (_roads[i].activeInHierarchy)
            {
                _roads[i].transform.position = new Vector3(_roads[i].transform.position.x,
                    _roads[i].transform.position.y, 
                    _roads[i].transform.position.z - _playerController.RealSpeed * Time.fixedDeltaTime);

                if (_roads[i].transform.localPosition.z <= _maxSpacing)
                {
                    _roads[i].SetActive(false);
                    _currentScore++;
                    textScore.text = $"Ñ÷¸ò: {_currentScore}";
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
