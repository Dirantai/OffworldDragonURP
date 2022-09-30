using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLooker : FollowPlayer
{
    private GunTest playerGuns;
    private Quaternion previousRotation;

    void Start()
    {
        playerGuns = player.GetComponent<GunTest>();
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        base.OnUpdate();
        if(playerGuns.shipTarget != null){
                Quaternion lookAtTarget = Quaternion.LookRotation(playerGuns.shipTarget.position - transform.position);
                transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, lookAtTarget, 10 * Time.deltaTime);
                previousRotation = transform.GetChild(0).rotation;
        }else{
                transform.rotation = player.rotation;
                if(previousRotation != Quaternion.Euler(0, 0, 0)){
                    transform.GetChild(0).rotation = previousRotation;
                    previousRotation = Quaternion.Euler(0, 0, 0);
                }
                transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 0, 0), 10 * Time.deltaTime);
        }
    }
}
