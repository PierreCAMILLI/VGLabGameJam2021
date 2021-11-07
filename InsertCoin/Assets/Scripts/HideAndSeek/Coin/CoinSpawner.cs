using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField]
    private Coin _coin;

    private Transform[] _coinLocations;

    private void Start()
    {
        // GetComponentInChildren also takes the parent, we don't want that
        List<Transform> locations = new List<Transform>();
        foreach(Transform location in transform)
        {
            locations.Add(location);
        }
        _coinLocations = locations.ToArray();
    }

    public void SpawnCoinAtRandomLocation()
    {
        Transform randomLocation = _coinLocations[Random.Range(0, _coinLocations.Length)];
        _coin.gameObject.SetActive(true);
        _coin.transform.position = randomLocation.position;
        _coin.transform.localRotation = randomLocation.localRotation;
        _coin.OnSpawn();
    }
}
