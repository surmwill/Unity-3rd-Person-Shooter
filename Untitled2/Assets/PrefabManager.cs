using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;
    public GameObject bullet, bulletTrail;

    void Awake()
    {
        instance = this;

        bullet = GameObject.Find("Bullet");
        if (!bullet) Debug.Log("Could not get a reference to the Bullet prefab");

        bulletTrail = GameObject.Find("BulletTrail");
        if (!bulletTrail) Debug.Log("Could not get a reference to the BulletTrail prefab");
    }
}
