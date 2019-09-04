using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    private GameObject gun;
    private Vector3 velocity = Vector3.zero;
    private Vector3 accel = Vector3.zero;

    private float speed = 15.0f;
    private float jumpSpeed = 15.0f;
    private float gravity = -20.0f;
    private bool running = false;

    void Awake()
    {
        gun = GameObject.Find("Player/Pistol");
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        accel.y = gravity;
    }

    public void UpdatePlayerMovement()
    {
        running = false;
        transform.Rotate(0, GameInput.instance.mouseRotationX * GameInput.instance.playerRotationSpeed * Time.deltaTime, 0);
        //else if (aim_rot_y.eulerAngles.y < -85.0f) aim_rot_y = Quaternion.Euler(-85.0f, 0, 0);

        if (characterController.isGrounded) {
            if (characterController.isGrounded) velocity = Vector3.zero;
            if (Input.GetKey(KeyCode.Space)) velocity.y = jumpSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            velocity.z = speed;
            if (characterController.isGrounded) running = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity.x = -speed;
            if (characterController.isGrounded) running = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity.z = -speed;
            if (characterController.isGrounded) running = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity.x = speed;
            if (characterController.isGrounded) running = true;
        }
      
        velocity += accel * Time.deltaTime;
        characterController.Move(transform.TransformDirection(velocity * Time.deltaTime));
        animator.SetBool("running", running);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.transform.tag == PrefabManager.instance.bullet.tag)
        {
            Debug.Log("hit by bullet");
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == PrefabManager.instance.bullet.tag)
        {
            Bullet.CollisionProperties collisionProperties = hit.gameObject.GetComponent<Bullet>().GetCollisionProperties();
            characterController.Move(collisionProperties.bulletDirection * collisionProperties.kickBack);

            /*
            Vector3 bulletPush = collisionProperties.bulletDirection * collisionProperties.kickBack;
            if (characterController.isGrounded) bulletPush *= 8.0f;
            velocity += bulletPush;
            */
        }
    }
}
