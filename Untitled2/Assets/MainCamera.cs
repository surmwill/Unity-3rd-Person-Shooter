using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    GameObject player;
    Vector3 initOffset = new Vector3(0, 0, -10f);
    Vector3 offset;

    void Awake()
    {
        player = GameObject.Find("Player");
        if (!player) Debug.Log("Could not get a reference to Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        offset = initOffset;
    }

    public void UpdateCameraMovement()
    {
        offset = Quaternion.Euler(
            -GameInput.instance.mouseRotationY * GameInput.instance.playerRotationSpeed * Time.deltaTime,
            GameInput.instance.mouseRotationX * GameInput.instance.playerRotationSpeed * Time.deltaTime,
            0) * offset;

        transform.position = player.transform.position + offset;
        transform.LookAt(player.transform.position);
        // transform.LookAt()
    }
}
