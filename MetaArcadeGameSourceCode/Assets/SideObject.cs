using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideObject : MonoBehaviour
{
    public bool isMaterialObject = false;
    public Material material;
    public Material material2;
    public float speedMultiplier;
    public bool isWall = false;
    public bool isTree = false;
    private void OnEnable()
    {
        RaceObjectPool.OnRaceStarted += onRaceStart;
    }

    private void OnDisable()
    {
        RaceObjectPool.OnRaceStarted -= onRaceStart;
    }

    private void onRaceStart()
    {
        if (isMaterialObject)
        {
            if (isWall)
            {
                material = this.transform.GetChild(0).GetComponent<MeshRenderer>().material;
                material2 = this.transform.GetChild(1).GetComponent<MeshRenderer>().material;
                return;
            }
            material = this.GetComponent<MeshRenderer>().material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!RaceObjectPool.isRaceOn) return;

        if (isMaterialObject && material != null)
        {

            material.mainTextureOffset += RaceObjectPool.Instance.speed * speedMultiplier * Vector2.right * Time.deltaTime;
            if (material2 != null)
            {
                material2.mainTextureOffset += RaceObjectPool.Instance.speed * speedMultiplier * Vector2.right * Time.deltaTime;
            }
        }

        if (isTree)
        {
            this.transform.Translate(Vector3.back * speedMultiplier * RaceObjectPool.Instance.speed * Time.deltaTime);
            if (this.transform.position.z < -30)
            {
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 160);
            }

        }
    }
}