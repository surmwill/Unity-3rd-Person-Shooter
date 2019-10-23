using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * InitGun(
 *      Vector3 gripLocalOffset, 
 *      Vector3 barrelLocalOffset, 
 *      float bulletSpread,
 *      float bulletKickback, 
 *      float timeBetweenShots
 * )
 */

public class M16 : Gun
{
    Vector3 GRIP_LOCAL_OFFSET = new Vector3(0f, 1.65f, -0.5f);    // grip y is forward, x left, z up
    Vector3 BARREL_LOCAL_OFFSET = new Vector3(0, 0, -2.0f);
    const float BULLET_SPREAD = 0.01f, BULLET_KICKBACK = 1.0f, TIME_BETWEEN_SHOTS = 0.1f;

    protected override void Start()
    {
        base.Start();
        InitGun(GRIP_LOCAL_OFFSET, BARREL_LOCAL_OFFSET, BULLET_SPREAD, BULLET_KICKBACK, TIME_BETWEEN_SHOTS);
    }
}
