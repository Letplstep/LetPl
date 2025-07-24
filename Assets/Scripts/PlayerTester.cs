using UnityEngine;

public class PlayerTester : MonoBehaviour
{
    public float moveSpeed = 2f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tile"))
        {
            Debug.Log("≈∏¿œ π‚¿Ω: " + other.name);
        }
    }
}