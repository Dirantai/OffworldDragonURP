using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BoomerZoomer : ShipAI
{
    public override void OnUpdate()
    {
        if (shipTarget != null)
        {
            float distanceToShipTarget = (transform.position - shipTarget.position).magnitude;
            if (shipState == ShipState.Wrestling)
            {
                movementTarget = shipTarget.position;
                if (distanceToShipTarget < engagementRanges.gunRange * 0.1f)
                {
                    shoot = true;
                }
            }
            else
            {
                shoot = false;
            }

            if (gunSystem != null)
            {
                gunSystem.shipTarget = shipTarget;
                gunSystem.Shoot = shoot;
            }

        }
        else
        {
            shipState = ShipState.Breakingoff;
            SelectTarget();
            gunSystem.Shoot = false;
        }

        //PathFinder(movementTarget - transform.position);

        if (markerUI != null)
        {
            markerUI.iconPosition = transform.position;
        }
    }

    public override void HandleMovement(Vector3 movementInput, Vector3 rotationInput, float maxSpeedMultiplier)
    {
        movementVector = movementTarget;
        float distanceToTarget = (movementTarget - transform.position).magnitude;
        Vector3 trueMovementTarget = movementVector;
        Vector3 trueRotationTarget = movementTarget;
        Vector3 rollTarget = transform.up;
        bool roll = false;
        float dotDirection = Vector3.Dot(transform.forward, (movementTarget - transform.position).normalized);

        switch (shipState)
        {
            case ShipState.Breakingoff:
                movementInput = new Vector3(0.2f, 0, 0);
                float sign = direction % 2;
                if(sign > 0){
                    sign = -1;
                }else{
                    sign = 1;
                }

                if(direction < 50){
                    rotationInput = new Vector3(0, 0, -0.1f * sign);
                }else if(direction >= 50){
                    rotationInput = new Vector3(0, -0.1f * sign, 0);
                }
                
                if(dotDirection > 0){
                    shipState = ShipState.Wrestling;
                }

                break;
            case ShipState.Wrestling:
                roll = true;
                rollTarget = movementTarget + Vector3.up;
                trueRotationTarget = movementTarget + (movementTarget - transform.position).normalized * 100;
                movementInput = new Vector3(1, 0, 0);
                rotationInput = AimAtTarget(rollTarget, trueRotationTarget, roll);

                if(distanceToTarget < 100 && dotDirection < 0){
                    shipState = ShipState.Breakingoff;
                }

                break;
            case ShipState.Idle:
                direction = Random.Range(0, 100);
                Transform player = null;
                if (GameObject.FindGameObjectWithTag("Player") != null)
                {
                    player = GameObject.FindGameObjectWithTag("Player").transform;
                    shipState = ShipState.Breakingoff;
                }
                break;
            // case ShipState.Pathing:
            //     maxSpeedMultiplier = 0.3f;
            //     player = null;
            //     if (GameObject.FindGameObjectWithTag("Player") != null)
            //     {
            //         player = GameObject.FindGameObjectWithTag("Player").transform;
            //     }

            //     if (player != null && transform.tag == "Ally")
            //     {
            //         if (shipTarget == null)
            //         {
            //             movementTarget = player.position + (player.right * (wingmanPosition.x / 10)) + (player.forward * (wingmanPosition.z / 10));
            //         }
            //     }
            //     else
            //     {
            //         if (shipTarget != null)
            //         {
            //             movementTarget = shipTarget.position;
            //         }
            //     }

            //     if (Node != null)
            //     {
            //         movementVector = Node;
            //     }
            //     distanceToTarget = (transform.position - movementVector).magnitude;
            //     trueMovementTarget = movementVector;
            //     trueRotationTarget = movementVector;
            //     break;
        }

        movementInput = HandleCollisionDetection(movementInput);
        
        base.HandleMovement(movementInput, rotationInput, maxSpeedMultiplier);
    }

    void SelectTarget()
    {
        GameObject closestEnemy = null;
        float randomNumber = Random.Range(0, 100);

        if (transform.tag == "Enemy")
        {

            GameObject[] TargetList = GameObject.FindGameObjectsWithTag("Ally");
            GameObject[] updatedTargetList = new GameObject[TargetList.Length + 1];
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                updatedTargetList[0] = GameObject.FindGameObjectWithTag("Player");
                for (int i = 1; i < updatedTargetList.Length; i++)
                {
                    updatedTargetList[i] = TargetList[i - 1];
                }
            }
            else
            {
                updatedTargetList = TargetList;
            }

            float closestDistance = 0;
            foreach (GameObject enemy in updatedTargetList)
            {
                if (closestDistance == 0)
                {
                    closestDistance = (transform.position - enemy.transform.position).magnitude;
                    closestEnemy = enemy;
                }
                else
                {
                    float distanceFromTarget = (transform.position - enemy.transform.position).magnitude;
                    if (distanceFromTarget < closestDistance)
                    {
                        closestDistance = distanceFromTarget;
                        closestEnemy = enemy;
                    }
                }
            }
        }

        if (closestEnemy != null)
        {
            shipTarget = closestEnemy.transform;
            if (shipTarget.tag != "Player")
            {
                shipTarget.GetComponent<ShipAI>().OnLock(transform);
            }
        } 
    }
}
