using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HpHolder : MonoBehaviour
{
    Transform HpBar;
    Text HpValueObj;

    Text pointHolder;

    [HideInInspector] public int EnemyKilled = 0;
    float HP = 1;

    GameObject StealthSkill;
    GameObject DashSkill;
    GameObject HpInfo;
    GameObject HpDisplay;
    GameObject Anchor;
    GameObject Canvas;
    GameObject AutoAttack;
    GameObject Anchor2;
    GameObject MiniMap;
    GameObject KillCounter;
    GameObject Anchor3;

    float ratio;

    void Start()
    {
        //For it to be compatible with other resulations
        {
            Canvas = GameObject.Find("Canvas");

            MiniMap = Canvas.transform.Find("Map").gameObject;
            StealthSkill = Canvas.transform.Find("StealthSkill").gameObject;
            DashSkill = Canvas.transform.Find("DashSkill").gameObject;
            HpDisplay = Canvas.transform.Find("HpDisplay").gameObject;
            HpInfo = Canvas.transform.Find("HpInfo").gameObject;
            Anchor = Canvas.transform.Find("GameObject").gameObject;
            Anchor2 = Canvas.transform.Find("Anchor2").gameObject;
            Anchor3 = Canvas.transform.Find("Anchor3").gameObject;
            AutoAttack = Canvas.transform.Find("AutoAttack").gameObject;
            KillCounter = Canvas.transform.Find("KillCounter").gameObject;

            float ratioHorizontal = Screen.width / 1920f;
            float ratioVertical = Screen.height / 1080f;
            ratio = (ratioHorizontal + ratioVertical) / 2;

            //Sets position according to Ratio, referanced to AnchorX   
            float AnchorX = Anchor.transform.position.x;
            PositionCorrectorForX(AutoAttack.transform, ratio, AnchorX);
            PositionCorrectorForX(StealthSkill.transform, ratio, AnchorX);
            PositionCorrectorForX(DashSkill.transform, ratio, AnchorX);
            PositionCorrectorForX(HpInfo.transform, ratio, AnchorX);
            PositionCorrectorForX(HpDisplay.transform, ratio, AnchorX);
            PositionCorrector(MiniMap.transform, ratio, Anchor2.transform.position);
            PositionCorrector(KillCounter.transform, ratio, Anchor3.transform.position);

            //Sets scale according to Ratio
            ScaleCorrector(StealthSkill.transform, ratio);
            ScaleCorrector(DashSkill.transform, ratio);
            ScaleCorrector(HpInfo.transform, ratio);
            ScaleCorrector(HpDisplay.transform, ratio);
            ScaleCorrector(AutoAttack.transform, ratio);
            ScaleCorrector(MiniMap.transform, ratio);
            ScaleCorrector(KillCounter.transform, ratio);
        }

        HpBar = Canvas.transform.Find("HpDisplay");
        HpValueObj = HpInfo.gameObject.GetComponent<Text>();
    }

    public void ChangeHp(float ChangeValue)
    {
        HP += ChangeValue;

        //Update HpValue
        {
            HpBar.localScale = new Vector3(ratio * HP, ratio, ratio);

            HpValueObj.text = (HP * 100).ToString("f0");

            DashSkill.transform.position += new Vector3(ratio * ChangeValue * 250, 0, 0);
            StealthSkill.transform.position += new Vector3(ratio * ChangeValue * 250, 0, 0);

            AutoAttack.transform.position += new Vector3(ratio * ChangeValue * -250, 0, 0);

            if (HP <= 0)
            {
                HP = 0;
                ActivateDeathScreen(EnemyKilled);
            }
        }
    }

    void ActivateDeathScreen(int score)
    {
        Time.timeScale = 0;
        pointHolder.text = score + " POINTS";
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("SampleScene 1");
    }

    void PositionCorrectorForX(Transform ObjToPosition, float ratio, float AnchorX)
    {
        ObjToPosition.position = new Vector3(AnchorX + (ObjToPosition.position.x - AnchorX) * ratio, ObjToPosition.position.y * ratio, 0);
    }

    void PositionCorrector(Transform ObjToPosition, float ratio, Vector2 Anchor)
    {
        ObjToPosition.position = new Vector3(Anchor.x + (ObjToPosition.position.x - Anchor.x) * ratio,
            Anchor.y + (ObjToPosition.position.y - Anchor.y) * ratio, 0);
    }

    void ScaleCorrector(Transform objToScale, float ratio)
    {
        objToScale.localScale = objToScale.localScale * ratio;
    }
}
