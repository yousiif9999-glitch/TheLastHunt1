using UnityEngine;

public class CutsceneCameraFollow : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public HelicopterCrashCutscene cutscene;

    [Header("Offsets")]
    public Vector3 normalOffset = new Vector3(0f, 3f, -10f);
    public Vector3 fallOffset = new Vector3(0f, 5f, -15f);
    public Vector3 lookOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Smoothing")]
    public float moveSmooth = 4f;
    public float rotateSmooth = 4f;

    void LateUpdate()
    {
        if (target == null) return;

        float t = 0f;
        if (cutscene != null)
            t = cutscene.FallProgress;

        Vector3 desiredOffset = Vector3.Lerp(normalOffset, fallOffset, t);
        Vector3 desiredPosition = target.position + desiredOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            Time.deltaTime * moveSmooth
        );

        Vector3 lookPoint = target.position + lookOffset;
        Quaternion targetRotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotateSmooth
        );
    }
}