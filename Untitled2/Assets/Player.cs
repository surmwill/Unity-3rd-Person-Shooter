using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController characterController;
    private GameObject gun;
    private Vector3 velocity = Vector3.zero;
    private Vector3 accel = Vector3.zero;

    private float speed = 15.0f;
    private float jumpSpeed = 25.0f;
    private float gravity = -20.0f;

    void Awake()
    {
        gun = GameObject.Find("Player/Pistol");
    }

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        accel.y = gravity;
    }

    public void UpdatePlayerMovement()
    {
        transform.Rotate(0, GameInput.instance.mouseRotationX * GameInput.instance.playerRotationSpeed * Time.deltaTime, 0);
        if(gun) gun.transform.Rotate(GameInput.instance.mouseRotationY * GameInput.instance.playerRotationSpeed * Time.deltaTime, 0, 0);

        if (characterController.isGrounded)
        {
            velocity = Vector3.zero;
            if (Input.GetKey(KeyCode.Space)) velocity.y = jumpSpeed;
        }
       
        if (Input.GetKey(KeyCode.W)) velocity.z = speed;
        if (Input.GetKey(KeyCode.A)) velocity.x = -speed;
        if (Input.GetKey(KeyCode.S)) velocity.z = -speed;
        if (Input.GetKey(KeyCode.D)) velocity.x = speed;
      
        velocity += accel * Time.deltaTime;
        characterController.Move(transform.TransformDirection(velocity * Time.deltaTime));
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == PrefabManager.instance.bullet.tag) {
            Debug.Log(other.gameObject.name);
        }
   
    }
}
