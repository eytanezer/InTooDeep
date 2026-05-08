using UnityEngine;

public class DashBubbleEffect : MonoBehaviour
{
    [SerializeField] private GameObject dashBubblePrefab;
    [SerializeField] private Transform dashBubblePoint;
    [SerializeField] private float bubbleSpeed = 3f;

    public void PlayDashBubbles(Vector2 dashDirection)
    {
        if (dashBubblePrefab == null || dashBubblePoint == null || dashDirection == Vector2.zero)
        {
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

        Destroy(bubbles, 2f);
    }
}