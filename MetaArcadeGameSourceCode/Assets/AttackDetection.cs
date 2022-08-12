using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetection : MonoBehaviour
{
    [SerializeField] PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pv.IsMine && other.CompareTag("Player"))
        {
            if ( other.TryGetComponent<PhotonView>(out PhotonView p_view))
            {
                if (pv.Owner != p_view.Owner)
                {
                   
                    this.gameObject.SetActive(false);
                    pv.RPC("AttackRecieved", p_view.Owner, pv.Owner.UserId);
                }
            }
        }
    }

   

}
