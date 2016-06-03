using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common.Event;
using Common.Global;
using BT;
using System;

public class PanelMain : BasePanel
{
    private GameObject commonAttack;
    private RectTransform commonAttackRectTrans;

    private GameObject skill_1;
    private GameObject skill_2;

    private Image cdImgSkill_1;
    private Image cdImgSkill_2;

    private Text cdTextSkill_1;
    private Text cdTextSkill_2;

    private int cdTimeSkill_1 = 10;
    private int cdTimeSkill_2 = 15;

    private bool skill_1Cooled = true;
    private bool skill_2Cooled = true;

    //
    private float skill_1Length = 1.833f;
    private float skill_2Length = 1.292f;

    public PanelMain()
    {
        SetPanelPrefabPath(ABConfig.PANEL_Main);
    }

    public override void Init()
    {
        //这里获取组件
        commonAttack = m_tran.Find("AttackArea/CommonAttack").gameObject;
        commonAttackRectTrans = commonAttack.GetComponent<RectTransform>();

        skill_1 = m_tran.Find("AttackArea/Skill_Showoff").gameObject;
        skill_2 = m_tran.Find("AttackArea/Skill_spin").gameObject;

        cdImgSkill_1 = m_tran.Find("AttackArea/Skill_Showoff/ShowoffCD").GetComponent<Image>();
        cdImgSkill_2 = m_tran.Find("AttackArea/Skill_spin/SpinCD").GetComponent<Image>();

        cdTextSkill_1 = m_tran.Find("AttackArea/Skill_Showoff/ShowoffCDTime").GetComponent<Text>();
        cdTextSkill_2 = m_tran.Find("AttackArea/Skill_spin/SpinCDTime").GetComponent<Text>();
    }

    public override void InitEvent()
    {
        //这里初始化事件
        UGUIClickHandler.Get(commonAttack).onPointerClick += delegate { onClickCommonAttackBtn(); };
        UGUIClickHandler.Get(skill_1).onPointerClick += delegate { onClickSkill_1Btn(); };
        UGUIClickHandler.Get(skill_2).onPointerClick += delegate { onClickSkill_2Btn(); };

        onAtkFinished = this.onAttackFinished;
        onSkl_1Finished = this.onSkill_1Finished;
    }

    public override void OnShow()
    {
        //直接在easytouch中处理了，优先响应UI点击
        //攻击按钮屏蔽easytouch响应
//         Vector2 pos = RectTransformUtility.WorldToScreenPoint(UIManager.UICamera, attackAreaTrans.position);
//         Rect caRect = new Rect(pos.x, pos.y, attackAreaTrans.sizeDelta.x, attackAreaTrans.sizeDelta.y);
// 
//         float rate = (float)Screen.width / (float)Screen.currentResolution.width;
//         Vector2 size = attackAreaTrans.sizeDelta * rate;
//         Rect rect = new Rect(pos.x - size.x / 2, pos.y - size.y / 2, size.x, size.y);
// 
//         Debug.Log(attackAreaTrans.position + " ;  " + pos + " ; " + caRect);
//         EasyTouch.AddReservedArea(rect);

        clearWidget();
    }

    private void clearWidget()
    {
        cdImgSkill_1.fillAmount = 0;
        cdImgSkill_2.fillAmount = 0;

        cdTextSkill_1.text = string.Empty;
        cdTextSkill_2.text = string.Empty;
    }

    private void onClickCommonAttackBtn()
    {
        EventDispatcher.TriggerEvent(BTreeEventConfig.OnAttack, BTCheckAttackOpt.Common, onAtkFinished);
    }

    
    private void onClickSkill_1Btn()
    {
        if (skill_1Cooled)
        {
            skill_1Cooled = false;
            cdImgSkill_1.fillAmount = 1f;

            GlobalDelegate.Instance.View.StartCoroutine(countDown(cdTimeSkill_1,cdImgSkill_1, cdTextSkill_1, () => { skill_1Cooled = true; }));
            EventDispatcher.TriggerEvent(BTreeEventConfig.OnAttack, BTCheckAttackOpt.Skill_1, onAtkFinished);
        }
    }

    
    private void onClickSkill_2Btn()
    {
        if (skill_2Cooled)
        {
            skill_2Cooled = false;
            cdImgSkill_2.fillAmount = 1f;

            GlobalDelegate.Instance.View.StartCoroutine(countDown(cdTimeSkill_2,cdImgSkill_2, cdTextSkill_2, () => { skill_2Cooled = true; }));
            EventDispatcher.TriggerEvent(BTreeEventConfig.OnAttack, BTCheckAttackOpt.Skill_2, onAtkFinished);
        }
    }

    private Action onAtkFinished;
    private Action onSkl_1Finished;
    
    private void onAttackFinished()
    {
        Debug.Log("PanelMain onAttackFinished...");
    }

    private void onSkill_1Finished()
    {
        Debug.Log("PanelMain onSkill_1Finished...");
    }

    public override void OnBack()
    {

    }

    public override void OnHide()
    {
    }

    public override void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }

    public override void Update()
    {

    }

    private IEnumerator countDown(int times,Image img, Text timeText,Action callback = null)
    {
        timeText.gameObject.SetActive(true);
        timeText.text = times.ToString();
        int totaltime = times;

        while (times > 0)
        {
            for (float timer = 0; timer < 1; timer += Time.deltaTime)
            {
                timeText.text = times.ToString();
                img.fillAmount = ((float)times - timer) / (float)totaltime;

                yield return 0;
            }
                
            //yield return new WaitForSeconds(1f);

            --times;
        }

        timeText.gameObject.SetActive(false);
        img.fillAmount = 0;

        if (callback != null)
        {
            callback();
        }
    }
}
