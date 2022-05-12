using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamikaze : RushEnemy
{
    public override void Attack()
    {
        //Kill self to spawn the explosion deathFX
        Death();
    }
}
