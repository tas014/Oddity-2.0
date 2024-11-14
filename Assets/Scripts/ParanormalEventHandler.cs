using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParanormalEventHandler : MonoBehaviour
{
    public GameObject[] availableProps = null;
    [SerializeField] private Player player;
    public bool isEventOnCooldown = true;
    public bool didSpawnEvent = false;
    private float minEventCooldown = 40;
    private float cooldownTimer = 0;

    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (CanSpawnEvent() && availableProps != null) {
            int chosenObj = Random.Range(0, availableProps.Length);
            AttemptEventSpawn(availableProps[chosenObj]);
            if (didSpawnEvent) {
                didSpawnEvent = false;
                player.eventObject = availableProps[chosenObj];
                player.eventStartTime = Time.time;
                Debug.Log("Event spawned!");
            }
        }
        if (Input.GetKeyDown(KeyCode.F7)) {
            cooldownTimer = minEventCooldown;
        }
    }

    private bool CanSpawnEvent() {
        // Actual spawn conditions go here... (are the parents there, etc...)
        bool canSpawn = cooldownTimer >= minEventCooldown;
        return canSpawn;
    }

    private void AttemptEventSpawn (GameObject prop) {
        PropBehavior target = prop.GetComponent<PropBehavior>();
        target.ToggleEventTrigger();
        Debug.Log("The object " + prop.name + " was chosen for an event");
    }

    public void ToggleEventCooldown() {
        isEventOnCooldown = !isEventOnCooldown;
        didSpawnEvent = true;
        cooldownTimer = 0f;
    }
}
