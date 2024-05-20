using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("TurnsValue")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float rotateSpeed;
    private ScriptableCars _car;
    private Transform _busStop;
    private PassengerRoad _passengerRoad;
    private bool _isTurning;
    private float _turnStartTime;
    private float _newPosition = 3.7f;
    private float _oldPositon = 3.7f;
    private bool _right;
    private float y;
    private bool _back = false;
    private bool _oldBack;
    private float _rotateAngle;
    private const float _rotatePitch = 10;
    private bool _crashed;
    private bool _canTurnOpposite = true;
    private bool _awaitStart = false;
    private bool _canHit = true;
    private bool _enable = true;
    private bool _enableController = true;
    private bool _parked;
    private PlayerFullness _playerFullness;
    //for speed
    private float _currentSpeed;
    private Animator _mainCameraAnim;
    public float CurrentSpeed => _currentSpeed;
    public bool Parked => _parked;
    public float RealSpeed; 
    public float MaxSpeed;
    public Action<bool> DirectBtns;
    public Action BrakeBtn;
    private PlayerInput _playerInput;
    //for await turn
    private float _currentAwaitSomeTime;
    private const float _maxAwaitSomeTime = 0.65f;

    private bool _waitClick;
    private int _direction;
    #region mono

    private void Awake()
    {
        _mainCameraAnim = Camera.main.GetComponent<Animator>();
        y = transform.rotation.eulerAngles.y;
        _currentSpeed = 10;
        _crashed = false;
    }

    private void Start()
    {
        _playerFullness = GetComponent<PlayerFullness>();
        _passengerRoad = ServiceLocator.current.Get<PassengerRoad>();
        _playerInput = ServiceLocator.current.Get<PlayerInput>();
    }

    private void OnEnable()
    {
        DirectBtns += DirectionTurn;
        BrakeBtn += Brake;
    }

    private void OnDisable()
    {
        DirectBtns -= DirectionTurn;
        BrakeBtn -= Brake;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DirectionTurn(false);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            DirectionTurn(true);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerInput.DoBrake(true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _playerInput.DoBrake(false);
        }
    }

    private void FixedUpdate()
    {
        if (LossGame.GetEnd() )
            return;

        MathPlayer();
        CheckTime();
        Controll();
        AutoPilot();
        CorrectZ();
    }

    private void OnCollisionEnter(Collision collision)
    {
        HitWall(collision);
        HitBot(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bot"))
        {
            Debug.Log("Exit");
            _crashed = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("BusStop"))
        {
            _waitClick = true;
            Debug.Log("waitClick true");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("BusStop"))
        {
            _waitClick = false;
            Debug.Log("waitClick false");
        }
    }

    #endregion

    private void Controll()
    {
        if (!_crashed)
        {
            if (_enable)
            {
                if (_currentSpeed < MaxSpeed && !PlayerInput.GetBrake() && _enableController)
                {
                    _currentSpeed += Time.fixedDeltaTime * _car.Acceleration;
                    RotatePitch(true);
                }
                if (_currentSpeed >= MaxSpeed - 2)
                {
                    RotatePitch();
                }

                if (PlayerInput.GetBrake())
                {
                    RotatePitch(false);
                }

                if (_currentSpeed > MaxSpeed)
                {
                    _currentSpeed -= Time.fixedDeltaTime * _car.Acceleration;
                }
            }

            if (_isTurning)
            {
                float time = Time.time - _turnStartTime;
                RotateFrameCar(time);
                TurnCar(time);
            }
        }
    }

    private void CheckTime()
    {
        if (_awaitStart)
        {
            _currentAwaitSomeTime += Time.fixedDeltaTime;
        }
        if (_currentAwaitSomeTime >= _maxAwaitSomeTime)
        {
            _currentAwaitSomeTime = 0;
            _awaitStart = false;
        }
    }

    public void SetCar(ScriptableCars car)
    {
        _car = car;
        MaxSpeed = _car.MaxSpeed;
    }

    public ScriptableCars GetCar() => _car;

    private void TurnCar(float time)
    {
        if (!_back && _currentSpeed <= 60)
        {
            _back = true;
        }
        if (_back)
        {
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(_newPosition, transform.position.y, transform.position.z),
                time * turnSpeed * 2);
        }

        if (_right && transform.position.x < _newPosition + 0.85f)
        {
            if (!_back && _right)
            {
                transform.position = Vector3.Lerp(transform.position,
                new Vector3(_newPosition + 1f, transform.position.y, transform.position.z),
                time * turnSpeed * 2);
            }
        }

        else if (!_right && transform.position.x > _newPosition - 0.85f)
        {
            if (!_back && !_right)
            {
                transform.position = Vector3.Lerp(transform.position,
                new Vector3(_newPosition - 1f, transform.position.y, transform.position.z),
                time * turnSpeed * 2);
            }
        }
        else
            MoveBack();
    }

    private void RotatePitch(bool forward, float pitch = 5)
    {
        if (forward)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, y, transform.rotation.z + pitch), Time.fixedDeltaTime * 6);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, y, transform.rotation.z - pitch), Time.fixedDeltaTime * 6);
        }
    }

    private void RotatePitch()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, y, 0),
            Time.fixedDeltaTime * 10);
    }

    private void CorrectZ()
    {
        if (!_enable)
            return;
        transform.position = Vector3.Lerp(transform.position,
                new Vector3(transform.position.x, transform.position.y, 0),
                Time.fixedDeltaTime * .3f);
    }

    private void RotateFrameCar(float time)
    {
        if (_right && _rotateAngle > 0)
        {
            _rotateAngle -= time * rotateSpeed;
        }
        else if (!_right && _rotateAngle < 0)
        {
            _rotateAngle += time * rotateSpeed;
        }

        Quaternion direction = Quaternion.Euler(0, y + _rotateAngle, transform.rotation.eulerAngles.z);
        if (_newPosition > _oldPositon || _newPosition < _oldPositon)
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, time);
    }

    public void DirectionTurn(bool right)
    {
        if (_waitToStartOnPark)
        {
            CheckStart(right);
        }

        if (_enableController == false)
            return;
        if (_direction == 0)
        {
            if (_waitClick && right == _passengerRoad.Right)
            {
                if (!right)
                    _direction = -1;
                else
                    _direction = 1;
                return;
            }
        }

        

        if (_oldBack != right && _canTurnOpposite)
        {

            _canTurnOpposite = false;
            _awaitStart = true;
            _currentAwaitSomeTime = 0;
            Invoke(nameof(AwaitTrunOpposite), 0.3f);
            Turn(right);

        }
        else if (_oldBack == right && !_awaitStart)
        {
            _awaitStart = true;
            _currentAwaitSomeTime = 0;
            Turn(right);
        }
        _oldBack = right;
    }

    private void AwaitTrunOpposite()
    {
        _canTurnOpposite = true;
    }

    private void Turn(bool right)
    {
        _back = false;
        _right = right;
        _oldBack = right;
        _oldPositon = (float)Math.Round(transform.position.x, 1);
        MathTurn();

    }

    private void MoveBack()
    {
        if (!_back)
        {
            _oldBack = _right;
            _right = !_right;
            MathTurn(10, false);
            _back = true;
        }
    }

    private void MathTurn(float angle = 20, bool enabled = true)
    {
        if (_right)
            _rotateAngle = angle;
        else
            _rotateAngle = -angle;

        if (enabled)
        {
            _newPosition += PlayerInput.Turn(_right);
            _newPosition = (float)Math.Round(_newPosition, 1);
        }

        _isTurning = true;
        _turnStartTime = Time.time;
    }

    private void MathPlayer()
    {
        turnSpeed = PlayerMath.MathTrunSpeed(CurrentSpeed);
        rotateSpeed = PlayerMath.MathRotateSpeed(CurrentSpeed);
        RealSpeed = PlayerMath.MathSpeed(CurrentSpeed);
    }

    private void Brake()
    {
        if (!_enable)
            return;
        if (_currentSpeed >= 25)
        { 
            _currentSpeed -= Time.fixedDeltaTime * 45;
        }
    }
    //delete later
    public void AddMaxSpeed(float speed)
    {
        if (MaxSpeed >= 150)
            return;
        MaxSpeed += speed;
    }

    //delete later
    public void TakeMaxSpeed(float speed)
    {
        if (MaxSpeed <= 25)
            return;
        MaxSpeed -= speed;
    }


    private void HitWall(Collision collision)
    {
        if (!_canHit)
            return;
        _mainCameraAnim.Play("HitShake");
        if (collision.gameObject.CompareTag("Block"))
        {
            _isTurning = false;
            Turn(!_right);
            _oldBack = _right;
            _canTurnOpposite = false;
            _currentAwaitSomeTime = 0;
            Invoke(nameof(AwaitTrunOpposite), 0.4f);
            _canHit = false;
            Invoke(nameof(SetCanHit), 0.4f); 

        }
    }

    private void HitBot(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bot"))
        {
            _currentAwaitSomeTime = 0;
            _mainCameraAnim.Play("HitShake");
            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;
            if (collision.gameObject.GetComponent<IIController>().Oncoming)
            { 
                LossGame.Loss?.Invoke();
                return;
            }

            if (CurrentSpeed >= 70)
            {
                LossGame.Loss?.Invoke();
            }
            else
            {
                if (normal.z > 0.5f || normal.z < -0.5f)
                {
                   _currentSpeed = collision.gameObject.GetComponent<IIController>().CurrentSpeed - 3;
                }

                if (normal.x < -0.25f || normal.x > 0.25f)
                {
                    _isTurning = false;
                    Turn(!_right);
                    _oldBack = _right;
                    _currentSpeed -= 10;
                }

            }
        }
    }
    #region autoPilot

    private void AutoPilot()
    {
        Parking();
        LeaveParking();
    }

    private bool _canStop =true;

    private Transform _stopPoint;
    private Transform _startPoint;
    private bool _wasTurnToStop = false;
    private bool _wasTurnToStart = false;
    private bool _wasTakeOrDrop = false;
    private bool _waitToStartOnPark = false;
    public void Parking()
    {
        if (_parked || !_canStop)
            return;
        if (_busStop == null && RoadStorage.BusStop != null)
        {
            _busStop = RoadStorage.BusStop.GetComponent<PassengerRoad>().GetStopPoint();
        }
        if (_busStop == null)
        {
            Debug.Log("Bus stop Null");
            return;
        }
        if (_busStop.gameObject.activeInHierarchy)
        {
            if (RoadStorage.BusStop.GetComponent<PassengerRoad>().Right && _direction == 1 ||
            !RoadStorage.BusStop.GetComponent<PassengerRoad>().Right && _direction == -1) 
            {
                if (_stopPoint == null)
                { 
                    _stopPoint = RoadStorage.BusStop.GetComponent<PassengerRoad>().GetNeddedPointToStop();
                }
                if (_startPoint == null)
                {
                    _startPoint = RoadStorage.BusStop.GetComponent<PassengerRoad>().GetNeddedPoitnToStart();
                }
                CheckSpace();
                _enableController = false;

                if (_currentSpeed > 40)
                    _currentSpeed -= Time.fixedDeltaTime * 40;

                if (Vector3.Distance(transform.position, _stopPoint.position) < 15)
                {
                    if(!_wasTurnToStop)
                    { 
                        Turn(RoadStorage.BusStop.GetComponent<PassengerRoad>().Right);
                        _wasTurnToStop = true;
                    }
                }

                if (Vector3.Distance(transform.position, _busStop.position) <= 1.5f && !_wasTakeOrDrop)
                {
                    _currentSpeed = 0;
                    Invoke(nameof(TakeDropPassenger), 0.3f);
                }
            }
        }
    }

    private void TakeDropPassenger()
    {
        if (!_wasTakeOrDrop)
        { 
            RoadStorage.BusStop.GetComponent<PassengerRoad>().TakeDrop();
            _wasTakeOrDrop = true;
            SetParkedTrue();
        }

    }

    private void SetParkedTrue()
    {
        _parked = true;
    }

    private void SetParkedFalse()
    {
        _parked = false;
    }

    private void LeaveParking()
    {
        if (_parked)
        {
            if (_wasTakeOrDrop)
            {
                if (_currentSpeed < 30)
                    _currentSpeed += Time.fixedDeltaTime * 30;

                if (Vector3.Distance(transform.position, _startPoint.position) < 1.5)
                {
                    if (!_wasTurnToStart)
                    {
                        _currentSpeed = 0;
                        _enableController = false;
                        _waitToStartOnPark = true;
                    }
                }
            }
        }
    }

    private void CheckStart(bool right)
    {
        Debug.Log("this is check start");
        if (right != RoadStorage.BusStop.GetComponent<PassengerRoad>().Right)
        {
            Debug.Log("this is check start");
            SetDefault();
            Turn(!RoadStorage.BusStop.GetComponent<PassengerRoad>().Right);
        }
    }

    private void SetDefault()
    {
        _direction = 0;
        _enableController = true;
        _wasTurnToStop = false;
        _waitToStartOnPark = false;
        _wasTurnToStart = false;
        _wasTakeOrDrop = false;

        Invoke(nameof(SetParkedFalse), 3f);
    }

    private void CheckSpace()
    {
        if (Physics.Raycast(transform.position, transform.right, out var hitFront, 10))
        {
            if (hitFront.collider.gameObject.CompareTag("Bot"))
            {
                if (_currentSpeed > hitFront.collider.gameObject.GetComponent<IIController>().CurrentSpeed)
                {
                    _currentSpeed = hitFront.collider.gameObject.GetComponent<IIController>().CurrentSpeed;
                }
            }
        }
    }
    
    #endregion

    private void SetCanHit() => _canHit = true;
}
