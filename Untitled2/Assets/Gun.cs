using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    // constants
    const string PLAYER_GRIP_HAND_PATH = "Player/Armature/MiddleBack/UpperBack/LeftUpperArm/LeftLowerArm/LeftWrist";
    const string BULLET_POOL_NAME = "bullet pool name";
    const int BULLET_POOL_SIZE = 5;

    // Shooting properties
    float bulletSpeed, bulletSpread, bulletKickback;

    // Creation and firing of bullets
    int layerMask;
    Camera mainCamera;
    Vector3 barrelLocalOffset;  // bullets should come out of here instead of the center of the gun
    ObjectPool bulletPool;

    // Calculating gun's position relative to player
    Player player;
    Transform playerGripHand;   // gun won't perfectly align with player's hand
    Vector3 gripLocalOffset;

    void Awake()
    {
        layerMask = LayerMask.GetMask(Player.PLAYER_LAYER);

        mainCamera = Utils.FindGameObject(MainCamera.CAMERA_PATH, ToString()).GetComponent<Camera>();
        playerGripHand = Utils.FindGameObject(PLAYER_GRIP_HAND_PATH, ToString()).transform;
        player = Utils.FindGameObject(Player.PLAYER_PATH, ToString()).GetComponent<Player>();

        if (!mainCamera) Debug.Log("Could not get a reference to MainCamera");
        if (!playerGripHand) Debug.Log("Could not get a reference to the player's gripping hand");
        if (!player) Debug.Log("Could not get a reference to player");
    }

    virtual protected void Start()
    {
        bulletPool = ScriptableObject.CreateInstance<ObjectPool>();
        bulletPool.Init(BULLET_POOL_NAME, PrefabManager.instance.bullet, BULLET_POOL_SIZE);
    }

    public void InitGun(Vector3 gripLocalOffset, Vector3 barrelLocalOffset, float bulletSpeed, float bulletSpread, float bulletKickback)
    {
        this.bulletSpeed = bulletSpeed;
        this.bulletSpread = bulletSpread;
        this.bulletKickback = bulletKickback;

        this.barrelLocalOffset = barrelLocalOffset; // bullets should come out of here instead of the center of the gun
        this.gripLocalOffset = gripLocalOffset; // gun won't perfectly align with player's hand
    }

    // Called by Player.LateUpdate() to match the player's animated position changes
    public void UpdatePosition()
    {
        transform.position = playerGripHand.transform.position + playerGripHand.TransformVector(gripLocalOffset);
        transform.rotation = Quaternion.Euler(-player.rot_x, player.rot_y + 180.0f, 0);
    }

    public void Shoot()
    {
        Vector3 bulletSpawn = transform.position + transform.TransformVector(barrelLocalOffset);
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100.0f, ~layerMask))
        {
            Vector3 direction = (hit.point - bulletSpawn).normalized;
            GameObject bullet = bulletPool.Fetch(bulletSpawn, Quaternion.identity);
            if(bullet) bullet.GetComponent<Bullet>().InitBullet(direction, bulletKickback);
        }
        else
        {
            // error
        }
    }
}
