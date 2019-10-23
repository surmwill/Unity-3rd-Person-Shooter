using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;
    public GameObject m16, bullet, bulletTrail, groundBlock, grassPlatform;

    void Awake()
    {
        instance = this;
    }
}
