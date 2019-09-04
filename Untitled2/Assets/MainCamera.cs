using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    GameObject player;
    Transform playerTransformRelToLocalOrigin;
    Player playerScript;
    Vector3 cameraPosOffset = new Vector3(0, 2.0f, -2.0f);
    Vector3 aimAtOffset = new Vector3(0.25f, 0, 1);
    Quaternion pos_rot_x = Quaternion.identity;
    float fov = 60.0f;
    float offset_angle;
    float rot_x;

    void Awake()
    {
        player = GameObject.Find("Player");
        if (!player) Debug.Log("Could not get a reference to Player");
        else playerScript = player.GetComponent<Player>();
        if (!playerScript) Debug.Log("Could not get a reference to the Player's script");
        playerTransformRelToLocalOrigin = player.transform.Find("01");
        if (!playerTransformRelToLocalOrigin) Debug.Log("Could not get a reference to Player's local origin");

        Vector3 projection = Vector3.ProjectOnPlane(cameraPosOffset, new Vector3(0, -1, 0));
        offset_angle = Mathf.Acos(Vector3.Dot(projection.normalized, cameraPosOffset.normalized)) * Mathf.Rad2Deg;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateCameraMovement()
    {
        float temp_rot_x = rot_x + -GameInput.instance.mouseRotationY * GameInput.instance.playerRotationSpeed * Time.deltaTime;
        if (!(temp_rot_x + offset_angle > fov || temp_rot_x + offset_angle < -fov)) rot_x = temp_rot_x;
        //rot_x += -GameInput.instance.mouseRotationY * GameInput.instance.playerRotationSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(rot_x, player.transform.eulerAngles.y, 0);
        Vector3 aimAt = playerTransformRelToLocalOrigin.position + playerTransformRelToLocalOrigin.TransformDirection(aimAtOffset);
        transform.position = aimAt + (rotation * cameraPosOffset);
        transform.LookAt(aimAt);
    }
}
