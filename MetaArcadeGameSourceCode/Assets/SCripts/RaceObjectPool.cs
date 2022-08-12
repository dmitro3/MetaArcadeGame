using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceObjectPool : MonoBehaviour
{
    #region Singleton
    public static RaceObjectPool Instance;
    void Awake()
    {
        Instance = this;
    }
    #endregion

    [SerializeField] List<GameObject> barriersInGame=new List<GameObject>();
    [SerializeField] List<GameObject> InActivePlatform = new List<GameObject>();


    public static bool isRaceOn=false;

    
    public float[] offsetsOfGeneration;
    public float speed;

    
    [Header("StartData")]
    [SerializeField] List<GameObject> start_barriers_in_list=new List<GameObject>();
    [SerializeField] List<GameObject> start_InActivebarriers_in_list=new List<GameObject>();
    [SerializeField] List<Vector3> start_positions=new List<Vector3>();
    [SerializeField] Vector3 non_activePosition;

    [Header("Material Scroll")]
    [SerializeField] Material[] RoadMaterials;
    [SerializeField] SideObject[] SideObjects;
    [SerializeField] MeshRenderer road;


    public static Action OnRaceStarted;



    void Start()
    {
        start_barriers_in_list = barriersInGame;
        start_InActivebarriers_in_list = InActivePlatform;
        for (int i = 0; i < start_barriers_in_list.Count; i++)
        {
            start_positions.Add(start_barriers_in_list[i].transform.localPosition);
        }
        non_activePosition = start_InActivebarriers_in_list[0].transform.localPosition;

    }

    public void ResetRace()
    {
        barriersInGame = start_barriers_in_list;
        InActivePlatform = start_InActivebarriers_in_list;
        for(int i=0; i<barriersInGame.Count; i++)
        {
            barriersInGame[i].transform.localPosition = start_positions[i];
        }
        for (int i = 0; i < InActivePlatform.Count; i++)
        {
            InActivePlatform[i].transform.localPosition = non_activePosition;
        }

    }
    // Start is called before the first frame update
    private void OnEnable()
    {
        Barrier.OnBarrierDisabled += OnBarrierGotDisabled;
    }   

    private void OnDisable()
    {
        Barrier.OnBarrierDisabled -= OnBarrierGotDisabled;
    }

    public void SetMaterials(int roadMaterial)
    {
        road.material = RoadMaterials[roadMaterial];

       /* for (int i = 0; i < SideObjects.Length; i++)
        {
            SideObjects[i].gameObject.SetActive(false);
        }
        SideObjects[sideObjectIndex].gameObject.SetActive(true);*/
    }
    public void StartRace()
    {
        OnRaceStarted?.Invoke();

        isRaceOn = true;
       
    }

    private void FixedUpdate()
    {
        if (isRaceOn)
        {
            speed *= 1.0005f;
        }
        else
        {
            speed = 1;
        }
    }


    int loop_index=0;
    private void OnBarrierGotDisabled(GameObject obj)
    {
        if (barriersInGame.Contains(obj))
        {
            barriersInGame.Remove(obj);
            InActivePlatform.Add(obj);
        }
        ActivateNewBarrier();
    }

    


    public void ActivateNewBarrier()
    {
        GameObject inactive = GetInactiveBarrier();
        if (inactive != null)
        {
            loop_index++;
            if (loop_index >= offsetsOfGeneration.Length)
            {
                loop_index = 0;
            }
            barriersInGame.Add(inactive);
            inactive.transform.position = new Vector3(inactive.transform.position.x, inactive.transform.position.y,200 + offsetsOfGeneration[loop_index]);
            inactive.SetActive(true);
        }
    }
    public GameObject GetInactiveBarrier()
    {
        GameObject inactivePlatform = InActivePlatform[0];
        InActivePlatform.RemoveAt(0);

        return inactivePlatform;
    }

   
}
