using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipSystem2 : BasicForceSystem
{
    public bool invincible;
    public Rigidbody shipRigid;
    public Transform shipTarget;
    //public Transform cameraObject;
    public ParticleSystem speedLines;
    public GameObject deathExplosion;
    public GameObject collisionEffect;
    public MissileSystem missiles;
    public GunTest gunSystem;
    public CameraShake shakeSystem;
    public SoundSystem soundSystem;
    public MovementValues shipMovementValues;
    public MovementValues shipRotationalValues;

    [System.Serializable]
    public class ShipStats
    {
        public float shieldGrade;
        public float hullGrade;
        public float shieldDelay;
        public float shieldRechargeRate;
        public float boostRechargeSpeed;
    }
    
    public float currentShieldHealth;
    public float currentHullHealth;
    public bool active;
    public bool boostReady;
    public bool visuals;
    public bool mutedVisuals;
    public bool vJoy = true;
    public ShipStats shipStats;

    private float gravity;
    private Transform orbittingBody;
    private float boostPenalty;
    private float currentRegenTime;
    private float collisionTimer;
    private float currentThrusterVolume;
    //private Quaternion startRotation;
    private bool splashTracker;
    private bool chargeCruise;

    public InputActionAsset inputs;
    public LayerMask layerMask;

    public void SetOrbittingBody(Transform body, float g){
        orbittingBody = body;
        gravity = g;
    }

    public bool GetTracker(){
        return splashTracker;
    }

    public void SetTracker(bool boolean){
        splashTracker = boolean;
    }

    float MouseLookSystem(float mouseInputAxis, int maxValue, float deadzone)
    {

        float screenCentre = maxValue / 2;

        float finalInput = ((mouseInputAxis - screenCentre) * 1.5f) / screenCentre;
        
        if(Mathf.Abs(finalInput) < deadzone){
            finalInput = 0;
        }else{
            if(finalInput > 0){
                finalInput -= deadzone;
            }else{
                finalInput += deadzone;
            }
        }

        return Mathf.Clamp(-finalInput, -1, 1);
    }

    private void Start()
    {
        // if(cameraObject != null){
        //     startRotation = cameraObject.localRotation;
        // }

        // if(!File.Exists(Application.dataPath + "/KeyBinds.JSON")){
        //     var text = inputs.ToJson();
        //     File.WriteAllText(Application.dataPath + "/KeyBinds.JSON", text);
        // }else{
        //     var text = File.ReadAllText(Application.dataPath + "/KeyBinds.JSON");
        //     inputs.LoadFromJson(text);
        // }
        active = true;
        Cursor.visible = false;
        setThrusters(shipModel.gameObject.GetComponentsInChildren<ParticleSystem>());

        shipStats.hullGrade = Mathf.Clamp(shipStats.hullGrade / 10, 0.1f, 100);
        shipStats.shieldGrade = Mathf.Clamp(shipStats.shieldGrade / 10, 0.1f, 100);

        currentShieldHealth = 100;
        currentHullHealth = 100;

        currentBoost = 100;

        OnStart();
    }

    public virtual void OnStart() { }

    public float GetHealth()
    {
        return currentHullHealth;
    }

    public float GetShield()
    {
        return currentShieldHealth;
    }

    void Update(){
        OnUpdate();
    }

    Vector2 fakeDelta;

    void FixedUpdate()
    {
        if (active)
        {
            inputs?.actionMaps[0].Enable();
            if (currentHullHealth <= 0)
            {
                OnDeath();
                GameObject g = Instantiate(deathExplosion, transform.position, transform.rotation) as GameObject;
                g.transform.parent = transform.parent;
                Destroy(gameObject);
            }
            Vector3 movementInput = Vector3.zero;
            Vector3 rotationInput = Vector3.zero;

            if(inputs != null){
                movementInput = new Vector3(inputs["Thrust"].ReadValue<float>(), inputs["Lateral Thrust"].ReadValue<float>(), inputs["Vertical Thrust"].ReadValue<float>());
                if(vJoy){
                    rotationInput = new Vector3(inputs["Roll"].ReadValue<float>(), MouseLookSystem(inputs["Mouse Position"].ReadValue<Vector2>().y, Screen.height, 0.03f), -MouseLookSystem(inputs["Mouse Position"].ReadValue<Vector2>().x, Screen.width, 0.03f));
                }else{
                    rotationInput = new Vector3(inputs["Roll"].ReadValue<float>(), -(inputs["Pitch"].ReadValue<float>() + fakeDelta.y), inputs["Yaw"].ReadValue<float>() + fakeDelta.x);
                    
                }
            }

            HandleMovement(movementInput, rotationInput, 1);
            if (visuals) HandleVisuals(rotationInput);
        }
        else
        {
            inputs?.actionMaps[0].Disable();
            HandleMovement(Vector3.zero, Vector3.zero, 1);
        }
    }

    void ShieldRegen()
    {
        if(currentRegenTime > 0)
        {
            currentRegenTime -= Time.deltaTime;
        }
        else
        {
            currentRegenTime = 0;
            if(currentShieldHealth < 100)
            {
                currentShieldHealth += Time.deltaTime * shipStats.shieldRechargeRate;
            }
            else
            {
                currentShieldHealth = 100;
            }
        }
    }
    private float cruiseTimer;
    private bool chargeToggle;
    public virtual void OnUpdate()
    {
        ShieldRegen();

        fakeDelta += inputs["Mouse Delta"].ReadValue<Vector2>();
        fakeDelta = Vector2.ClampMagnitude(fakeDelta, 1);
        fakeDelta = Vector2.Lerp(fakeDelta, Vector2.zero, 6 * Time.deltaTime);

        chargeCruise = inputs["Super Cruise"].ReadValue<float>() > 0.5f && !superCruising ? true : false;
        SuperCruise();

        if(chargeCruise){
            cruiseTimer += Time.deltaTime;
            if(cruiseTimer >= 3){
                chargeToggle = false;
                superCruising = true;
            }
        }else{
           cruiseTimer = 0; 
        }

        if(currentBoost < 100){
            currentBoost += Time.deltaTime * shipStats.boostRechargeSpeed * boostPenalty;
        }else{
            currentBoost = 100;
        }

        if(currentBoost >= 70) boostReady = true;

        if(!superCruising){
            if (inputs["Missile"].triggered)
            {
                missiles?.FireMissile(gunSystem.shipTarget);
            }

            if (inputs["Boost"].ReadValue<float>() > 0 && boostReady){
                ShakeCamera(5, 5, true);
                boostReady = false;
                boosting = true;
                boostPenalty = Mathf.Clamp(currentBoost / 100, 0.8f, 1);
                currentBoost = 0;
                boostDuration = 0;
                soundSystem?.SetVolume(1, "Boost");
                soundSystem?.PlaySounds("Boost");
            }

            if (inputs["Decouple"].triggered) decoupled = !decoupled;
        }else{
            if(inputs["Super Cruise"].ReadValue<float>() < 0.5f){
                chargeToggle = true;
            }
            if(chargeToggle){
                if(inputs["Super Cruise"].ReadValue<float>() > 0.5f){
                    superCruising = false;
                    superCruisingEnd = true;
                }
            }
            decoupled = false;
        }

        if (inputs["Vjoy"].triggered){
            vJoy = !vJoy;
        }
    }

    void SuperCruise(){
        RaycastHit hit;
        Vector3 hitPos = Vector3.zero;
        if(Physics.SphereCast(transform.position, 1, transform.forward, out hit, 1500, layerMask)){
            hitPos = hit.point;
        }

        if(hitPos != Vector3.zero){
            chargeCruise = false;
            if(superCruising){
                superCruising = false;
                superCruisingEnd = true;
            }
        }
    }

    void HandleVisuals(Vector3 rotationalInput)
    {
        
        float xOffset = -rotationalInput.z;
        float yOffset = rotationalInput.y;

        float modifier = 1;
        float modifier2 = 1;
        float boostModifier = 1;

        if (mutedVisuals)
        {
            modifier = 25;
            modifier2 = 0.5f;
        }

        if (boostDuration > 0 && !boosting) boostModifier = 2.2f;

        shipModel.localRotation = Quaternion.Lerp(shipModel.localRotation, Quaternion.Euler(yOffset * 10 / modifier, -xOffset * 15 / modifier, (xOffset * 25 + rotationalInput.x * 10) / modifier), 3 * Time.deltaTime);

        float xPosOffset = Vector3.Dot(transform.right, shipRigid.velocity);
        float yPosOffset = Vector3.Dot(transform.up, shipRigid.velocity);
        float zPosOffset = Vector3.Dot(transform.forward, shipRigid.velocity);

        ParticleSystem.MainModule mainModule = speedLines.main;
        if(speedLines != null && zPosOffset >= 10){
            mainModule.startSizeMultiplier = 0.1f * Mathf.Clamp((zPosOffset / 10), 0, 1);
            mainModule.startSpeedMultiplier = Mathf.Clamp(zPosOffset, 20, 50);
        }else{
            mainModule.startSizeMultiplier = 0;
        }

        Vector3 visualVector = new Vector3(Mathf.Clamp(xPosOffset, -30 * modifier2, 30 * modifier2), Mathf.Clamp(yPosOffset, -30 * modifier2, 30 * modifier2), Mathf.Clamp(zPosOffset, -30 * modifier2, 30 * modifier2 * boostModifier));

        shipModel.localPosition = Vector3.Lerp(shipModel.localPosition,  (visualVector / modifier) / 10, 6 * Time.deltaTime);
    }
    
    public virtual void HandleMovement(Vector3 movementInput, Vector3 rotationInput, float maxSpeedMultiplier)
    {

        SetMovementValues(shipMovementValues);
        SetVelocity(shipRigid.velocity);
        Vector3 maxSpeedVector = new Vector3(shipMovementValues.maxSpeedVector.x * maxSpeedMultiplier, shipMovementValues.maxSpeedVector.y * maxSpeedMultiplier, shipMovementValues.maxSpeedVector.z * maxSpeedMultiplier);
        Vector3 forceVector = new Vector3(shipMovementValues.maxForceVector.x, shipMovementValues.maxForceVector.y, shipMovementValues.maxForceVector.z);

        if (boosting)
        {
            if(boostDuration < 1){
                boostDuration += Time.deltaTime;
            }else{
                ShakeCamera(10, 5, true);
                soundSystem?.PlaySounds("Boost Thrust");
                boostDuration = 1;
                boosting = false;
            }
        }else{
            if(boostDuration > 0){
                boostDuration -= Time.deltaTime / 3;
                maxSpeedVector = maxSpeedVector * shipMovementValues.speedMultiplier;
                forceVector = forceVector * shipMovementValues.forceMultiplier;
                movementInput = new Vector3(movementInput.x + 1, movementInput.y, movementInput.z);
            }
        }

        if(superCruising){
            maxSpeedVector = new Vector3(1000, 1, 1);
            forceVector = new Vector3(1000, 1000, 1000);
            movementInput = new Vector3(movementInput.x + 2, 0, 0);
        }

        if(superCruisingEnd){
            maxSpeedVector = new Vector3(1000, 1, 1);
            forceVector = new Vector3(1000, 1000, 1000);
            movementInput = new Vector3(0, 0, 0);

            if(shipRigid.velocity.magnitude < shipMovementValues.maxSpeedVector.x){
                superCruisingEnd = false;
            }
        }

        float volume = Mathf.Clamp((((movementInput.magnitude + rotationInput.magnitude) / 6) + (shipRigid.velocity.magnitude / 80)) / 4, 0, 1);

        currentThrusterVolume = Mathf.Lerp(currentThrusterVolume, volume, 6 * Time.deltaTime);

        soundSystem?.SetVolume(currentThrusterVolume, "Thrust");

        if(decoupled && orbittingBody != null) shipRigid.AddForce(shipModel.position + (orbittingBody.position - shipModel.position).normalized * shipRigid.mass * gravity);

        if(movementInput.magnitude < 0.01f && shipRigid.velocity.magnitude < 0.1f){
            shipRigid.drag = 1;
        }else{
            shipRigid.drag = 0;
        }

        shipRigid.AddForce(CalculateFinalInput(movementInput, maxSpeedVector, forceVector));

        SetMovementValues(shipRotationalValues);
    
        if(inputs != null){
            if(vJoy){
                maxSpeedVector = new Vector3(shipRotationalValues.maxSpeedVector.x * Mathf.Abs(MouseLookSystem(inputs["Mouse Position"].ReadValue<Vector2>().y, Screen.height, 0.03f)), shipRotationalValues.maxSpeedVector.y * Mathf.Abs(MouseLookSystem(inputs["Mouse Position"].ReadValue<Vector2>().x, Screen.width, 0.03f)), shipRotationalValues.maxSpeedVector.z);
            }else{
                maxSpeedVector = new Vector3(shipRotationalValues.maxSpeedVector.x, shipRotationalValues.maxSpeedVector.y, shipRotationalValues.maxSpeedVector.z);
            }
        }

        forceVector = new Vector3(shipRotationalValues.maxForceVector.x, shipRotationalValues.maxForceVector.y, shipRotationalValues.maxForceVector.z);
        transform.Rotate(CalculateFinalInput(rotationInput, maxSpeedVector, forceVector, 10));
    }

    private void ShakeCamera(float intensity, float duration, bool overwrite){
        shakeSystem?.ShakeImpulse(intensity, duration, overwrite);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ShakeCamera(10, 0.5f, false);
        if(collision.transform.tag == "Bullet")
        {
            float pitch = (Random.Range(90f, 100) / 100);
            soundSystem?.SetPitch(pitch, "Bullet Impact");
            soundSystem?.PlaySounds("Bullet Impact", 0.01f);
            OnDamage(collision.transform.GetComponent<Bullet>().Damage);
        }else{
            float pitch = (Random.Range(80f, 100) / 100);
            soundSystem?.SetPitch(pitch, "Impact Fast");
            soundSystem?.PlaySounds("Impact Fast", 0.2f);
            SpawnCollisionEffect(collision);
        }
    }

    void SpawnCollisionEffect(Collision collision){
        if(collisionEffect != null){
            foreach(ContactPoint contact in collision.contacts){
                GameObject effect = Instantiate(collisionEffect, contact.point, transform.rotation) as GameObject;
                effect.transform.parent = shipModel;
                effect.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }
    }

    private void OnCollisionStay(Collision collision){
        
        if(shipRigid.velocity.magnitude > 0.5f){
            ShakeCamera(5, 1, false);
            if(collisionTimer < 0.1f){
            collisionTimer += Time.deltaTime;
            }else{
                collisionTimer = 0;
                if(collision.relativeVelocity.magnitude > 10){
                    float pitch = (Random.Range(95, 100f) / 100);
                    soundSystem?.SetPitch(pitch, "Impact Fast");
                    soundSystem?.PlaySounds("Impact Fast", 0.2f);
                }
                SpawnCollisionEffect(collision);
            }
        }
    }

    public void OnDamage(float damage)
    {
        if(!invincible){
            if (currentShieldHealth > 0)
            {
                currentShieldHealth -= damage / shipStats.shieldGrade;
                currentRegenTime = shipStats.shieldDelay;
            }
            else
            {
                currentShieldHealth = 0;
                currentHullHealth -= damage / shipStats.hullGrade;
            }
        }
    }

    public void OnKill(float distance){
        if(distance < 100){
            currentShieldHealth = Mathf.Clamp(currentShieldHealth + 20, 0, 100);
        }
    }

    public virtual void OnDeath()
    {
        currentHullHealth = 0;
        Transform camRig = GameObject.FindGameObjectWithTag("CamRig").transform;
        camRig.transform.parent = null;
        gunSystem.KillIcons();
    }
}