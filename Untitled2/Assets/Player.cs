using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // public constants
    public const string PLAYER_PATH = "Player";
    public const string PLAYER_LAYER = "Player";

    // constants
    const string LAYER_AIM = "Aiming";
    const string LEFT_UPPER_ARM_PATH = "Player/Armature/MiddleBack/UpperBack/LeftUpperArm";
    const string RIGHT_UPPER_ARM_PATH = "Player/Armature/MiddleBack/UpperBack/RightUpperArm";

    // movement objects
    Transform leftUpperArm, rightUpperArm;
    Vector3 trans_x_axis;   // used to rotate the arms (at the shoulder) when adjusting the aiming angle
    CharacterController characterController;
    Animator animator;
    Vector3 velocity = Vector3.zero;
    Vector3 accel = Vector3.zero;

    // movement properties/values
    public float rot_x { get; private set; } = 0.0f;
    public float rot_y { get; private set; } = 0.0f;
    float speed = 15.0f;
    float jumpSpeed = 15.0f;
    float gravity = -20.0f;
    private bool running = false;

    // weapons
    Gun gun;
    private bool aiming = true;
    private bool gunEquipped = false;

    // To toggle aim
    enum LMBState { PRESSED, NOT_PRESSED };
    LMBState currLmbState = LMBState.PRESSED;
    LMBState prevLmbState = LMBState.NOT_PRESSED;

    void Awake()
    {
        leftUpperArm = Utils.FindGameObject(LEFT_UPPER_ARM_PATH, ToString()).transform;
        rightUpperArm = Utils.FindGameObject(RIGHT_UPPER_ARM_PATH, ToString()).transform;

        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        if (!animator) Debug.Log("Could not get a reference to the Animator component");
        if (!characterController) Debug.Log("Could not get a reference to the CharacterController component");

        accel.y = gravity;
    }

    void Start()
    {
        animator.SetLayerWeight(animator.GetLayerIndex(LAYER_AIM), 1.0f);
    }

    void Update()
    {
        updateAimingState();
    }

    void updateAimingState()
    {
        if (Input.GetMouseButton(1)) currLmbState = LMBState.PRESSED;
        else currLmbState = LMBState.NOT_PRESSED;

        // toggle aim when the LMB is clicked & let go
        if (currLmbState == LMBState.PRESSED && prevLmbState == LMBState.NOT_PRESSED)
        {
            animator.SetLayerWeight(animator.GetLayerIndex(LAYER_AIM), 1.0f);
            aiming = true;
        }
        else if (currLmbState == LMBState.NOT_PRESSED && prevLmbState == LMBState.PRESSED)
        {
            prevLmbState = LMBState.NOT_PRESSED;
            animator.SetLayerWeight(animator.GetLayerIndex(LAYER_AIM), 0.0f);
            aiming = false;
        }

        prevLmbState = currLmbState;
    }

    // Called by GameInput's Update() 
    public void UpdatePlayerMovement()
    {
        // determine the direction we are facing (rotation around y axis)
        rot_y += GameInput.instance.mouseRotationX * GameInput.instance.playerRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, rot_y, 0);

        // determine the angle we are looking up/down (to adjust the arms up/down while aiming)
        float temp_rot_x = rot_x + -GameInput.instance.mouseRotationY * GameInput.instance.playerRotationSpeed * Time.deltaTime;
        if (!(temp_rot_x > MainCamera.MAX_X_ROT || temp_rot_x < -MainCamera.MAX_X_ROT)) rot_x = temp_rot_x;

       //  rot_x -= GameInput.instance.mouseRotationY * GameInput.instance.playerRotationSpeed * Time.deltaTime;
        trans_x_axis = transform.rotation * new Vector3(1, 0, 0);

        Debug.Log(rot_x);

        running = false;    // default to not playing a running animation (if we are jumping or just standing still)

        if (characterController.isGrounded)
        {
            if (characterController.isGrounded) velocity = Vector3.zero;    // don't fall through the map
            if (Input.GetKey(KeyCode.Space)) velocity.y = jumpSpeed;    // we can jump if we are on the ground
        }

        // change position according to the WASD arrow keys
        if (Input.GetKey(KeyCode.W))
        {
            velocity.z = speed;
            if (characterController.isGrounded) running = true; // moving + on the ground = play run animation
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity.x = -speed;
            if (characterController.isGrounded) running = true; // moving + on the ground = play run animation
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity.z = -speed;
            if (characterController.isGrounded) running = true; // moving + on the ground = play run animation
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity.x = speed;
            if (characterController.isGrounded) running = true; // moving + on the ground = play run animation
        }

        velocity += accel * Time.deltaTime;
        characterController.Move(transform.TransformDirection(velocity * Time.deltaTime));
        animator.SetBool("running", running);
    }

    void LateUpdate()
    {
        /*
         * After the playing the aiming animation (which is more of a pose being one frame), raise
         * and lower the arms and adjust the rotation of the gun to meet the angle we are looking up/down
         */
        if(gunEquipped && aiming) {
            rightUpperArm.RotateAround(rightUpperArm.position, trans_x_axis, rot_x);
            leftUpperArm.RotateAround(leftUpperArm.position, trans_x_axis, rot_x);
            gun.UpdatePosition();

            if (Input.GetMouseButtonDown(0)) gun.Shoot();   // Shooting
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == PrefabManager.instance.groundBlock.tag) 
        {
            GroundBlock block = hit.gameObject.GetComponent<GroundBlock>();
            if(!block.grassType)
            {
                GrassGrid grassGrid = Instantiate(
                    PrefabManager.instance.grassPlatform, 
                    block.gameObject.transform.position + block.gameObject.transform.TransformVector(-0.5f, 0.5001f, -0.5f), 
                    block.transform.rotation
                    ).GetComponent<GrassGrid>();

                grassGrid.Init(
                    block.transform.localScale.x,
                    block.transform.localScale.z,
                    block.transform.InverseTransformPoint(hit.point));
                block.grassType = true;
            }
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == PrefabManager.instance.bullet.tag)
        {
            Bullet.CollisionProperties collisionProperties = hit.gameObject.GetComponent<Bullet>().GetCollisionProperties();
            characterController.Move(collisionProperties.bulletDirection * collisionProperties.kickBack);
        }
        else if(hit.tag == PrefabManager.instance.m16.tag)
        {
            if (gunEquipped) return;

            gun = hit.gameObject.GetComponent<Gun>();
            gun.gameObject.GetComponent<Collider>().enabled = false;
            gunEquipped = true;
        }


    }
}
