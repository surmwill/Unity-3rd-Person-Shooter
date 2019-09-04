using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput instance;

    public float playerRotationSpeed = 300.0f;
    public float mouseRotationX, mouseRotationY;
    Player player;
    MainCamera mainCamera;

    void Awake()
    {
        instance = this;

        player = GameObject.Find("Player").GetComponent<Player>();
        if (!player) Debug.Log("Could not get reference to Player componenet");
        mainCamera = GameObject.Find("MainCamera").GetComponent<MainCamera>();
        if (!mainCamera) Debug.Log("Could not get reference to MainCamera componenet");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mouseRotationX = Input.GetAxis("Mouse X");
        mouseRotationY = Input.GetAxis("Mouse Y");

        player.UpdatePlayerMovement();
        mainCamera.UpdateCameraMovement();
    }
}
