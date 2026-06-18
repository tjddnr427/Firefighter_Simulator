using UnityEngine;

public class FireManager : MonoBehaviour
{
    public GameObject firePrefab;
    public Transform[] fireSpawnPoints;

    void Start()
    {
        foreach (Transform point in fireSpawnPoints)
            Instantiate(firePrefab, point.position, point.rotation);
    }
}
