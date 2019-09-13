using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    private Vector3 m16GripLocalOffset = new Vector3(0f, 1.65f, -0.5f);    // grip y is forward, x left, z up
    private Vector3 m16BarrelLocalOffset = new Vector3(0, 0, -2.0f);

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        InitGun(m16GripLocalOffset, m16BarrelLocalOffset, 1.0f, 1.0f, 2.5f);
    }
}
