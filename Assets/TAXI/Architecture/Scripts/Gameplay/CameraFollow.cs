using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [field: SerializeField] public Transform target { private get; set; }

    public void SetTarger(Transform newTarget)
    {
        target = newTarget;
    }
    private void LateUpdate()
    {
        
    }

    public void Shacke()
    { 
        
    }

}
