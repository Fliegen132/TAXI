using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("TurnsValue")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float rotateSpeed;
    private ScriptableCars _car;
    private bool _isTurning;
    private float _turnStartTime;
    private float _newPosition = 3.7f;
    private float _oldPositon = 3.7f;
    private bool _right;
    private float y;
    private bool _back = false;
    private float _rotateAngle;
    private bool _crashed;
    //for speed
    private float _currentSpeed;
    public float CurrentSpeed => _currentSpeed;
    public float RealSpeed; 
    public float MaxSpeed;

    public Action<bool> DirectBtns;
    public Action BrakeBtn;

    private void Awake()
    {
        y = transform.rotation.eulerAngles.y;
        _currentSpeed = 10;
        _crashed = false;
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

    public void SetCar(ScriptableCars car)
    {
        _car = car;
        MaxSpeed = _car.MaxSpeed;
    }
    public ScriptableCars GetCar() => _car;

    private void FixedUpdate()
    {
        if (LossGame.GetEnd())
            return;
        MathPlayer();
        if (!_crashed)
        {
            if (_currentSpeed < MaxSpeed && !PlayerInput.GetBrake())
                _currentSpeed += Time.fixedDeltaTime * _car.Acceleration;
            if (_currentSpeed > MaxSpeed)
                _currentSpeed -= Time.fixedDeltaTime * _car.Acceleration;
        }
        
        if (_isTurning)
        {
            float time = Time.time - _turnStartTime;
            RotateFrameCar(time);
            TurnCar(time);
        }
    }

    private void TurnCar(float time)
    {
        if (!_back && _currentSpeed <= 60)
        {
            _back = true;
        }
        if (_back)
        {
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(_newPosition, transform.position.y, 0),
                time * turnSpeed * 2);
        }

        if (_right && transform.position.x < _newPosition + 0.85f)
        {
            if (!_back && _right)
            {
                transform.position = Vector3.Lerp(transform.position,
                new Vector3(_newPosition + 1f, transform.position.y, 0),
                time * turnSpeed * 2);
            }
        }

        else if (!_right && transform.position.x > _newPosition - 0.85f)
        {
            if (!_back && !_right)
            {
                transform.position = Vector3.Lerp(transform.position,
                new Vector3(_newPosition - 1f, transform.position.y, 0),
                time * turnSpeed * 2);
            }
        }
        else
            MoveBack();
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

        Quaternion direction = Quaternion.Euler(0, y + _rotateAngle, 0);
        if (_newPosition > _oldPositon || _newPosition < _oldPositon)
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, time);
    }

    public void DirectionTurn(bool right)
    {
        if (transform.position.x + 0.1f <= _newPosition || transform.position.x - 0.1f >= _newPosition)
            return;
        _back = false;
        _right = right;
        _oldPositon = (float)Math.Round(transform.position.x, 1);
        MathTurn();
    }

    private void MoveBack()
    {
        if (!_back)
        {
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
        if(_currentSpeed >= 25)
            _currentSpeed -= Time.fixedDeltaTime * 30;
    }
    //удалить в будущем
    public void AddMaxSpeed(float speed)
    {
        if (MaxSpeed >= 100)
            return;
        MaxSpeed += speed;
    }

    //удалить в будущем
    public void TakeMaxSpeed(float speed)
    {
        if (MaxSpeed <= 20)
            return;
        MaxSpeed -= speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            HitWall();
        }

        if (collision.gameObject.CompareTag("Bot"))
        {
            HitBot();
            _crashed = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bot"))
        {
            _crashed = false;
            HitBot();
        }
    }

    private void HitWall()
    {
        _isTurning = false;
        _right = !_right;
        _oldPositon = (float)Math.Round(transform.position.x, 1);
        _back = true;
        MathTurn();
    }

    private void HitBot()
    {
        if (CurrentSpeed >= 70)
        {
            LossGame.Loss?.Invoke();
        }
        else
        {
            _currentSpeed -= 10;
        }
    }
}
