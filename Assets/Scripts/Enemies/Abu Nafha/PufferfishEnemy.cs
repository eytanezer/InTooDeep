using Player;
using UnityEngine;

public class PufferfishEnemy : MonoBehaviour
{
    [Header("Inflation")]
    [SerializeField] private float inflatedScaleMultiplier = 1.8f;
    [SerializeField] private float inflateSpeed = 5f;
    [SerializeField] private float deflateSpeed = 3f;

    [Header("Damage")]
    [SerializeField] private float normalDamage = 5f;
    [SerializeField] private float inflatedDamage = 15f;

    private Vector3 _normalScale;
    private Vector3 _inflatedScale;

    private bool _isInflated;

    private void Awake()
    {
        _normalScale = transform.localScale;
        _inflatedScale = _normalScale * inflatedScaleMultiplier;
    }

    private void Update()
    {
        Vector3 targetScale;

        if (_isInflated)
        {
            targetScale = _inflatedScale;
        }
        else
        {
            targetScale = _normalScale;
        }

        float speed;

        if (_isInflated)
        {
            speed = inflateSpeed;
        }
        else
        {
            speed = deflateSpeed;
        }

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            speed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerLight"))
        {
            return;
        }

        _isInflated = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerLight"))
        {
            return;
        }

        _isInflated = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerAirSupply airSupply =
            collision.gameObject.GetComponentInParent<PlayerAirSupply>();

        if (airSupply == null)
        {
            return;
        }

        float damage;

        if (_isInflated)
        {
            damage = inflatedDamage;
        }
        else
        {
            damage = normalDamage;
        }

        airSupply.UseAirSupply(damage);

        Debug.Log("Pufferfish hit player, oxygen reduced by: " + damage);
    }
}