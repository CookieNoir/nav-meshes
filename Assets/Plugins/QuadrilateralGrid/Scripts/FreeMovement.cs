using UnityEngine;

public class FreeMovement : MonoBehaviour
{
    public float speed = 1f;
    public float sensitivity = 1f;
    private float h = 0f;
    private float v = 0f;
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) gameObject.transform.position += transform.forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) gameObject.transform.position += -transform.forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) gameObject.transform.position += -transform.right * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) gameObject.transform.position += transform.right * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse1))
        {
            v -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            v = Mathf.Clamp(v, -90f, 90f);
            h += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            transform.eulerAngles = new Vector3(v, h, 0f);
        }
    }
}