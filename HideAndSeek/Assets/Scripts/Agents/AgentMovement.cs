using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Vector3 startPosition = new Vector3(13f, 2f, 19f);
    
    private bool _isAlive = true;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // ResetPosition();
    }

    public void Move(Vector2 input)
    {
        Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

        if (direction != Vector3.zero)
        {
            // Vector3 targetPosition = rb.position + direction * (moveSpeed * Time.fixedDeltaTime);
            rb.AddForce(direction * (moveSpeed * Time.fixedDeltaTime), ForceMode.VelocityChange);
        }
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool IsAlive()
    {
        return _isAlive;
    }

    public void ResetPosition()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = startPosition;
        rb.rotation = Quaternion.identity;
    }
}