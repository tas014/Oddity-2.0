using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationCollider : MonoBehaviour
{
    [SerializeField] private GameObject[] roomObjects;
    [SerializeField] private ParanormalEventHandler paranormalEventHandler;

    private void OnTriggerEnter(Collider other) {
        if (roomObjects.Length != 0) {
            paranormalEventHandler.availableProps = roomObjects;
            Debug.Log("Updated room Objects!");
        } else {
            paranormalEventHandler.availableProps = null;
        }
    }
    private void OnTriggerExit(Collider other) {
        Debug.Log("Player has left " + this.name + " trigger");
    }
}
