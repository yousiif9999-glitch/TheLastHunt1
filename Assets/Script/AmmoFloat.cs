using UnityEngine;

public class AmmoFloat : MonoBehaviour
{
    public float hoverHeight = 0.5f;   // كم فوق الأرض
    public float floatAmount = 0.15f;  // الاهتزاز
    public float floatSpeed = 2f;
    public float rotateSpeed = 60f;

    private float groundY;

    void Start()
    {
        // يحدد مستوى الأرض تحت الأمو
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
        {
            groundY = hit.point.y;
        }
        else
        {
            groundY = transform.position.y;
        }
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;

        transform.position = new Vector3(
            transform.position.x,
            groundY + hoverHeight + yOffset,
            transform.position.z
        );

        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}