using UnityEngine;

public class MiniMapCameraFollow : MonoBehaviour
{
    public Transform player;
    public float height = 60f;

    void LateUpdate()
    {
        if (player == null)
            return;

        transform.position = new Vector3(player.position.x, height, player.position.z);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}