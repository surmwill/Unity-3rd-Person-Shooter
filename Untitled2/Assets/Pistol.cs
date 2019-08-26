using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    // Start is called before the first frame update
    void Start()
    {
        InitGun(transform.Find("Barrel"), 1.0f, 1.0f);
    }
}
