using UnityEngine;

public class SeekerBehaviour : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hider"))
        {
            Debug.Log("Hider caught!");
            GameManager.Instance.EndGame(false);
        }
    }
}