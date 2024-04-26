using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator 
{
    private List<GameObject> roads = new List<GameObject>();
    private readonly Transform _roadsParent;
    private float _distance = 80;
    public IEnumerable<GameObject> Roads => roads;

    public RoadGenerator(Transform parent)
    {
        _roadsParent = parent;
        Create();
        ReuseRoad();
    }

    private void Create()
    {
        for (int i = 0; i < 5; i++)
        {
            var loadGo = Resources.Load<GameObject>("Road");
            GameObject go = Object.Instantiate(loadGo, _roadsParent);
            go.SetActive(false);
            roads.Add(go);
        }
    }

    public void SubtractDistance()
    {
        _distance -= 80;
    }

    public void ReuseRoad()
    {
        for (int i = 0;i < roads.Count; i++) 
        {
            if (!roads[i].activeInHierarchy)
            {
                roads[i].SetActive(true);
                roads[i].transform.position = new Vector3(roads[i].transform.position.x,
                    roads[i].transform.position.y, roads[i].transform.position.z + _distance);
                _distance += 80;
            }
        }
    }
}
