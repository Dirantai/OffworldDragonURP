using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipUI : MonoBehaviour
{
    private ShipSystem2 controller;
    private GunTest gunSystem;
    public Transform healthBar;
    public Transform shieldBar;
    public Transform energyBar;
    public Transform nEnergyBar;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ShipSystem2>();
        gunSystem = GetComponent<GunTest>();
    }

    // Update is called once per frame
    void Update()
    {

        healthBar.localScale = new Vector3(0.5f, 0.5f * controller.GetHealth() / 100, 1);
        shieldBar.localScale = new Vector3(0.5f, 0.5f * controller.GetShield() / 100, 1);
        healthBar.localPosition = new Vector3(healthBar.localPosition.x, -160 *  ((100 - controller.GetHealth()) / 100), 0);
        shieldBar.localPosition = new Vector3(shieldBar.localPosition.x, -160 * ((100 - controller.GetShield()) / 100), 0);
        if(gunSystem.UpdateUI()){
            energyBar.localScale = new Vector3(0.5f, 0.5f * gunSystem.GetCurrentEnergy() / 100, 1);
            energyBar.localPosition = new Vector3(energyBar.localPosition.x, -160 * ((100 - gunSystem.GetCurrentEnergy()) / 100), 0);
        }
        nEnergyBar.localScale = new Vector3(0.5f, 0.5f * gunSystem.GetNewEnergy() / 100, 1);
        nEnergyBar.localPosition = new Vector3(energyBar.localPosition.x, -160 * ((100 - gunSystem.GetNewEnergy()) / 100), 0);
    }
}
