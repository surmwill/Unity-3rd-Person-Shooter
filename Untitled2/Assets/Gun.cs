using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    float bulletSpeed, bulletSpread, bulletKickback;
    int layerMask;
    bool _equipped = false;
    public bool equipped { get { return _equipped; } set { _equipped = value; } }
    Camera mainCamera;
    Vector3 barrelLocalOffset, gripLocalOffset;
    Transform playerGripHand;
    ObjectPool pool;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Player");
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        if (!mainCamera) Debug.Log("Could not get a reference to MainCamera");
        playerGripHand = GameObject.Find("Player/Armature/MiddleBack/UpperBack/LeftUpperArm/LeftLowerArm/LeftWrist").transform;
        if (!playerGripHand) Debug.Log("Could not get a reference to the player's gripping hand");
    }

    virtual protected void Start()
    {
        pool = ScriptableObject.CreateInstance<ObjectPool>();
        pool.Init("name", PrefabManager.instance.bullet, 100);
    }

    public void InitGun(Vector3 gripLocalOffset, Vector3 barrelLocalOffset, float bulletSpeed, float bulletSpread, float bulletKickback)
    {
        this.bulletSpeed = bulletSpeed;
        this.bulletSpread = bulletSpread;
        this.bulletKickback = bulletKickback;
        this.barrelLocalOffset = barrelLocalOffset;
        this.gripLocalOffset = gripLocalOffset;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) Shoot();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!equipped) return;
        transform.position = playerGripHand.transform.position + playerGripHand.TransformVector(gripLocalOffset);
        transform.rotation = Quaternion.Euler(0, GameObject.Find("Player").transform.eulerAngles.y + 180.0f, 0);
    }

    void Shoot()
    {
        Vector3 bulletSpawn = transform.position + transform.TransformVector(barrelLocalOffset);
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100.0f, ~layerMask))
        {
            Vector3 direction = (hit.point - bulletSpawn).normalized;
            /*
            GameObject bullet = Instantiate(
                PrefabManager.instance.bullet, 
                bulletSpawn, 
                Quaternion.identity);
            */
            GameObject bullet = pool.Fetch(bulletSpawn, Quaternion.identity);
            if(bullet) bullet.GetComponent<Bullet>().InitBullet(direction, bulletKickback);
        }
        else
        {
            // error
        }
        // Instantiate(PrefabManager.instance.bullet, barrel.position, transform.rotation);
    }
}
