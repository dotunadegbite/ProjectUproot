using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    // User agents
    [SerializeField] GameObject Barberian;
    [SerializeField] GameObject Farmer;
    [SerializeField] GameObject FarmerGirl;

    // Enemy agents
    [SerializeField] GameObject[] enemies;

    public void triggerCombat() {
        Debug.Log("triggerCombat");
        // 1. Figure out combat order, based on speed stat

        // 2. For each agent in the scene, perform an attack
    }
}
