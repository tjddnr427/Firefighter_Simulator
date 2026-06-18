using UnityEngine;

public class FireManager : MonoBehaviour
{
    public GameObject firePrefab;

    void Start()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("FireSpawnPoint");
        foreach (GameObject point in points)
            Instantiate(firePrefab, point.transform.position, point.transform.rotation);
    }
}
