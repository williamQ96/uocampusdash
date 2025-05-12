using UnityEngine;

public class Wander : MonoBehaviour
{
    [Header("Wander Settings")]
    public float walkSpeed = 2f;
    public float wanderRadius = 10f;
    public float waypointTolerance = 0.5f;
    public float pauseDuration = 1f;

    private Vector3 targetPoint;
    private float pauseTimer = 0f;
    private bool isPaused = false;

    void Start()
    {
        ChooseNewDestination();
    }

    void Update()
    {
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                ChooseNewDestination();
            }
            return;
        }

        // Move toward the target point
        Vector3 direction = (targetPoint - transform.position);
        direction.y = 0; // keep on same y‑level
        if (direction.magnitude <= waypointTolerance)
        {
            // Arrived: pause briefly, then pick a new point
            isPaused   = true;
            pauseTimer = pauseDuration;
        }
        else
        {
            // Walk forward
            transform.position += direction.normalized * walkSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 5f
            );
        }
    }

    private void ChooseNewDestination()
    {
        // Pick a random point in a circle around the initial position
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPoint = new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y,
            transform.position.z + randomCircle.y
        );
    }
}
