using UnityEngine;

public class RandomizeOption : MonoBehaviour
{
    private float[] lines = { -11.1f, -3.7f, 3.7f, 11.1f};

    private void OnEnable()
    {
        int i = Random.Range(0, transform.childCount);
        transform.GetChild(i).gameObject.SetActive(true);

        GenerateCars();
    }


    private void GenerateCars()
    {
        int survive = 0;
        for (int i = 0; i < StorageCars.IICars.Count; i++)
        {
            if (StorageCars.IICars[i].activeInHierarchy)
            {
                survive++;
            }
        }
        if (survive <= 0)
        {
            for(int i = 0; i < 7; i++)
            {
                int randomCar = Random.Range(0, StorageCars.IICars.Count);
                int randomLine = Random.Range(0, lines.Length);
                if (!StorageCars.IICars[randomCar].gameObject.activeInHierarchy)
                {
                    StorageCars.IICars[randomCar].transform.position = new Vector3(lines[randomLine], StorageCars.IICars[randomCar].transform.position.y, Random.Range(100, 150));
                    StorageCars.IICars[randomCar].SetActive(true);
                }
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
