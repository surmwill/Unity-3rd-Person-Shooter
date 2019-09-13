using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public struct CollisionProperties
    {
        public Vector3 bulletDirection;
        public float kickBack;

        public CollisionProperties(Vector3 bulletDirection, float kickBack)
        {
            this.kickBack = kickBack;
            this.bulletDirection = bulletDirection;
        }
    }

    const float MAX_WORLD_LENGTH = 10.0f;
    float bulletSpeed = 5.0f;
    float bulletSize = 0.1f;
    const float timeToLive = 10.0f;
    float currTime = 0.0f;

    Vector3 bulletOrigin;
    GameObject bulletTrail;
    CollisionProperties collisionProperties;

    void Awake()
    {
        bulletTrail = Instantiate(PrefabManager.instance.bulletTrail);
        bulletTrail.SetActive(false);
    }

    void OnEnable()
    {
        bulletTrail.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
        bulletTrail.SetActive(true);
        bulletOrigin = transform.position;
        transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
        currTime = 0.0f;
    }

    void OnDisable()
    {
        bulletTrail.SetActive(false);
    }

    public void InitBullet(Vector3 bulletDirection, float kickBack)
    {
        collisionProperties = new CollisionProperties(bulletDirection, kickBack);
    }

    public CollisionProperties GetCollisionProperties() { return collisionProperties; }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(collisionProperties.bulletDirection * Time.deltaTime * bulletSpeed, Space.World);
        Vector3 trailVec = bulletOrigin - transform.position;
    
        float trailScale = 0.5f * (transform.position - bulletOrigin).magnitude;
        bulletTrail.transform.position = transform.position + 0.5f * trailVec;
        bulletTrail.transform.localScale = new Vector3(bulletSize, trailScale, bulletSize);

        if(trailVec != Vector3.zero) bulletTrail.transform.rotation = Quaternion.LookRotation(trailVec) * Quaternion.AngleAxis(90, Vector3.right);

        currTime += Time.deltaTime;
        if (currTime > timeToLive) gameObject.SetActive(false);
    }
}
