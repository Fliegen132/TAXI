using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("TurnsValue")]
    [SerializeField] private float turnSpeed = 0.1f;
    [SerializeField] private float rotateSpeed = 2;
    private Rigidbody _rigidbody;
    private ScriptableCars _car;
    private bool _isTurning;
    private float _turnStartTime;
    private float _newPosition = 3.7f;
    private float _oldPositon = 3.7f;
    private bool _right;
    private float y;
    private bool _back = false;
    private float _rotateAngle;

    //for speed
    private float _currentSpeed;
    public float CurrentSpeed => _currentSpeed;
    public float MaxSpeed;

    public Action<bool> Direct;
    private void Awake()
    {
        y = transform.rotation.eulerAngles.y;
        _currentSpeed = 30;
    }

    private void OnEnable()
    {
        Direct += DirectionTurn;
    }

    private void OnDisable()
    {
        Direct -= DirectionTurn;
    }

    public void SetCar(ScriptableCars car)
    {
        _car = car;
        MaxSpeed = _car.MaxSpeed;
    }
    public ScriptableCars GetCar() => _car;
   

    private void FixedUpdate()
    {
        if (_currentSpeed < MaxSpeed)
            _currentSpeed += Time.fixedDeltaTime * _car.Acceleration;
        if (_currentSpeed > MaxSpeed)
            _currentSpeed -= Time.fixedDeltaTime * _car.Acceleration;
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
                time * turnSpeed * 3);
        }

        if (_right && transform.position.x < _newPosition + 0.85f)
        {
            if (!_back && _right)
            {
                transform.position = Vector3.Lerp(transform.position,
                new Vector3(_newPosition + 1f, transform.position.y, 0),
                time * turnSpeed);
            }
        }

        else if (!_right && transform.position.x > _newPosition - 0.85f)
        {
            if (!_back && !_right)
            {
                transform.position = Vector3.Lerp(transform.position,
                new Vector3(_newPosition - 1f, transform.position.y, 0),
                time * turnSpeed);
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

    public void AddMaxSpeed(float speed)
    {
        if (MaxSpeed >= 220)
            return;
        if (MaxSpeed >= 40)
        {
            turnSpeed += 0.05f;
            rotateSpeed += 1f;
        }
        
        MaxSpeed += speed;
    }

    public void TakeMaxSpeed(float speed)
    {
        if (MaxSpeed <= 20)
            return;
        if (MaxSpeed >= 50)
        { 
            turnSpeed -= 0.05f;
            rotateSpeed -= 1f;
        }
        MaxSpeed -= speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            _isTurning = false;
            _right = !_right;
            _oldPositon = (float)Math.Round(transform.position.x, 1);
            _back = true;
            MathTurn();
        }
    }
}
