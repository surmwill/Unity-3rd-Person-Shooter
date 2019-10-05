using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Used to calculate the push back a player experiences when colliding with the bullet
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

    const float MAX_WORLD_LENGTH = 10.0f;   // max length of the bullet trail
    const float TIME_TO_LIVE = 1.0f;    // time that the bullet trail exists

    public CollisionProperties collisionProperties { get; private set; }

    float bulletSpeed = 5.0f, bulletSize = 0.1f;
    float currTime = 0.0f;

    Vector3 bulletOrigin;
    GameObject bulletTrail;

    void Awake()
    {
        transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
        bulletTrail = Instantiate(PrefabManager.instance.bulletTrail);
        bulletTrail.SetActive(false);
    }

    void OnEnable()
    {
        bulletOrigin = transform.position;
        currTime = 0.0f;
    }

    public void InitBullet(Vector3 bulletDirection, float kickBack)
    {
        collisionProperties = new CollisionProperties(bulletDirection, kickBack);
    }

    // Update is called once per frame
    void Update()
    {
        /* 
         * This cannot go into OnEnable() as one might expect. This is because the Gun fetches the bullet from
         * its ObjectPool in its Update() call. The ObjectPool sets the bullet to be active which triggers
         * the Bullet OnEnable() callback. If the bullet trail is set to active in OnEnable() it will appear
         * in its old position until its position is updated in the next Update() call NEXT frame. This causes
         * a strange positional jump.
         */
        if (!bulletTrail.activeInHierarchy) bulletTrail.SetActive(true);

        // move the bullet, and then construct the trail according to the bullet's position and the origin from which it was fired
        transform.Translate(collisionProperties.bulletDirection * Time.deltaTime * bulletSpeed, Space.World);
        Vector3 trailVec = bulletOrigin - transform.position;

        // Scale according to the distance between the bullet's position and origin. 0.5f scaled since a cyclinder has height 2
        float trailScale = 0.5f * (transform.position - bulletOrigin).magnitude;    
        bulletTrail.transform.position = transform.position + 0.5f * trailVec;  // move halfway between the bullet and its origin
        bulletTrail.transform.localScale = new Vector3(bulletSize, trailScale, bulletSize);
        bulletTrail.transform.rotation = Quaternion.LookRotation(trailVec) * Quaternion.AngleAxis(90, Vector3.right);

        currTime += Time.deltaTime;
        if (currTime > TIME_TO_LIVE)
        {
            gameObject.SetActive(false);
            bulletTrail.SetActive(false);
        }
       
    }
}
