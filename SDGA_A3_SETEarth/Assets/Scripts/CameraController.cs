using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        RectTransform leftRect = leftOverlay.GetComponent<RectTransform>();
        var a = Camera.main.ScreenToWorldPoint(leftRect.rect.min);
        var b = Camera.main.ScreenToWorldPoint(leftRect.rect.max);
        UIWidth = (int)(b.x - a.x);
        RectTransform topRect = topOverlay.GetComponent<RectTransform>();
        var c = Camera.main.ScreenToWorldPoint(topRect.rect.min);
        var d = Camera.main.ScreenToWorldPoint(topRect.rect.max);
        UIHeight = (int)(d.y - c.y);
    }

    public  float original_size = 21;
    public const float min_size = 1;
    public float max_size = 42;

    public GameObject leftOverlay;
    public GameObject topOverlay;

    public int UIWidth = 125;
    public int UIHeight = 75;

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

        if (moveHorizontal < 0)
        {
            transform.Translate(new Vector3(
               -1 * Mathf.Min(
                    Mathf.Abs(((-0.5f * world_width) - (transform.position.x + StaticData.size_increment/2.0f - cam.orthographicSize * aspect))),
                    Mathf.Abs(moveHorizontal)), 0, 0));
        }
        else if (moveHorizontal > 0)
        {
            transform.Translate(new Vector3(
                Mathf.Min(
                    Mathf.Abs(((0.5f * world_width) - (transform.position.x + StaticData.size_increment/2.0f + cam.orthographicSize * aspect))),
                    Mathf.Abs(moveHorizontal)), 0, 0));
        }

        /*if ((moveHorizontal < 0 && (transform.position.x + moveHorizontal - cam.orthographicSize * aspect) + (-1 * 0.5f * world_width)) ||
            (moveHorizontal > 0 && transform.position.x + moveHorizontal + cam.orthographicSize * aspect < (0.5f * world_width)))
        {
            transform.Translate(new Vector3(moveHorizontal, 0, 0));
        }*/

        if (moveVertical < 0)
        {
            transform.Translate(new Vector3(0,
               -1 * Mathf.Min(
                    Mathf.Abs(((-0.5f * world_height) - (transform.position.y + StaticData.size_increment / 2.0f - cam.orthographicSize))),
                    Mathf.Abs(moveVertical)), 0));
        }
        else if (moveVertical > 0)
        {
            transform.Translate(new Vector3(0,
                Mathf.Min(
                    Mathf.Abs(((0.5f * world_height) - (transform.position.y + StaticData.size_increment / 2.0f + cam.orthographicSize))),
                    Mathf.Abs(moveVertical)), 0));
        }

        /*if ((moveVertical < 0 && transform.position.y + moveVertical - cam.orthographicSize > -0.5f * world_height) ||
            (moveVertical > 0 && transform.position.y + moveVertical + cam.orthographicSize < 0.5f * world_height))
        {
            transform.Translate(new Vector3(0, moveVertical), 0);
        }*/

        RecalculateMaxSize();

        if (Input.GetKey(KeyCode.Space))
        {
            cam.orthographicSize += Mathf.Min(zoom_speed * (cam.orthographicSize / original_size),
                max_size - cam.orthographicSize);
            //cam.orthographicSize += zoom_speed * (cam.orthographicSize / original_size);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            cam.orthographicSize -= Mathf.Min(Mathf.Abs(zoom_speed * (cam.orthographicSize / original_size)),
                Mathf.Abs(min_size - cam.orthographicSize));
        }
    }

    public void SizeTo(int width, int height)
    {
        world_width = (width + 1) * StaticData.size_increment + UIWidth;
        world_height = (height + 1) * StaticData.size_increment + UIHeight;
        transform.position = new Vector3(0,-0.5f * StaticData.size_increment,transform.position.z);
        cam.orthographicSize = (height * StaticData.size_increment) / 2;
        original_size = cam.orthographicSize;
        RecalculateMaxSize();
    }

    private void RecalculateMaxSize()
    {
        float left_buffer = Mathf.Abs(0.5f * world_width - transform.position.x);
        float right_buffer = Mathf.Abs(transform.position.x + 0.5f * world_width);
        float top_buffer = Mathf.Abs(0.5f * world_height - transform.position.y);
        float bottom_buffer = Mathf.Abs(transform.position.y + 0.5f * world_height);


        float horizontal_buffer = Mathf.Min(left_buffer, right_buffer);
        float vertical_buffer = Mathf.Min(top_buffer, bottom_buffer);

        float horizontal_max_size = horizontal_buffer * (float)Screen.height / (float)Screen.width;
        float vertical_max_size = vertical_buffer;

        float min_max_size = Mathf.Min(horizontal_max_size, vertical_max_size);
        max_size = min_max_size;

       /* if (cam.orthographicSize > max_size)
        {
            cam.orthographicSize = max_size;

            if (horizontal_max_size < vertical_max_size)
            {
                transform.Translate(new Vector3)
            }
            else
            {

            }
        }*/
    }
}
