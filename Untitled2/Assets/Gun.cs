using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    float bulletSpeed, bulletSpread, bulletKickback;
    float bulletSpawnOffset = 0.5f;
    Transform barrel;
    Camera mainCamera;
    int layerMask;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Player");
    }

    public void InitGun(Transform barrel, float bulletSpeed, float bulletSpread, float bulletKickback)
    {
        this.barrel = barrel;
        this.bulletSpeed = bulletSpeed;
        this.bulletSpread = bulletSpread;
        this.bulletKickback = bulletKickback;
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        if (!mainCamera) Debug.Log("Could not get a reference to MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100.0f, ~layerMask))
        {
            Vector3 direction = (hit.point - barrel.position).normalized;
            Debug.Log(direction);
            GameObject bullet = Instantiate(
                PrefabManager.instance.bullet, 
                barrel.transform.position + barrel.transform.TransformVector(0, 0, -bulletSpawnOffset), 
                Quaternion.Euler(90, 0, 0) * Quaternion.LookRotation(direction));
            bullet.GetComponent<Bullet>().InitBullet(direction, bulletKickback);
        }
        else
        {
            // error
        }
        // Instantiate(PrefabManager.instance.bullet, barrel.position, transform.rotation);
    }
}
