using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform miniMapPanel;
    [SerializeField] private RectTransform playerDot;

    [Header("World Map Bounds")]
    [SerializeField] private Vector2 worldMin;
    [SerializeField] private Vector2 worldMax;

    private void Update()
    {
        if (player == null || miniMapPanel == null || playerDot == null)
        {
            return;
        }

        UpdatePlayerDot();
    }

    private void UpdatePlayerDot()
    {
        Vector2 playerPosition = player.position;

        float normalizedX = Mathf.InverseLerp(worldMin.x, worldMax.x, playerPosition.x);
        float normalizedY = Mathf.InverseLerp(worldMin.y, worldMax.y, playerPosition.y);

        float mapWidth = miniMapPanel.rect.width;
        float mapHeight = miniMapPanel.rect.height;

        float dotX = (normalizedX - 0.5f) * mapWidth;
        float dotY = (normalizedY - 0.5f) * mapHeight;

        float halfMapWidth = mapWidth / 2f;
        float halfMapHeight = mapHeight / 2f;

        float halfDotWidth = playerDot.rect.width / 2f;
        float halfDotHeight = playerDot.rect.height / 2f;

        dotX = Mathf.Clamp(dotX, -halfMapWidth + halfDotWidth, halfMapWidth - halfDotWidth);
        dotY = Mathf.Clamp(dotY, -halfMapHeight + halfDotHeight, halfMapHeight - halfDotHeight);

        playerDot.anchoredPosition = new Vector2(dotX, dotY);
    }
}