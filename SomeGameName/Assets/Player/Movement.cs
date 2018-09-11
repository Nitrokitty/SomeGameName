using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Movement : NetworkBehaviour
{

    CharacterController characterController;
    BoxCollider collider;
    public float horizontalSpeed = 1f;
    public float forwardSpeed = 1f;
    public float rotationSpeed = 1f;
    public float gravity = 2f;
    public float jumpForce = 5f;
    public float airTime = 2f;
    bool isJumping = false;
    float elapsedJumpTime = 0f;
    Vector3 forward;
    Vector3 right;
    Vector3 gravityVec;
    Teams team;
    //<<<<<<< HEAD
    //    Vector3 up;
    public Vector3 startPosition;
    TeamInventory teamInventory;

//=======

    public bool IsLocalPlayer
    {
        get { return isLocalPlayer; }
    }
    
    //Enable or disable mouse movement
    public bool allowMouseX = true;
    
    // Use this for initialization
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gravityVec = new Vector3(0, -gravity, 0);
        GameObject assignedBase;
        team = Manager.AssignPlayerToTeam(this.gameObject, out startPosition, out assignedBase);
        GetComponent<Inventory>().teamInventory = assignedBase.transform.Find("chest").Find("ChestRadius").GetComponent<TeamInventory>();
        GetComponent<Inventory>().forge = assignedBase.transform.Find("Anvil").transform.position;
       transform.position = startPosition;
        collider = GetComponent<BoxCollider>();
        teamInventory = gameObject.GetComponent<Inventory>().teamInventory;
    }

    // Update is called once per frame
    void Update()
    {
        //var direction = transform.TransformDirection(Vector3.forward + Vector3.right);
        //characterController.Move(new Vector3(direction.x * horizontalSpeed * Time.deltaTime * Input.GetAxis("Horizontal"), , direction.z * forwardSpeed * Time.deltaTime * Input.GetAxis("Vertical")));

        if (!isLocalPlayer || teamInventory.showInventory)
            return;

        var forw = Input.GetAxis("Vertical");
        var side = Input.GetAxis("Horizontal");
//<<<<<<< HEAD
//        var jump = Input.GetKeyDown("space");


//        if (Input.GetAxis("Rotate") != 0)
//            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime * Input.GetAxis("Rotate"), 0));
//=======
        var rot = Input.GetAxis("Rotate");
        var mouseX = Input.GetAxis("Mouse X");


        if (allowMouseX && mouseX != 0)
        {
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime * mouseX, 0));
        }

        if (rot != 0)
        {
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime * rot, 0));
        }

        //if (isJumping)
        //{
        //    elapsedJumpTime += Time.deltaTime;
        //    var jumpHeight = GetJumpHeight(elapsedJumpTime);
        //    up = new Vector3(0, jumpHeight, 0);
        //    if (elapsedJumpTime >= airTime)
        //    {
        //        isJumping = false;
        //        elapsedJumpTime = 0;
        //    }
        //    characterController.SimpleMove(up);
        //    return;
        //}

        if (!characterController.isGrounded)
        {
            characterController.SimpleMove(gravityVec);
            return;
        }

        if (forw != 0)
        {
            var direction = transform.TransformDirection(Vector3.forward);
            direction = direction * forwardSpeed;
            RaycastHit hit;
            var ray = new Ray(characterController.transform.position, transform.TransformDirection(Vector3.down));
            //Debug.Log(characterController.Raycast(ray, out hit, 10000000));

            if (characterController.Raycast(ray, out hit, 10000000))//if (characterController.isGrounded && collider.Raycast(ray, out hit, 100) && hit.transform.gameObject.tag == "Terrain")
            {
                var slope = hit.normal;
                if (slope.y > 45)
                    forward = forward * ((60 - slope.y) / 15f);
                //Adjust character based on normal
            }
            forward = new Vector3(direction.x * Time.deltaTime * forw, 0, direction.z * Time.deltaTime * forw);
        }
        else
            forward = Vector3.zero;

        if (side != 0)
        {
            var direction = transform.TransformDirection(Vector3.right);
            direction = direction * forwardSpeed;
            right = new Vector3(direction.x * Time.deltaTime * side, 0, direction.z * Time.deltaTime * side);
        }
        else
            right = Vector3.zero;



        //if (jump)
        //{
        //    //var direction = transform.TransformDirection(Vector3.up);
        //    //direction = direction * jumpForce;
        //    //up = new Vector3(0, direction.y * Time.deltaTime, 0);
        //    isJumping = true;
        //    //Debug.Log(direction);
        //}
        //else
        //    up = Vector3.zero;

        //if (jump)
        //    characterController.Move(forward + right + up);
        //else
        characterController.Move(forward + right + gravityVec);

    }

    float GetJumpHeight(float elapsedTime)
    {
        //y = -(x - h)2
        return -Mathf.Pow((elapsedTime - airTime * 2), 2) * jumpForce;
    }
}