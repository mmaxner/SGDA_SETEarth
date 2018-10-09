using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
	}

    private const float speed = 0.25f;
    private Camera cam;

	// Update is called once per frame
	void FixedUpdate () {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(moveHorizontal * speed, moveVertical * speed), 0);

        if (Input.GetKey(KeyCode.Space))
        {
            cam.orthographicSize += speed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            cam.orthographicSize -= speed;
        }
    }
}
