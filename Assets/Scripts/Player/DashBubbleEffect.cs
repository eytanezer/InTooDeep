using UnityEngine;

public class DashBubbleEffect : MonoBehaviour
{
    [SerializeField] private GameObject dashBubblePrefab;
    [SerializeField] private Transform dashBubblePoint;
    [SerializeField] private float bubbleSpeed = 8f;
    [SerializeField] private float destroyAfterSeconds = 2f;

    private void Awake()
    {
        FindBubblePointIfMissing();
    }

    private void FindBubblePointIfMissing()
    {
        if (dashBubblePoint == null)
        {
            dashBubblePoint = transform.Find("DashBubblePoint");
        }
    }

    public void PlayDashBubbles(Vector2 dashDirection)
    {
        Debug.Log("PLAY DASH BUBBLES CALLED");

        FindBubblePointIfMissing();

        if (dashBubblePrefab == null)
        {
            Debug.LogError("BUBBLE ERROR: Dash Bubble Prefab is missing on " + gameObject.name);
            return;
        }

        if (dashBubblePoint == null)
        {
            Debug.LogError("BUBBLE ERROR: Dash Bubble Point is missing on " + gameObject.name);
            return;
        }

        if (dashDirection == Vector2.zero)
        {
            Debug.LogError("BUBBLE ERROR: Dash direction is zero");
            return;
        }

        Vector2 bubbleDirection = -dashDirection.normalized;

        GameObject bubbles = Instantiate(
            dashBubblePrefab,
            dashBubblePoint.position,
            Quaternion.identity
        );

        bubbles.SetActive(true);

        ParticleSystem[] systems = bubbles.GetComponentsInChildren<ParticleSystem>(true);

        Debug.Log("BUBBLE OBJECT CREATED. Particle systems found: " + systems.Length);

        foreach (ParticleSystem ps in systems)
        {
            ps.gameObject.SetActive(true);

            var main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startSpeed = 0f;

            var velocity = ps.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.World;
            velocity.x = new ParticleSystem.MinMaxCurve(bubbleDirection.x * bubbleSpeed);
            velocity.y = new ParticleSystem.MinMaxCurve(bubbleDirection.y * bubbleSpeed);
            velocity.z = new ParticleSystem.MinMaxCurve(0f);

            ps.Clear(true);
            ps.Play(true);
        }

        Destroy(bubbles, destroyAfterSeconds);
    }
}