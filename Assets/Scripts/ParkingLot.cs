using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingLot : MonoBehaviour
{
    public bool IsOccupied { get; set; }
    public Vector3 Orientation => transform.forward;
    private Collider fullEndCollider;

    private void Awake()
    {
        fullEndCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("agent"))
        {
            if (fullEndCollider.bounds.Intersects(other.bounds))
            {
                if (!IsOccupied)
                {
                    float bonusfactor = 0.2f;
                    float alignment = Vector3.Dot(gameObject.transform.right,
                        other.gameObject.transform.up);
                    if (alignment > 0)
                        bonusfactor = 0.8f;
                    float bonus = bonusfactor * Mathf.Abs(alignment);
                    StartCoroutine( other.gameObject.transform.parent.GetComponent<AutoParkAgent>().JackpotReward(bonus));
                }
            }
        }
    }

}
