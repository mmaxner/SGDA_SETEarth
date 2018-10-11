using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        screenHeightInUnits = Camera.main.orthographicSize * 2;
        screenWidthInUnits = screenHeightInUnits * Screen.width / Screen.height;
    }

    public const float original_size = 20;
    public const float min_size = 1;
    public const float max_size = 42;

    private float screenHeightInUnits;
    private float screenWidthInUnits;

    private float world_width = 257 * StaticData.size_increment;
    private float world_height = 129 * StaticData.size_increment;

    private const float zoom_speed = 0.25f;
    private const float move_speed = 0.25f;
    private Camera cam;

	// Update is called once per frame
	void FixedUpdate () {
        float aspect = (float)Screen.width / (float)Screen.height;
        float moveHorizontal = Input.GetAxis("Horizontal") * move_speed * ((cam.orthographicSize + 5) / original_size);
        float moveVertical = Input.GetAxis("Vertical") * move_speed * ((cam.orthographicSize + 5) / original_size);

        if (transform.position.x + moveHorizontal - cam.orthographicSize * aspect > (-1 * 0.5f * world_width) &&
            transform.position.x + moveHorizontal + cam.orthographicSize * aspect < (0.5f * world_width))
        {
            transform.Translate(new Vector3(moveHorizontal, 0, 0));
        }

        if (transform.position.y + moveVertical - cam.orthographicSize > -0.5f * world_height &&
            transform.position.y + moveVertical + cam.orthographicSize < 0.5f * world_height)
        {
            transform.Translate(new Vector3(0, moveVertical), 0);
        }
        if (Input.GetKey(KeyCode.Space) && cam.orthographicSize < max_size)
        {
            cam.orthographicSize += zoom_speed * (cam.orthographicSize / original_size);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && cam.orthographicSize > min_size)
        {
            cam.orthographicSize -= zoom_speed * (cam.orthographicSize / original_size);
        }
    }

    public void SizeTo(int width, int height)
    {
        transform.position = new Vector3(0,-0.5f * StaticData.size_increment,transform.position.z);
        cam.orthographicSize = (height * StaticData.size_increment) / 2;
    }
}
