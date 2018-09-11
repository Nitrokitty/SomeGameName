using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Vector3 cameraOffset = new Vector3(0, 2, -10);
    public bool useMouse = true;
    public bool invertMouseY = true;
    public float rotationSpeed = 50.0f;
    TeamInventory teamInventory;
    //For Inverting Mouse Movement
    private float mouseYInvert = -1.0f;

    private float radius = 0.0f;
    private float deg = 0.0f;

    GameObject player;
    // Use this for initialization
    void Start()
    {
        player = transform.parent.gameObject;
        
        transform.position = player.transform.position + cameraOffset;
        //transform.position = player.transform.position + cameraOffset;
        updatePositioning();

    }

    // Update is called once per frame
    void Update()
    {
       
        var mouseY = Input.GetAxis("Mouse Y");
        float rot = rotationSpeed * Time.deltaTime * mouseY * mouseYInvert;
        if (useMouse && mouseY != 0)
        {
            //if (invertMouseY) mouseYInvert = -1.0f;
            //else mouseYInvert = 1.0f;
            //transform.Rotate(new Vector3(rot, 0, 0));
            //Move Camera up and down.
            //transform.Translate(new Vector3(Mathf.Cos(rotationSpeed * Time.deltaTime * mouseY * mouseYInvert), Mathf.Sin(rotationSpeed * Time.deltaTime * mouseY * mouseYInvert), 0));
            //transform.position = new Vector3(0, radius * Mathf.Sin(transform.rotation.eulerAngles.x), 0) + player.transform.position + cameraOffset;
        }

        //if(Manager.DEBUG)
        //    transform.position = player.transform.position + cameraOffset;
    }

    //Call this if camera is repositioned
    void updatePositioning()
    {
        deg = Mathf.Tan(transform.position.y / transform.position.x);
        radius = Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.y, 2);
    }
}