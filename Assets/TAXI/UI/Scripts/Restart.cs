using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void Res()
    {
        StorageCars.Player = null;
        StorageCars.IICars = new List<GameObject>();

        SceneManager.LoadScene(0);
    }
}
