using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Agent[] userAgents;
    [SerializeField] Agent[] enemyAgents;

    private int numEnemiesAlive;
    private int numUserAgentsAlive;

    private Agent[] allAgents;

    private bool finishedGame;

    private Agent attacker;
    private Agent target;

    bool isCoroutineReady = true;

    int index = -1;

    void Start()
    {
        Debug.Log("Starting game");
        allAgents = userAgents.Concat(enemyAgents).ToArray();

        numEnemiesAlive = enemyAgents.Length;
        numUserAgentsAlive = userAgents.Length;
    }

    IEnumerator AttackCoroutine()
    {
        attacker.SetAttackTrigger(true);

        var damageToDeal = this.attacker.attack;
        Debug.Log(attacker.gameObject.name + " is attacking " + target.gameObject.name);

        target.health -= damageToDeal;

        Debug.Log(target.gameObject.name + " health is now " + target.health);

        if (target.health <= 0) {
            Debug.Log(target.gameObject.name + " has died");
            // target died
            target.isDead = true;

            // Remove target from the board
            target.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1);

        attacker.SetAttackTrigger(false);

        if (target.isDead) {
            if (target.isUser) {
                numUserAgentsAlive--;
            } else {
                numEnemiesAlive--;
            }

            checkWinLoseConditions();
        }

        Debug.Log("Done with turn");
        yield return null;
    }

    public void orderAgentsBySpeed() {
        Debug.Log("orderAgentsBySpeed");
        System.Array.Sort(allAgents, new AgentComparer());

        // Reset all positions
        for (int i = 0; i < allAgents.Length; i++) {
            var agent = allAgents[i];

            Vector3 temp = new Vector3(0,0,0);

            agent.gameObject.transform.position = temp;
        }

        int start = -6;

        for (int i = 0; i < allAgents.Length; i++) {
            var agent = allAgents[i];

            Vector3 temp = new Vector3(start,0,0);

            agent.gameObject.transform.position += temp;
            start += 2;
        }
    }

    public void UpdateAttacker() {
        index++;
        if (index == allAgents.Length) {
            index = 0;
        }

        attacker = allAgents[index];

        if (attacker.isDead) {
            UpdateAttacker();
        }
    }

    public void PerformAttack() {
        UpdateAttacker();
        Debug.Log("Attacker is " + attacker.gameObject.name);

        // Make sure we're not dead
        if (attacker.isDead) {
            Debug.Log("can't attack because dead");
            return;
        }

        // Find the target
        target = null;
        if (attacker.isUser && numEnemiesAlive > 0) {
            // Find a random enemy to attack
            // Debug.Log("Found an enemy to attack");
            for (int j = 0; j < enemyAgents.Length; j++) {
                if (enemyAgents[j].health > 0) {
                    target = enemyAgents[j];
                    break;
                }
            }
        } else if (!attacker.isUser && numUserAgentsAlive > 0) {
            // Find a random user agent to attack
            // Debug.Log("Found an user agent to attack");
            for (int j = 0; j < userAgents.Length; j++) {
                if (userAgents[j].health > 0) {
                    target = userAgents[j];
                    break;
                }
            }
        }

        // Combat
        if (target != null) {
            StartCoroutine(AttackCoroutine());
        }
    }

    private void checkWinLoseConditions() {
        // Debug.Log("num user agents alive = " + numUserAgentsAlive);
        // Debug.Log("num enemy agents alive = " + numEnemiesAlive);
        if (numUserAgentsAlive == 0) {
            // Lose
            Debug.Log("You lose");

            // Transition to lose screen
            finishedGame = true;
        } else if (numEnemiesAlive == 0) {
            // Win
            Debug.Log("You win");

            // Transition to win screen

            finishedGame = true;
        }
    }
}

class AgentComparer : IComparer<Agent>
{
    public int Compare(Agent x, Agent y)
    {
        if (x.speed > y.speed) {
            return -1;
        } else if (x.speed < y.speed) {
            return 1;
        } else {
            var rand = new System.Random();

            return rand.NextDouble() >= 0.5 ? 1 : -1;
        }
    }
}
