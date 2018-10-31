using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RegPanel : PanelBase
{
    private InputField idInput;
    private InputField pwInput;
    private Button regBtn;
    private Button closeBtn;
    private InputField repInput;

    #region 生命周期
    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RegPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        repInput = skinTrans.Find("RepInput").GetComponent<InputField>();

        regBtn.onClick.AddListener(OnRegClick);
        closeBtn.onClick.AddListener(OnCloseClick);
    }
    #endregion


    public void OnCloseClick()
    {
        PanelMgr.instance.OpenPanel<LoginPanel>("");
        Close();
    }

    public void OnRegClick()
    {
        //用户名密码为空
        if (idInput.text == "" || pwInput.text == "")
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "用户名密码不能为空!");
            return;
        }
        //两次密码不同
        if (pwInput.text != repInput.text)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "两次输入的密码不同！");
            return;
        }

    }

}