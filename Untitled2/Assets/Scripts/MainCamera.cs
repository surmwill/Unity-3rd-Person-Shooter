using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public const string CAMERA_PATH = "MainCamera";
    public const float MAX_X_ROT = 60.0f;

    const string PLAYER_TRANSFORM_PATH = "Player/01";

    GameObject player;
    Transform playerTransform;

    Vector3 cameraPosOffset = new Vector3(0, 0.0f, -2.0f);  // camera has this offset from the player model
    Vector3 lookAtOffset = new Vector3(0.25f, 0, 1);    // don't look exactly at the player, but close

    float offset_angle, rot_x = 0.0f;

    void Awake()
    {
        player = Utils.FindGameObject(Player.PLAYER_PATH, ToString());

        playerTransform = Utils.FindGameObject(PLAYER_TRANSFORM_PATH, ToString()).transform;
        if (!playerTransform) Debug.Log("Could not get a reference to Player's local origin");

        /*
         * Before we adjust the player's orientation, calculate the camera's initial elevation from the xz (ground) plane
         * Assume the camera's position is not off-center (a zero in the x-component) .
         */ 
        Vector3 projection = Vector3.ProjectOnPlane(cameraPosOffset, new Vector3(0, -1, 0));
        offset_angle = Mathf.Acos(Vector3.Dot(projection.normalized, cameraPosOffset.normalized)) * Mathf.Rad2Deg;
    }

    // Called by GameInput's Update()
    public void UpdateCameraMovement()
    {
        /*
         * Ensure the camera's positional rotation around the x-axis falls into a certain range. For example,
         * we would not want the camera to make full circles around the player if we continually looking up  
         */
        float temp_rot_x = rot_x + -GameInput.instance.mouseRotationY * GameInput.instance.playerRotationSpeed * Time.deltaTime;
        if (!(temp_rot_x + offset_angle > MAX_X_ROT || temp_rot_x + offset_angle < -MAX_X_ROT)) rot_x = temp_rot_x;

        /*
         * The idea is we start with the camera at its initial offset from the player (up and looking down
         * at the player for example). Then we rotate the camera's position around the world's x and y axis
         * according to where we move-the-mouse/look in the game. Then we add translate according to the player's
         * position (in this case a little offsetted). Then we adjust the camera's viewing direction to look
         * at the player.
         */
        Quaternion rotation = Quaternion.Euler(rot_x, player.transform.eulerAngles.y, 0);
        Vector3 lookAt = playerTransform.position + playerTransform.TransformDirection(lookAtOffset);
        transform.position = lookAt + (rotation * cameraPosOffset);
        transform.LookAt(lookAt);
    }
}
