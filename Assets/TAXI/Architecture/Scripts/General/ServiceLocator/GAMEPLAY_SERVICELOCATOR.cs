using UnityEngine;

public class GAMEPLAY_SERVICELOCATOR : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    private void Awake()
    {
        ServiceLocator serviceLocator = new ServiceLocator();
        ServiceLocator.current.Register<PlayerInput>(_playerInput);
    }
}
