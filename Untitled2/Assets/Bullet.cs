using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    const float MAX_WORLD_LENGTH = 10.0f;
    Vector3 bulletOrigin;
    GameObject bulletTrail;

    // Start is called before the first frame update
    void Start()
    {
        // bulletTrail = Instantiate(PrefabManager.instance.bulletTrail, transform.position, transform.rotation);
        bulletTrail = Instantiate(PrefabManager.instance.bulletTrail);
        bulletOrigin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.Translate(transform.TransformDirection(0,0,0.1f));
        transform.Translate(0, 0, -0.1f);
        Vector3 trailVec = bulletOrigin - transform.position;


        /*
        float trailScale = 0.5f * (transform.position - bulletOrigin).magnitude;
        bulletTrail.transform.localScale = new Vector3(1, trailScale, 1);
        bulletTrail.transform.localPosition = new Vector3(0, 0, -trailScale) + transform.position;
        */
    }
}
