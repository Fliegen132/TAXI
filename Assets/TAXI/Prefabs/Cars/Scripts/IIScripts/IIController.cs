using UnityEngine;

public class IIController : MonoBehaviour
{
    private float _maxSpeed = 49f;
    private float _currentSpeed;
    private BoxCollider _boxCollider;
    private Rigidbody _rigidbody;
    private readonly float rayLength = 10;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _maxSpeed = Random.Range(40, 60);
    }

    private void FixedUpdate()
    {
        Debug.Log(_maxSpeed);
        Move();
        CheckSpace();
    }

    private void Move()
    {
        float maxSpeed = _maxSpeed - StorageCars.Player.GetComponent<PlayerController>().CurrentSpeed;
        float moveSpeed = maxSpeed * Time.fixedDeltaTime;
        Vector3 moveDirection = transform.right * moveSpeed;
        _rigidbody.MovePosition(transform.position + moveDirection);
        if (transform.localPosition.z <= -170)
        {
            gameObject.SetActive(false);
        }
    }

    private void Turn()
    {
        
    }

    public void SetDefault()
    { 
        //if dead///
    }

    private void CheckSpace()
    {
        Vector3 center = transform.position;
        Vector3 front = center + transform.forward * _boxCollider.size.z;
        Vector3 back = center + transform.forward * _boxCollider.size.z;
        Vector3 frontRight = front + transform.right * _boxCollider.size.x * 100 / 2;
        Vector3 backRight = back + transform.right * _boxCollider.size.x * 100 / 2 * -1;

        
        if (Physics.Raycast(center, transform.forward, out var hitLeft, rayLength))
        {
            if (hitLeft.collider.gameObject.CompareTag("Player"))
                Debug.Log("hitLeft: " + hitLeft.collider.gameObject.name);
        }
        if (Physics.Raycast(center, -transform.forward, out var hitRight, rayLength))
        {
            if (hitRight.collider.gameObject.CompareTag("Player"))
                Debug.Log("hitRight: " + hitRight.collider.gameObject.name);
        }

        if (Physics.Raycast(center, transform.right, out var hitFront, rayLength))
        {
            if (hitFront.collider.gameObject.CompareTag("Player"))
                Debug.Log("Front: " + hitFront.collider.gameObject.name);
        }

        if (Physics.Raycast(center, -transform.right, out var hitBack, rayLength))
        {
            if (hitBack.collider.gameObject.CompareTag("Player"))
                Debug.Log("Back: " + hitBack.collider.gameObject.name);
        }


        if (Physics.Raycast(frontRight, -transform.forward, out var hitRightFront, rayLength))
        {
            if (hitRightFront.collider.gameObject.CompareTag("Player"))
                Debug.Log("RightFront: " + hitRightFront.collider.gameObject.name);
        }
        if (Physics.Raycast(frontRight, transform.forward, out var hitLeftFront, rayLength))
        {
            if (hitLeftFront.collider.gameObject.CompareTag("Player"))
                Debug.Log("hitLeftFront: " + hitLeftFront.collider.gameObject.name);
        }

        if (Physics.Raycast(backRight, -transform.forward, out var hitRightBack, rayLength))
        {
            if (hitRightBack.collider.gameObject.CompareTag("Player"))
                Debug.Log("hitRightBack: " + hitRightBack.collider.gameObject.name);
        }

        if (Physics.Raycast(backRight, transform.forward, out var hitLeftBack, rayLength))
        {
            if (hitLeftBack.collider.gameObject.CompareTag("Player"))
                Debug.Log("hitLeftBack: " + hitLeftBack.collider.gameObject.name);
        }

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

}
