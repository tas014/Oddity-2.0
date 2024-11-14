using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // Paranormal Event related
    [Header("Event info")]
    public GameObject eventObject;
    public ParanormalEventHandler paranormalEventHandler;
    public float eventStartTime = 0;
    public float stress = 0;

    // Movement and vision related
    [Header("Movement and vision")]
    [SerializeField] private float moveSpeed = 7;
    [SerializeField] private float mouseSens = 1;
    [SerializeField] private Camera playerCam;
    private float verticalCamCap = 80;
    private float speedModifier = 1;
    private PlayerInputActions playerInputActions;
    private CharacterController characterController;

    // Inputs
    private float verticalRotation;


    private void Start() {
        characterController = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    private void Update() {
        HandleMovement();
        HandleRotation();
        bool seenEvent = DidSeeParanormalEvent();
        if (seenEvent && !paranormalEventHandler.isEventOnCooldown) {
            stress += 15;
            paranormalEventHandler.ToggleEventCooldown();
        }
    }

    private void HandleMovement() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            speedModifier = 1.6f;
        } else {
            speedModifier = 1;
        }
        float frontalSpeed = Input.GetAxis("Vertical") * moveSpeed * speedModifier;
        float horizontalSpeed = Input.GetAxis("Horizontal") * moveSpeed * speedModifier;

        Vector3 speed = new(horizontalSpeed, 0, frontalSpeed);
        speed = transform.rotation * speed;

        characterController.SimpleMove(speed);
    }

    private void HandleRotation() {
        float mouseXRotation = Input.GetAxis("Mouse X") * mouseSens;
        float mouseYRotation = Input.GetAxis("Mouse Y") * mouseSens;

        transform.Rotate(0, mouseXRotation, 0);
        verticalRotation -= mouseYRotation;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalCamCap, verticalCamCap);
        playerCam.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private bool DidSeeParanormalEvent() {
        // If 20 seconds haven't passed since the event took place...
        if (Time.time >= 20 && Time.time - eventStartTime <= 20) {
            // Check if camera can see assigned object (eventObject)
            bool isInCamView = false;
            Vector3 viewPos = playerCam.WorldToViewportPoint(eventObject.transform.position);
            if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0) {
                // Object is in cam range!
                isInCamView = true;
            }
            // Assign ray origin to cam position (raise height by 2.4);
            Vector3 rayOrigin = new(transform.position.x, transform.position.y + 2.4f, transform.position.z);

            // And check what the lineCast hits (necessary bc our target objects needs its collider active since the paranormal event will require it to interact physically)
            Physics.Linecast(rayOrigin, eventObject.transform.position, out RaycastHit hit);

            // If the game object hit by the linecast is our target object...
            bool seesEventObject = hit.collider.gameObject.name == eventObject.name;

            // We can now check wether object is within FOV and can be seen directly!
            if (isInCamView && seesEventObject) {
                Debug.Log("Event is being seen by the player!");
                return true;
            }
            Debug.Log("Event timer working, however the player cannot see the event");
        } else {
            Debug.Log("Event has happened over 20 seconds ago");
        }
        return false;
    }
}
