using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTest : MonoBehaviour
{

    public Transform shipTarget;
    public HitMarkerSystem hitMarkerSystem;
    public Transform cursorUI;
    private ShipSystem2 player;
    public float speedMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        speedMultiplier = 1;
        Cursor.lockState = CursorLockMode.Confined;
        turrets = GetComponentsInChildren<Turret>();
        player = GetComponent<ShipSystem2>();
    }

    Vector3 Intercept(float bulletSpeed)
    {
        InterceptionSystem interceptor = new InterceptionSystem();
        if (shipTarget != null)
        {
            return interceptor.CalculateInterceptPosition(shipTarget.position, shipTarget.GetComponent<Rigidbody>().velocity, transform.position, bulletSpeed);
        }

        return Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        ControlTurrets();
        if (!AI)
        {
            HandleEnemySelection();
        }
        else
        {
            if (shipTarget != null)
            {
                if (shipTarget.tag == "Player")
                {
                    speedMultiplier = 0.75f;
                }
                else
                {
                    speedMultiplier = 1;
                }
            }
        }
    }

    void HandleEnemySelection()
    {
        if (player.inputs["Target"].triggered)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Vector3 enemyScreenPoint = Camera.main.WorldToScreenPoint(enemy.transform.position);
                enemyScreenPoint = new Vector3(enemyScreenPoint.x, enemyScreenPoint.y, 0);
                Vector3 mouseToEnemy = enemyScreenPoint - cursorUI.position;
                if(mouseToEnemy.magnitude <= 45)
                {
                    shipTarget = enemy.transform;
                    shipTarget.GetComponent<ShipAI>().OnLock(transform);
                }
            }
        }
    }

    Vector3 AimingSystem(float bulletSpeed, Turret turret, UIElementSystem aimReticle)
    {
        Vector3 interceptPoint = Intercept(bulletSpeed);
        float reticleProduct = Vector3.Dot(interceptPoint - transform.position, transform.forward);
        if(player.vJoy){
            cursorUI.position = player.inputs["Mouse Position"].ReadValue<Vector2>();
        }else{
            cursorUI.position = new Vector2(Screen.width / 2, Screen.height / 2);
        }

        Ray reticleRay = Camera.main.ScreenPointToRay(cursorUI.position);

        if (shipTarget != null)
        {
            float distance = (shipTarget.position - reticleRay.origin).magnitude;
            
            Vector3 aimPoint = reticleRay.origin + (reticleRay.direction * distance);

            if (aimReticle != null)
            {
                aimReticle.iconPosition = interceptPoint;
            }
            Vector3 aimpointToIntercept = aimPoint - interceptPoint;
            Vector3 trueAimPoint = Vector3.zero;
            Vector3 screenAimToIntercept = Camera.main.WorldToScreenPoint(aimPoint) - Camera.main.WorldToScreenPoint(interceptPoint);
            if (reticleProduct > 0)
            {
                if (screenAimToIntercept.magnitude < 100)
                {
                    Shoot = true;
                    trueAimPoint = aimpointToIntercept / (200 / (screenAimToIntercept.magnitude)) + interceptPoint;
                    if(screenAimToIntercept.magnitude < 25)
                    {
                        trueAimPoint = interceptPoint;
                    }
                }
                else
                {
                    if(player.inputs["Shoot"].ReadValue<float>() == 0) Shoot = false;
                    trueAimPoint = aimPoint;
                }
            }
            else
            {
                distance = 100;
                trueAimPoint = reticleRay.origin + (reticleRay.direction * 100);
            }

            turret.HandleUI(distance);

            return trueAimPoint;
        }
        else
        {
            if(player.inputs["Shoot"].ReadValue<float>() == 0) Shoot = false;
            turret.HandleUI(100);

            Vector3 trueAimPoint = reticleRay.origin + (reticleRay.direction * 100);

            if (aimReticle != null)
            {
                aimReticle.iconPosition = trueAimPoint;
            }
            return trueAimPoint;
        }
    }

    public Turret[] turrets;
    public bool Shoot;
    public bool AI;
    Vector3 aimPoint;
    private float deltaInput;
    void ControlTurrets()
    {
        
        foreach (Turret turret in turrets)
        {
            
            if (turret.indecatorReticle != null)
            {
                aimPoint = AimingSystem(turret.weaponValues.projectileSpeed, turret, turret.indecatorReticle.GetComponent<UIElementSystem>());
            }
            else
            {
                //aimPoint = Intercept(turret.weaponValues.projectileSpeed * speedMultiplier);
                if (shipTarget != null)
                {
                    aimPoint = TestIntercept(turret.weaponValues.projectileSpeed * speedMultiplier, shipTarget.position, shipTarget.GetComponent<Rigidbody>().velocity, transform.position);
                }
            }
            turret.TurretTurn(aimPoint, turret.weaponValues.turnSpeed);
            if (!AI)
            {
                if(deltaInput < player.inputs["Shoot"].ReadValue<float>()){
                    deltaInput = player.inputs["Shoot"].ReadValue<float>();
                    Shoot = true;
                }else if (deltaInput > player.inputs["Shoot"].ReadValue<float>()){
                    deltaInput = player.inputs["Shoot"].ReadValue<float>();
                    Shoot = false;
                }
            }
            GunShoot(turret.weaponValues, Shoot);
        }
    }

    public Vector3 GetIntercept(){
        return aimPoint;
    }

    public int accuracy;

    Vector3 TestIntercept(float projectileSpeed, Vector3 targetPosition, Vector3 targetVelocity, Vector3 shipPosition)
    {
        float timeToIntercept = 0;
        float distanceToTarget = (targetPosition - shipPosition).magnitude;
        timeToIntercept = distanceToTarget / projectileSpeed;
        for (int i = 0; i < accuracy; i++)
        {
            distanceToTarget = ((targetPosition + (targetVelocity * timeToIntercept)) - shipPosition).magnitude;
            timeToIntercept = distanceToTarget / projectileSpeed;
        }
        return targetPosition + (targetVelocity * timeToIntercept);
    }


    public void GunShoot(Turret.WeaponValues weaponValues, bool shoot)
    {
        if (shoot && weaponValues.loaded)
        {
            player.soundSystem?.SetPitch(0.8f, "Shoot");
            player.soundSystem.PlaySounds("Shoot", 0.1f);
            weaponValues.loaded = false;
            GameObject instancedBullet = Instantiate(weaponValues.bulletModel, weaponValues.Barrel.position, weaponValues.Barrel.rotation) as GameObject;
            instancedBullet.transform.parent = transform.parent;
            instancedBullet.GetComponent<Bullet>().BulletVelocity = weaponValues.projectileSpeed * speedMultiplier;
            instancedBullet.GetComponent<Bullet>().Damage = weaponValues.baseDamage;
            instancedBullet.GetComponent<Bullet>().HitMarkerDetector = weaponValues.hitMarker;
            instancedBullet.GetComponent<Bullet>().shooter = player;
            instancedBullet.transform.rotation = Quaternion.LookRotation(CalculateSpreadVector(weaponValues)); //apply the spread
        }
    }

    Vector3 CalculateSpreadVector(Turret.WeaponValues weaponValues) //calculates spread and turns it into a vector
    {
        var spreadVector = Vector3.zero;

        var angle = Random.Range(0, 2 * Mathf.PI); //gets a random angle between 0 and 2pi radians. determines which direction the bullet will diviate
        var offset = Random.Range(0, weaponValues.spread * Mathf.Deg2Rad); //get an angle of how far the bullet will deviate from the centre in radians

        spreadVector += weaponValues.Barrel.right * Mathf.Cos(angle) * Mathf.Sin(offset); //calculate the x value
        spreadVector += weaponValues.Barrel.up * Mathf.Sin(angle) * Mathf.Sin(offset); //calculate the y value
        spreadVector += weaponValues.Barrel.forward * Mathf.Cos(offset); //tbh I think this is a useless value now

        return spreadVector.normalized; //normalize it
    }
}
