using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigMovement : MonoBehaviour
{
    //Move the Camera Rig to the Player if it exists, otherwise find a good spot and chill there for a while
    public Vector3 cameraOffset;
    public bool useMouse = true;
    public bool invertMouseY = true;
    public float rotationSpeed = 50.0f;
    
    //If Camera is Attached To Player
    private bool isAttached = false;
    public bool keepPositionAfterDetach = false;
    
    //Default Location of the Camera
    public Quaternion defPosRotation;
    public Vector3 defPosPosition;
    private GameObject camera;
    
    //Debugging Attachment
    public bool forceDetach = false;
    
    //Range for Camera
    public float rotationLimit = 90.0f;
    
    // Use this for initialization
    void Start()
    {
        defPosRotation = gameObject.transform.rotation;
        defPosPosition = gameObject.transform.position;
        camera = gameObject.transform.Find("Main Camera").gameObject;
        if (camera == null) print("Couldn't find Camera");
        //camera.transform.position = cameraOffset;
        //camera.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        doAttachment();
        doMovement();
        //print("Angle: " + transform.eulerAngles);
    }
    //Call this if camera is repositioned
    void doAttachment()
    {
        if (isAttached) {
          if (gameObject.transform.parent == null || forceDetach) {
            //print ("Detaching Camera");
            if (!keepPositionAfterDetach) resetPosition();
            isAttached = false;
            //TODO Remove This
            //Debug.Break();
            
          }
        } else {
          GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
          if (players == null || forceDetach) return;
          foreach(GameObject g in players) {
            Movement mov = g.GetComponent(typeof(Movement)) as Movement;
            if (mov.isLocalPlayer) {
              //print("Found a Local Player!");
              isAttached = true;
              //Set the position and rotation to zero
              //print("[Before Parent Set]Pos, Rot: " + gameObject.transform.position + " " + gameObject.transform.rotation);
              gameObject.transform.SetParent(g.transform);
              gameObject.transform.position = g.transform.position;
              gameObject.transform.rotation = g.transform.rotation;//Quaternion.identity
              camera.transform.position = gameObject.transform.position + cameraOffset;
              camera.transform.rotation = Quaternion.identity;
              //print("[After Parent Set]Pos, Rot: " + gameObject.transform.position + " " + gameObject.transform.rotation);
              //TODO 
              //Debug.Break();
              //camera.GetComponent<BGMController>().PlayBGM();//Play BGM When player is found
            }
          }
        }
    }
    
    //Reset the Position of the Camera to Default Position
    void resetPosition() {
      gameObject.transform.SetParent(null);
      gameObject.transform.position = defPosPosition;
      gameObject.transform.rotation = defPosRotation;
    }
    
    //Move the Camera
    void doMovement() {
        if (!isAttached) return;
        var deltaY = 0.0f;
        var deltaX = 0.0f;
        if (useMouse) {
            deltaY = Input.GetAxis("Mouse Y");
            deltaX = Input.GetAxis("Mouse X");
        } else {
            deltaX = Input.GetAxis("Camera X");
            deltaY = Input.GetAxis("Camera Y");
        }
        float invertY = 1.0f;
        if (invertMouseY) invertY = -1.0f;
        float rotX = rotationSpeed * Time.deltaTime * deltaX;
        float rotY = rotationSpeed * Time.deltaTime * deltaY * invertY;
        //Vector3 rot = new Vector3(rotY, 0, 0);
        Vector3 rot = transform.eulerAngles;
        rot.x = Mathf.Clamp((rot.x + rotY), 0, rotationLimit);
        //rot.x += rotY;
        transform.eulerAngles = rot;
        //Clamp the Rotation value
        
    }
    
    public void DetachFromPlayer() {
      gameObject.transform.parent = null;
      //camera.GetComponent<BGMController>().PlayInter();
    }
}