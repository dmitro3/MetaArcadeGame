using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{

    [SerializeField] GameObject authObj;
    [SerializeField] GameObject LoadingPanel;
    [SerializeField] GameObject playBTN;
    // Start is called before the first frame update
    void Start()
    {
        authObj.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayGame()
    {
        playBTN.SetActive(false);
        LoadingPanel.SetActive(true);
        SceneManager.LoadScene("MetaArcade");
    }
}
