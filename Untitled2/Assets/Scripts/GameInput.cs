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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        player = Utils.FindGameObject(Player.PLAYER_PATH, ToString()).GetComponent<Player>();
        mainCamera = Utils.FindGameObject(MainCamera.CAMERA_PATH, ToString()).GetComponent<MainCamera>();
        if (!player) Debug.Log("Could not get reference to Player componenet");
        if (!mainCamera) Debug.Log("Could not get reference to MainCamera componenet");
    }

    void Update()
    {
        mouseRotationX = Input.GetAxis("Mouse X");
        mouseRotationY = Input.GetAxis("Mouse Y");

        player.UpdatePlayerMovement();
        mainCamera.UpdateCameraMovement();
    }
}
