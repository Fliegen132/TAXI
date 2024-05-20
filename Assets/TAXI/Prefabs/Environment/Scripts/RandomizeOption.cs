using UnityEngine;

public class RandomizeOption : MonoBehaviour
{
    private void OnEnable()
    {
        int i = Random.Range(0, transform.childCount);
        transform.GetChild(i).gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
