using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Only handles movement, expects to have a EnemyWeapon attached to do damage, but not necessary
/// </summary>
public class ZoningEnemy : BaseEnemy
{
    [SerializeField]
    private float targetDistance;

    [SerializeField]
    private float offsetRotation = 90;

    private Vector3 targetLocation = new Vector3();

    // Update is called once per frame
    private void Update()
    {
        stopTime -= Time.deltaTime;
        if (!health.HasDied && stopTime <= 0)
        {
            OnMove();
        }
    }

    private void OnMove()
    {
        //Get opposite direction vector
        Vector3 targetToMe = (transform.position - target.transform.position).normalized;

        //Find zoning location
        targetLocation = target.transform.position + targetToMe * targetDistance;

        //Get distance vector to target location
        //Vector3 distance = targetLocation - transform.position;

        //Orient rotation
        Vector3 toTarget = (target.transform.position - transform.position).normalized;
        float newAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0, newAngle + offsetRotation);

        rb.position = Vector3.Lerp(rb.position, targetLocation, Time.deltaTime * speed);

        //transform.position += velocity;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetLocation, .2f);
    }
}
