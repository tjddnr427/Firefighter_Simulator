using UnityEngine;

public class Water : MonoBehaviour
{
    public float moveSpeed = 10f;

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            GameManager.Instance.AddScore(100);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
