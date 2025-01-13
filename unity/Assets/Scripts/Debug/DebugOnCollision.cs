using UnityEngine;

public class DebugOnCollision : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.name);

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);

    }
}
