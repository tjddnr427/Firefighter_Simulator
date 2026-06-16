using UnityEngine;

public class FireManager : MonoBehaviour
{
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private Transform[] fireSpawnPoints;

    void Start()
    {
        foreach (Transform point in fireSpawnPoints)
            Instantiate(firePrefab, point.position, point.rotation);
    }
}
