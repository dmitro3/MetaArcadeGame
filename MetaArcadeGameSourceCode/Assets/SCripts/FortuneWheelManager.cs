using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyUI.PickerWheelUI;
public class FortuneWheelManager : MonoBehaviour
{
    [SerializeField] PickerWheel wheel;
    [SerializeField] GameObject spin_btn;    
    [SerializeField] TMPro.TextMeshProUGUI txt_result;
    [SerializeField] GameObject PopupMessage;
    [SerializeField] UnityEngine.UI.Image Got_Prize_Icon;
    [SerializeField] TMPro.TextMeshProUGUI Got_Prize_Name;
    [SerializeField] GameObject ClaimBTN;
    [SerializeField] int prizeIndex;

    // Start is called before the first frame update
    void Start()
    {
        wheel.OnSpinStart(() =>
        {
            spin_btn.SetActive(false);
            Debug.Log("Spin Started");
        });
        wheel.OnSpinEnd((WheelPiece piece) =>
        {
            prizeIndex = piece.Index;                    
            DatabaseManager.Instance.UpdateSpinData();
            txt_result.text = piece.Label;

            ClaimBTN.SetActive(true);
            Got_Prize_Icon.sprite = piece.Icon;
            Got_Prize_Name.text = piece.Label;

            if (prizeIndex == 2)
            {
                ClaimBTN.SetActive(false);
                ClaimPrize();
               
            }
            else
            {
                LeanTween.scale(PopupMessage, Vector3.one, 0.5f).setEase(LeanTweenType.easeInQuad);
            }
            


        });
        
    }
    public void ClaimPrize()
    {
        ClaimBTN.SetActive(false);

        PlayerController my_controller = MetaManager.insta.myPlayer.GetComponent<PlayerController>();
        //TODO - ADD TO DATABASE
        
        


        Debug.Log(prizeIndex + " :" +wheel.wheelPieces[prizeIndex].Label);
        LeanTween.scale(PopupMessage, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInQuad).setOnComplete(()=> {
            MetaManager.insta.ToggleSpinUI(false);
        });
        my_controller.ClaimPrize(prizeIndex);
    }

    public void Spin()
    {
        wheel.Spin();
    }

   
}
