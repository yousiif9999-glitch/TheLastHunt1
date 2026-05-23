using UnityEngine;

public class MiniMapArrow : MonoBehaviour
{
    public Transform player;
    public float rotationOffset = 0f;

    void LateUpdate()
    {
        if (player == null)
            return;

        transform.localRotation = Quaternion.Euler(0f, 0f, -player.eulerAngles.y + rotationOffset);
    }
}