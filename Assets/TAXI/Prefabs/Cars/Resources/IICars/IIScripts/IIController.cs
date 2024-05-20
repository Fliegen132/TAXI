using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IIController : MonoBehaviour
{
    private float _maxSpeed = 60f;
    private float _currentSpeed;
    private BoxCollider _boxCollider;
    private Rigidbody _rigidbody;
    private PlayerController _player;
    private const float rayLength = 10;
    private bool _right;
    private bool _stop;
    private float _rotateAngle;
    private const float _turnSpeed = 2f;
    private const float _rotateSpeed = 30;
    private float _newPosition;
    private float y;
    private bool _blink = false;
    private bool _oncoming;
    private bool _startTurn;
    private bool _turn;
    private float _waitTime;
    private bool _checkSide;
    public bool Oncoming => _oncoming;
    public float CurrentSpeed => _currentSpeed;
    private List<MeshRenderer> _meshes = new();
    private Vector3 _startRotation;
    private void Awake()
    {
        Init();
        _newPosition = transform.position.x;
    }

    private void Start()
    {
        _player = StorageCars.Player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (LossGame.GetEnd())
            return;

        Move();

        if(_checkSide)
            CheckSide();

        if (_stop)
        {
            Stop();
            if (_startTurn)
            {
                RotateFrame();
            }
        }
        if(!_oncoming)
            CheckSpace();
    }

    private void OnEnable()
    {
        SetDefault();
    }

   
    private void Init()
    {
        _meshes.Add(GetComponent<MeshRenderer>());
        for (int i = 0; i < transform.childCount; i++)
        {
            _meshes.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
        }
        y = transform.rotation.eulerAngles.y;
        _startRotation = transform.rotation.eulerAngles;
        _boxCollider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void SetDefault()
    {
        foreach (var mesh in _meshes)
        {
            mesh.enabled = true;
        }

        transform.rotation = Quaternion.Euler(_startRotation);
        if (transform.position.x > 0)
        {
            _oncoming = false;
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        if (transform.position.x < 0)
        {
            _oncoming = true;
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        _currentSpeed = Random.Range(_maxSpeed - 5, _maxSpeed);
        _stop = false;
        _maxSpeed = Random.Range(55, 60);
        _currentSpeed = _maxSpeed;
        _rotateAngle = 0;
        _startTurn = false;
        _boxCollider.enabled = true;
        _waitTime = 0;
        _turn = false;
        _newPosition = transform.position.x;
    }

    private void Move()
    {
        float maxSpeed;

        if (!_oncoming)
            maxSpeed = _currentSpeed - _player.CurrentSpeed;
        else
            maxSpeed = _currentSpeed + _player.CurrentSpeed / 4;

        float moveSpeed = maxSpeed * Time.fixedDeltaTime;

        Vector3 moveDirection = transform.right * moveSpeed;

        transform.Translate(moveDirection, Space.World);

        Vector3 targetPosition = new Vector3(_newPosition, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, _turnSpeed * Time.fixedDeltaTime);

        if (transform.localPosition.z <= -170 || transform.localPosition.z >= 330f)
        {
            gameObject.SetActive(false);
        }
    }


    private void MathTurn()
    {
        if (_right)
            _newPosition = transform.position.x + 7.4f;
        else
            _newPosition = transform.position.x - 7.4f;

        Debug.Log(_newPosition);
    }

    private void CheckSpace()
    {
        if (_stop)
            return;
        Vector3 center = transform.position;
        //сзади 
        if (Physics.Raycast(center, -transform.right, out var hitBack, 10))
        {
            if (hitBack.collider.gameObject.CompareTag("Player"))
            {
                
                if (_chooseSide == false)
                {
                    ChooseSide();
                    _checkSide = true;
                }
            }
        }
        //спереди
        else if (Physics.Raycast(center, transform.right, out var hitFront, 7))
        {
            if (hitFront.collider.gameObject.CompareTag("Player"))
            {
                Brake();
                if (_waitTime > 3 && !_turn)
                {
                    ChooseSide();
                    MathTurn();
                    _turn = true;
                    _waitTime = 0;
                }
                else
                {
                    _waitTime += Time.deltaTime;
                }
            }
            else if (hitFront.collider.gameObject.CompareTag("Bot"))
            {
                Brake();
            }
        }
        
        else
        {
            if (_stop)
                return;
            if (_maxSpeed < 65)
            {
                AaccelerationMaxSpeed();
                Aacceleration();
            }
        }
    }

    private void CheckSide()
    {
        if (_stop)
            return;
        Vector3 center = transform.position;
        Vector3 front = center + transform.forward * _boxCollider.size.z;
        Vector3 back = center + transform.forward * _boxCollider.size.z;
        Vector3 frontRight = front + transform.right * _boxCollider.size.x * 100 / 2;
        Vector3 backRight = back + transform.right * _boxCollider.size.x * 100 / 2 * -1;
        //for right side
        
        if (Physics.Raycast(center, -transform.forward, out var hitRight, rayLength))
        {
            if (hitRight.collider.gameObject.CompareTag("Bot"))
            {
                AaccelerationMaxSpeed();
                Aacceleration();
            }
        }
        else if (Physics.Raycast(frontRight, -transform.forward, out var hitRightFront, rayLength))
        {
            if (hitRightFront.collider.gameObject.CompareTag("Bot"))
            {
                AaccelerationMaxSpeed();
                Aacceleration();
            }
        }
        else if (Physics.Raycast(backRight, -transform.forward, out var hitRightBack, rayLength))
        {
            if (hitRightBack.collider.gameObject.CompareTag("Bot"))
            {
                AaccelerationMaxSpeed();
                Aacceleration();
            }
        }
        else if (Physics.Raycast(backRight, transform.forward, out var hitLeftBack, rayLength))
        {
            if (hitLeftBack.collider.gameObject.CompareTag("Bot"))
            {
                AaccelerationMaxSpeed();
                Aacceleration();
            }
        }
        if (Physics.Raycast(center, transform.forward, out var hitLeft, rayLength))
        {
            if (hitLeft.collider.gameObject.CompareTag("Bot"))
            {
                AaccelerationMaxSpeed();
                Aacceleration();
            }
        }
        if (Physics.Raycast(frontRight, transform.forward, out var hitLeftFront, rayLength))
        {
            if (hitLeftFront.collider.gameObject.CompareTag("Bot"))
            {
                AaccelerationMaxSpeed();
                Aacceleration();
            }

        }
        else
        {
            if (_waitTime > 1.5f && !_turn)
            {
                ChooseSide();
                MathTurn();
                _turn = true;
                _waitTime = 0;
            }
            else
            {
                _waitTime += Time.deltaTime;
            }
        }
    }

    private void RotateFrame()
    {
        if (_rotateAngle < -18 || _rotateAngle > 18)
            return;
        if (_right && _rotateAngle < 20)
        {
            _rotateAngle -= Time.fixedDeltaTime * _rotateSpeed;
            _rotateAngle = Mathf.Min(_rotateAngle, 20); 
        }
        else if (!_right && _rotateAngle > -20)
        {
            _rotateAngle += Time.fixedDeltaTime * _rotateSpeed;
            _rotateAngle = Mathf.Max(_rotateAngle, -20);
        }

        Quaternion direction = Quaternion.Euler(0, y + _rotateAngle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, direction, Time.fixedDeltaTime);
    }

    private void Brake()
    {
        if(_currentSpeed > 24)
        { 
            _currentSpeed -= Time.fixedDeltaTime * 10f;
            _maxSpeed -= Time.fixedDeltaTime * 4f;
        }
    }

    private void Aacceleration()
    {
        if (_currentSpeed < _maxSpeed)
        { 
            _currentSpeed += Time.fixedDeltaTime * 10f;
        }
    }

    private void AaccelerationMaxSpeed()
    {
        _maxSpeed += Time.fixedDeltaTime * 5f;
    }

    private void Stop()
    {
        if (_currentSpeed > 15)
            _currentSpeed -= Time.fixedDeltaTime * 45f;
        _maxSpeed = 0;
    }
    private bool _chooseSide = false;
    private void ChooseSide()
    {
        if (transform.position.x > 10)
        {
            _right = false;
        }

        if (transform.position.x < 5 && transform.position.x > 0)
        {
            _right = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") ||  collision.gameObject.CompareTag("Bot") )
        {
            if (_stop)
                return;
            _stop = true;
            Invoke(nameof(DeactivateBoxCollider), 1f);
            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;

            if (normal.z < -0.5f || normal.z > 0.5f)
            {
                _startTurn = false;
                return;
            }
                //сбоку
            if (normal.x < -0.5f || normal.x > 0.5f)
            {
                _startTurn = true;
                float x = normal.x;
                if (x > 0.5f)
                {
                    _right = false;
                }
                if (x < -0.5f)
                {
                    _right = true;
                }
            }
        }
    }

    private void DeactivateBoxCollider()
    {
        _boxCollider.enabled = false;
        if(gameObject.activeInHierarchy)
            StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        yield return new WaitForSeconds(0.3f);
        if (gameObject.activeInHierarchy)
        {

            if (_blink == false)
            {
                foreach (var mesh in _meshes)
                {
                    mesh.enabled = false;
                }
                _blink = true;

            }
            else
            {
                foreach (var mesh in _meshes)
                {
                    mesh.enabled = true;
                }
                _blink = false;
            }
            StartCoroutine(Blink());
        }
    }
}
