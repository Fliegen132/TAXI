using UnityEngine;
using UnityEngine.Rendering;

public class CameraBehaviour : MonoBehaviour
{
    private PlayerController _controller;
    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _controller = StorageCars.Player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_controller.CurrentSpeed > 80)
        {
            _animator.SetBool("Shake", true);
        }
        else 
        {
            _animator.SetBool("Shake", false);
        }
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    { 
        
    }
}
