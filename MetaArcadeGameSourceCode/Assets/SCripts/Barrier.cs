using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public static Action<GameObject> OnBarrierDisabled;
    public float speedMultiplier;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.gameObject.activeInHierarchy)
        {
            if (RaceObjectPool.isRaceOn)
            {
                rb.MovePosition(rb.position +  Vector3.back * Time.fixedDeltaTime * speedMultiplier * RaceObjectPool.Instance.speed);
            }


            if (this.transform.localPosition.z < -30)
            {
                OnBarrierDisabled?.Invoke(this.gameObject);

                this.gameObject.SetActive(false);
            }

        }
    }

}
