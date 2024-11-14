using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropBehavior : MonoBehaviour {

    [Header("Event Data")] 
    public bool eventTrigger = false;
    public ParanormalEventHandler paranormalEventHandler;
    [SerializeField] private int intensity = 0;
    //[SerializeField] private Player player;

    // Rigidbody access
    Rigidbody prop_rigidbody;

    // Floating paranormal event data
    private readonly float floatingSpeed = 2;
    private Vector3 targetPosition;
    private float floatingTime = 20f;

    // Throwing paranormal event data
    private readonly float throwRange = 4;
    private readonly float verticalThrowForce = 3;
    private readonly float throwForce = 50;

    // Light toss paranormal event data
    private readonly float tossForce = 15;
    private readonly float tossRange = 0.5f;


    private void Start() {
        targetPosition = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        prop_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (eventTrigger) {
            if (AttemptEventGeneration(intensity)) {
                ToggleEventTrigger();
                paranormalEventHandler.ToggleEventCooldown();
                // add some way of telling the paranormal event handler that the event triggered.
            }
        }
    }

    private bool AttemptEventGeneration(int eventIntensity) {
        return eventIntensity switch {
            1 => TryWeakEvent(),
            2 => TryMediumEvent(),
            3 => TryStrongEvent(),
            _ => false,
        };
    }

    private bool TryWeakEvent() {
        //Weak event affecting this object...
        Vector3 canToss = GetClearTossDirection();
        if (canToss != transform.position) {
            TossObject(canToss);
            return true;
        }
        return false;
    }
    private bool TryMediumEvent() {
        //Medium event affecting this object...

        Vector3 canThrow = GetClearThrowingDirection();
        if (canThrow != transform.position) {
            ThrowObject(canThrow);
            return true;
        }
        return false;
    }
    private bool TryStrongEvent() {
        //Strong event affecting this object...
        bool canLevitate = !Physics.Linecast(transform.position, targetPosition);
        if (canLevitate) {
            Levitate(targetPosition);
        }
        return canLevitate;
    }

    private void Levitate(Vector3 levitatingPosition) {
        GravityDisableRoutine();
        transform.position = Vector3.MoveTowards(transform.position, levitatingPosition, floatingSpeed * Time.deltaTime);
    }

    private void ThrowObject(Vector3 direction) {
        prop_rigidbody.AddForce(direction * throwForce);
    }

    private void TossObject(Vector3 direction) {
        prop_rigidbody.AddForce(direction * tossForce);
    }

    private Vector3 GetClearThrowingDirection() {
        float randomThrowY = transform.position.y + Random.Range(1, verticalThrowForce);
        
        for (int i = 0; i < 30; i++) {
            float randomThrowZ = transform.position.z + Random.Range(-throwRange, throwRange);
            float randomThrowX = transform.position.x + Random.Range(-throwRange, throwRange);
            Vector3 throwingDirection = new(randomThrowX, randomThrowY, randomThrowZ);
            bool canThrow = !Physics.Linecast(transform.position, throwingDirection);

            if(canThrow) {
                return throwingDirection;
            }
        }
        return transform.position;
    }

    private Vector3 GetClearTossDirection() {
        for (int i = 0; i < 30; i++) {
            float randomTossZ = transform.position.z + Random.Range(-tossRange, tossRange);
            float randomTossX = transform.position.x + Random.Range(-tossRange, tossRange);
            Vector3 tossDirection = new(randomTossX, transform.position.y, randomTossZ);
            bool canThrow = !Physics.Linecast(transform.position, tossDirection);

            if (canThrow) {
                return tossDirection;
            }
        }
        return transform.position;
    }

    public void ToggleEventTrigger() {
        eventTrigger = !eventTrigger;
    }

    public IEnumerator GravityDisableRoutine() {
        prop_rigidbody.useGravity = false;
        yield return new WaitForSeconds(floatingTime); //You may change this number of seconds
        prop_rigidbody.useGravity = true;
    }

}
