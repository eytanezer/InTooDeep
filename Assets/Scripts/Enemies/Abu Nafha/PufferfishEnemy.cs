using Player;
using UnityEngine;

public class PufferfishEnemy : MonoBehaviour
{
    [Header("Inflation")]
    [SerializeField] private float inflationMultiplier = 1.8f;
    [SerializeField] private float inflateSpeed = 5f;
    [SerializeField] private float deflateSpeed = 3f;

    [Header("Damage")]
    [SerializeField] private float damage = 15f;

    private static Vector3 _normalScale = Vector3.one;
    private Vector3 _inflatedScale;
    
    private Vector3 _targetScale;
    private float _scalingSpeed;
    private bool _isInflated;

    void Start()
    {
        
        _inflatedScale = _normalScale * inflationMultiplier;
    }

    private void Update()
    {
        UpdateAttributes();

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            _targetScale,
            _scalingSpeed * Time.deltaTime
        );
    }

    void UpdateAttributes()
    {
        if (_isInflated)
        {
            _targetScale = _inflatedScale;
            _scalingSpeed = inflateSpeed;
        }
        else
        {
            _targetScale = _normalScale;
            _scalingSpeed = deflateSpeed;
        }
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
        airSupply.UseAirSupply(damage);

        Debug.Log("Pufferfish hit player, oxygen reduced by: " + damage);
    }
}