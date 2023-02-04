using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Agent[] userAgents;
    [SerializeField] Agent[] enemyAgents;

    [SerializeField] private Button orderAgentsButton;
    [SerializeField] private Button attackPlantButton;
    [SerializeField] private Button attackTreeButton;
    [SerializeField] private Button attackFlowerButton;
    [SerializeField] private Button attackButton;

    private int numEnemiesAlive;
    private int numUserAgentsAlive;

    private Agent[] allAgents;

    private bool finishedGame;

    private Agent attacker;
    private Agent target;

    bool isCoroutineReady = true;

    int index = -1;
    int turnsPassed = 0;

    void Start()
    {
        Debug.Log("Starting game");
        allAgents = userAgents.Concat(enemyAgents).ToArray();

        numEnemiesAlive = enemyAgents.Length;
        numUserAgentsAlive = userAgents.Length;

        // Start off with attack and target buttons disabled
        orderAgentsButton.gameObject.SetActive(true);
        attackPlantButton.gameObject.SetActive(false);
        attackTreeButton.gameObject.SetActive(false);
        attackFlowerButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(false);
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
        }
        yield return new WaitForSeconds(1);

        attacker.SetAttackTrigger(false);

        if (target.isDead) {
            if (target.isUser) {
                numUserAgentsAlive--;
            } else {
                numEnemiesAlive--;
            }

            // Remove target from the board
            target.gameObject.SetActive(false);

            checkWinLoseConditions();
        }

        Debug.Log("Done with turn");
        turnsPassed++;

        if (turnsPassed == allAgents.Length) {
            orderAgentsButton.gameObject.SetActive(true);
            attackPlantButton.gameObject.SetActive(false);
            attackTreeButton.gameObject.SetActive(false);
            attackFlowerButton.gameObject.SetActive(false);
            attackButton.gameObject.SetActive(false);
        } else {
            UpdateAttacker();
        }

        // orderAgentsButton.gameObject.SetActive(true);
        // attackPlantButton.gameObject.SetActive(false);
        // attackTreeButton.gameObject.SetActive(false);
        // attackFlowerButton.gameObject.SetActive(false);
        // attackButton.gameObject.SetActive(false);
        yield return null;
    }

    public void orderAgentsBySpeed() {
        Debug.Log("orderAgentsBySpeed");
        turnsPassed = 0;

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

            if (agent.isDead) {
                continue;
            }

            Vector3 temp = new Vector3(start,0,0);

            agent.gameObject.transform.position += temp;
            start += 2;
        }
        UpdateAttacker();
    }

    public void UpdateAttacker() {
        index++;
        if (index == allAgents.Length) {
            index = 0;
        }

        attacker = allAgents[index];

        if (attacker.isDead) {
            turnsPassed++;
            UpdateAttacker();
        }

        if (attacker.isUser) {
            // Show target buttons
            orderAgentsButton.gameObject.SetActive(false);
            attackPlantButton.gameObject.SetActive(true);
            attackTreeButton.gameObject.SetActive(true);
            attackFlowerButton.gameObject.SetActive(true);
        } else {
            attackButton.gameObject.SetActive(true);
        }
    }

    public void SetTargetToPlant() {
        target = allAgents.First(x => x.gameObject.name.Contains("Plant"));
        Debug.Log("target is now " + target.gameObject.name);

        attackPlantButton.gameObject.SetActive(false);
        attackTreeButton.gameObject.SetActive(false);
        attackFlowerButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(true);
    }

    public void SetTargetToTree() {
        target = allAgents.First(x => x.gameObject.name.Contains("Tree"));
        Debug.Log("target is now " + target.gameObject.name);

        attackPlantButton.gameObject.SetActive(false);
        attackTreeButton.gameObject.SetActive(false);
        attackFlowerButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(true);
    }

    public void SetTargetToFlower() {
        target = allAgents.First(x => x.gameObject.name.Contains("Flower"));
        Debug.Log("target is now " + target.gameObject.name);

        attackPlantButton.gameObject.SetActive(false);
        attackTreeButton.gameObject.SetActive(false);
        attackFlowerButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(true);
    }

    public void PerformAttack() {
        Debug.Log("Attacker is " + attacker.gameObject.name);

        // Make sure we're not dead
        if (attacker.isDead) {
            Debug.Log("can't attack because dead");
            return;
        }

        if (!attacker.isUser && numUserAgentsAlive > 0) {
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
