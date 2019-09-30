using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float DEBUG_rotate = 0.0f;
    public float rot_x { get; private set; } = 0.0f;
    public float rot_y { get; private set; } = 0.0f;

    Transform leftUpperArm, rightUpperArm;
    private CharacterController characterController;
    private Animator animator;
    private Gun gun;
    private Vector3 velocity = Vector3.zero;
    private Vector3 accel = Vector3.zero;
    private IEnumerator aimingCoroutine;

    private float speed = 15.0f;
    private float jumpSpeed = 15.0f;
    private float gravity = -20.0f;
    private bool running = false;
    private bool aiming = true;

    enum LMBState { PRESSED, NOT_PRESSED };
    private LMBState currLmbState = LMBState.PRESSED;
    private LMBState prevLmbState = LMBState.NOT_PRESSED;

    const string LAYER_AIM = "Aiming";
    const string LEFT_UPPER_ARM_PATH = "Player/Armature/MiddleBack/UpperBack/LeftUpperArm";
    const string RIGHT_UPPER_ARM_PATH = "Player/Armature/MiddleBack/UpperBack/RightUpperArm";

    const float aimingTransitionTime = 0.5f;
    const float aimingSteps = 10;

    Vector3 trans_x_axis;

    void Awake()
    {
        animator = GetComponent<Animator>();
        leftUpperArm = GameObject.Find(LEFT_UPPER_ARM_PATH).transform;
        if (!leftUpperArm) Debug.Log("Could not get a reference to leftUpperArm");
        rightUpperArm = GameObject.Find(RIGHT_UPPER_ARM_PATH).transform;
        if (!rightUpperArm) Debug.Log("Could not get a reference to rightUpperArm");
    }

    // Start is called before the first frame update
    void Start()
    {
        animator.SetLayerWeight(animator.GetLayerIndex(LAYER_AIM), 1.0f);
        characterController = GetComponent<CharacterController>();
        accel.y = gravity;
    }

    void Update()
    {
        if (Input.GetMouseButton(1)) currLmbState = LMBState.PRESSED;
        else currLmbState = LMBState.NOT_PRESSED;

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

    void LateUpdate()
    {
        if(gun && gun.Equipped && aiming) {
            rightUpperArm.RotateAround(rightUpperArm.position, trans_x_axis, rot_x);
            leftUpperArm.RotateAround(leftUpperArm.position, trans_x_axis, rot_x);
            // rightUpperArm.rotation = Quaternion.AngleAxis(DEBUG_rotate, trans_x_axis) * rightUpperArm.rotation;
            //rightUpperArm.rotation = Quaternion.Euler(45, 0, 0);
        }
    }

    public void UpdatePlayerMovement()
    {
        running = false;
        rot_y += GameInput.instance.mouseRotationX * GameInput.instance.playerRotationSpeed * Time.deltaTime;
        rot_x += GameInput.instance.mouseRotationY * GameInput.instance.playerRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, rot_y, 0);
        trans_x_axis = transform.rotation * new Vector3(1, 0, 0);

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

            /*
            Vector3 bulletPush = collisionProperties.bulletDirection * collisionProperties.kickBack;
            if (characterController.isGrounded) bulletPush *= 8.0f;
            velocity += bulletPush;
            */
        }
        else if(hit.tag == PrefabManager.instance.m16.tag)
        {
            GameObject gunObject = hit.gameObject;
            gun = gunObject.GetComponent<Gun>();
            if(!gun.Equipped)
            {
                gun.Equipped = true;
                gunObject.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
