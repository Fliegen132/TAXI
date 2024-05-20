using UnityEngine;

public class PassengerRoad : Road, IService
{
    private Transform _stopPoint;
    public bool Right;
    [SerializeField] private GameObject leftStopLine, rightStopLine;
    [SerializeField] private GameObject[] passengers;
    private PlayerFullness _playerFullness;
    [SerializeField] private GameObject _currentPassenger;

    [SerializeField] private Transform firstTurnPoint;
    [SerializeField] private Transform secondTurnPoint;

    private Transform _currentPlayerNeededPointToStop;
    private Transform _currentPlayerNeededPointToStart;
    private void Awake()
    {
        _stopPoint = transform.Find("StopPoint");
    }

    private void Start()
    {
        _playerFullness = StorageCars.Player.GetComponent<PlayerFullness>();
    }

    public void SetRight(bool right)
    {
        Right = right;

        if (Right)
        {
            _currentPlayerNeededPointToStart = secondTurnPoint;
            _currentPlayerNeededPointToStop = firstTurnPoint;
        }
        else 
        {
            _currentPlayerNeededPointToStart = firstTurnPoint;
            _currentPlayerNeededPointToStop = secondTurnPoint;
        }
    }

    public Transform GetNeddedPointToStop() => _currentPlayerNeededPointToStop;

    public Transform GetNeddedPoitnToStart() => _currentPlayerNeededPointToStart;

    private void OnEnable()
    {
        if (_playerFullness == null)
        {
            if (StorageCars.Player == null)
            {
                return;
            }
            else
            {
                _playerFullness = StorageCars.Player.GetComponent<PlayerFullness>();
            }
        }
        leftStopLine.SetActive(false);
        rightStopLine.SetActive(false);

        if (Right)
        {
            rightStopLine.SetActive(true);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            leftStopLine.SetActive(true);
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        CheckPlayer();
    }

    private void CheckPlayer()
    {
        if (_playerFullness.HavePassenger)
        {
            _currentPassenger = _playerFullness.GetPassenger();
            return;
        }
        else
        {
            _currentPassenger = passengers[Random.Range(0, passengers.Length)];
            _currentPassenger.SetActive(true);
        }
    }

    public void TakeDrop()
    {
        if (_playerFullness.HavePassenger)
        {
            DropPassenger();
        }
        else
        {
            TakePassenger();
        }
    }

    private void TakePassenger()
    {
        _currentPassenger.SetActive(false);
        _playerFullness.SetPassenger(_currentPassenger);
        _playerFullness.HavePassenger = true;
        Debug.Log("Вы взяли пассажира");
    }

    private void DropPassenger()
    {
        _currentPassenger.SetActive(true);
        _playerFullness.SetPassenger(null);
        _playerFullness.HavePassenger = false;
        //add money
        Debug.Log("Высадили и заработали 100");
    }

    public Transform GetStopPoint() => _stopPoint;

    
}
