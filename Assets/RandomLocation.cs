using UnityEngine;
using System.Collections.Generic;

public class RandomLocation : MonoBehaviour
{
    private List<Transform> locations = new List<Transform>();

    private void Start()
    {
        foreach (Transform child in transform)
        {
            locations.Add(child);
        }
    }

    public Transform GetRandomLocation()
    {
        // Shuffle alternative #1: Pick a random index
        int rand = Random.Range(0, locations.Count);
        return locations[rand];
    }
}
