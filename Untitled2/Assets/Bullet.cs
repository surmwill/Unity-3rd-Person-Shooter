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
    Vector3 bulletOrigin;
    GameObject bulletTrail;
    CollisionProperties collisionProperties;

    // Start is called before the first frame update
    void Start()
    {
        bulletTrail = Instantiate(PrefabManager.instance.bulletTrail);
        bulletTrail.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
        bulletTrail.transform.position = transform.position;
        bulletOrigin = transform.position;
    }

    public void InitBullet(Vector3 bulletDirection, float kickBack)
    {
        collisionProperties = new CollisionProperties(bulletDirection, kickBack);
    }

    public CollisionProperties GetCollisionProperties() { return collisionProperties; }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(collisionProperties.bulletDirection * Time.deltaTime, Space.World);
        Vector3 trailVec = bulletOrigin - transform.position;
    
        float trailScale = 0.5f * (transform.position - bulletOrigin).magnitude;
        bulletTrail.transform.position = transform.position + 0.5f * trailVec;
        bulletTrail.transform.localScale = new Vector3(1, trailScale, 1);

        if(trailVec != Vector3.zero) bulletTrail.transform.rotation = Quaternion.LookRotation(trailVec) * Quaternion.AngleAxis(90, Vector3.right);
    }
}
