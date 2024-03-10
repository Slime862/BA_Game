using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ba
{
    public class Sensei : MonoBehaviour, IItem
    {
        Transform pickedPoint = null;
        private bool canPicked = true;
        private Rigidbody rb;
        public float throwForce = 3f;
        private SphereCollider sphereCollider;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            sphereCollider = GetComponentInChildren<SphereCollider>();
        }
        public bool Picked(GameObject player)
        {
            if (!canPicked) return false;
            sphereCollider.enabled = false;
            pickedPoint = player.GetComponent<GamePlayerController>().pickPoint;
            rb.useGravity = false;
            return true;
        }

        public void Used(GameObject player)
        {
            Debug.Log("把老师丢出去");
            pickedPoint = null;
            transform.position = player.GetComponent<GamePlayerController>().throwPoint.transform.position;
            sphereCollider.enabled = true;
            rb.useGravity = true;
            rb.AddForce(throwForce * player.transform.forward, ForceMode.Impulse);

        }

        private void Update()
        {
            if (pickedPoint == null) return;
            this.transform.position = pickedPoint.position;
            transform.rotation = pickedPoint.rotation;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Ground")&&!canPicked)
            {
                canPicked = true;
                rb.velocity = Vector3.zero;
            }
        }

    }

}
