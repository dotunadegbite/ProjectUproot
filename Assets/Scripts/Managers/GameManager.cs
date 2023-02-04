using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CombatManager cm;

    void Start()
    {
        Debug.Log("Starting game");
    }

    void Update()
    {
        cm.triggerCombat();
    }
}
