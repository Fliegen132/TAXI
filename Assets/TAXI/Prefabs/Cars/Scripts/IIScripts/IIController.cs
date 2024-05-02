using UnityEngine;

public class IIController : MonoBehaviour
{
    private float _maxSpeed = 60f;
    private BoxCollider _boxCollider;
    private Rigidbody _rigidbody;
    private PlayerController _player;
    private readonly float rayLength = 10;
    private bool _right;
    private bool _oncoming;
    private bool _brake;
    private bool _stop;
    /// <summary>
    /// old info
    /// </summary>

    private Vector3 _rotation;

    private void Start()
    {
        _rotation = transform.rotation.eulerAngles;
        _boxCollider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _player = StorageCars.Player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (LossGame.GetEnd())
            return;
        Move();

        if(!_oncoming)
            CheckSpace();
    }

    private void Move()
    {
        if (!_brake)
        {
            if (!_oncoming)
            {
                float maxSpeed = _maxSpeed - _player.CurrentSpeed;
                float moveSpeed = maxSpeed * Time.fixedDeltaTime;
                Vector3 moveDirection = moveSpeed * transform.right;
                _rigidbody.MovePosition(transform.position + moveDirection);

            }
            else
            {
                float maxSpeed = _maxSpeed;
                float moveSpeed = maxSpeed * Time.fixedDeltaTime;
                Vector3 moveDirection = moveSpeed * transform.right;
                _rigidbody.MovePosition(transform.position + moveDirection);
            }
        }
        else
            Break();

        if (transform.localPosition.z <= -170)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        SetDefault();
    }

    public void SetDefault()
    {
        _brake = false;
        _stop = false;
        transform.rotation = Quaternion.Euler(_rotation);

        if (transform.position.x > 0)
        {
            _oncoming = false;
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        if(transform.position.x < 0)
        {
            _oncoming = true;
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private void Turn()
    {

    }

    private void CheckSpace()
    {
        Vector3 center = transform.position;
        //в первую очередь
        if (Physics.Raycast(center, transform.right, out var hitFront, 5))
        {
            if (hitFront.collider.gameObject.CompareTag("Player") || hitFront.collider.gameObject.CompareTag("Bot"))
            {
                Debug.Log("Back: " + hitFront.collider.gameObject.name);
                _brake = true;
            }
        }
        else
        {
            _brake = false;
            SetDefaultSpeed();
        }

        if (Physics.Raycast(center, -transform.right, out var hitBack, 5))
        {
            if (hitBack.collider.gameObject.CompareTag("Player"))
                Debug.Log("Back: " + hitBack.collider.gameObject.name);
        }
        //--------------------
    }

    private void CheckSide(ref bool right)
    {
        Vector3 center = transform.position;
        Vector3 front = center + transform.forward * _boxCollider.size.z;
        Vector3 back = center + transform.forward * _boxCollider.size.z;
        Vector3 frontRight = front + transform.right * _boxCollider.size.x * 100 / 2;
        Vector3 backRight = back + transform.right * _boxCollider.size.x * 100 / 2 * -1;
        //for right side
        if (Physics.Raycast(center, -transform.forward, out var hitRight, rayLength))
        {
            if (hitRight.collider.gameObject.CompareTag("Player"))
                Debug.Log("hitRight: " + hitRight.collider.gameObject.name);
        }

        if (Physics.Raycast(frontRight, -transform.forward, out var hitRightFront, rayLength))
        {
            if (hitRightFront.collider.gameObject.CompareTag("Player"))
                Debug.Log("RightFront: " + hitRightFront.collider.gameObject.name);
        }

        if (Physics.Raycast(backRight, -transform.forward, out var hitRightBack, rayLength))
        {
            if (hitRightBack.collider.gameObject.CompareTag("Player"))
                Debug.Log("hitRightBack: " + hitRightBack.collider.gameObject.name);
        }

        //for left side
        if (Physics.Raycast(backRight, transform.forward, out var hitLeftBack, rayLength))
        {
            if (hitLeftBack.collider.gameObject.CompareTag("Player"))
                Debug.Log("hitLeftBack: " + hitLeftBack.collider.gameObject.name);
        }
        if (Physics.Raycast(center, transform.forward, out var hitLeft, rayLength))
        {
            if (hitLeft.collider.gameObject.CompareTag("Player"))
                Debug.Log("hitLeft: " + hitLeft.collider.gameObject.name);
        }


        if (Physics.Raycast(frontRight, transform.forward, out var hitLeftFront, rayLength))
        {
            if (hitLeftFront.collider.gameObject.CompareTag("Player"))
                Debug.Log("hitLeftFront: " + hitLeftFront.collider.gameObject.name);
        }
    }

    private void Stop()
    {
        float moveSpeed = Time.fixedDeltaTime * 1;
        Vector3 moveDirection = -moveSpeed * transform.right;
        _rigidbody.MovePosition(transform.position + moveDirection);
    }

    private void Break()
    {
        if (_maxSpeed <= 60)
        {
            _maxSpeed = _player.CurrentSpeed;
        }
        else 
        {
            _maxSpeed = 60;
        }
        float maxSpeed = _maxSpeed - _player.CurrentSpeed;
        float moveSpeed = maxSpeed * Time.fixedDeltaTime;
        Vector3 moveDirection = moveSpeed * transform.right;
        _rigidbody.MovePosition(transform.position + moveDirection);
    }

    private void SetDefaultSpeed()
    {
        _maxSpeed = 60;
    }

    private void OnDrawGizmos()
    {
        BoxCollider _boxCollider = GetComponent<BoxCollider>();

        Vector3 center = transform.position;
        Vector3 front = center + transform.forward * _boxCollider.size.z;
        Vector3 back = center - transform.forward * _boxCollider.size.z;
        Vector3 frontRight = front + transform.right * _boxCollider.size.x * 100/2;
        Vector3 frontLeft = front - transform.right * _boxCollider.size.x * 100/2;
        Vector3 backRight = back + transform.right * _boxCollider.size.x * 100/2;
        Vector3 backLeft = back - transform.right * _boxCollider.size.x * 100 / 2;

        Debug.DrawRay(center, transform.forward * 10);
        Debug.DrawRay(center, -transform.forward * 10);

        Debug.DrawRay(center, -transform.right * 5);
        Debug.DrawRay(center, transform.right * 5);

        Debug.DrawRay(frontRight, transform.forward * 10);
        Debug.DrawRay(frontLeft, transform.forward * 10);
        Debug.DrawRay(backRight, -transform.forward * 10);
        Debug.DrawRay(backLeft, -transform.forward * 10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bot"))
        {
            _brake = true;
            Turn();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bot"))
        {
            _stop = true;
            Stop();
        }
    }
}
