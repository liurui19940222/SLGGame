using Game.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameHudTurn {

    private GameObject m_GameObject;
    private Transform m_Transform;
    private Animator m_Animator;

    private Text m_TurnTextBack;
    private Text m_TurnText;


    public GameHudTurn(Transform trans)
    {
        m_Transform = trans;
        m_GameObject = trans.gameObject;
        m_Animator = m_Transform.GetComponent<Animator>();
        m_TurnText = m_Transform.Find("TurnText").GetComponent<Text>();
        m_TurnTextBack = m_Transform.Find("TurnTextBack").GetComponent<Text>();

        ShowTurn(false, ETurnType.System);
    }

    public void ShowTurn(bool show, ETurnType type)
    {
        m_GameObject.SetActive(show);
        if (!show)
            return;

        if (type == ETurnType.OwnSide)
            m_TurnText.text = m_TurnTextBack.text = "我方回合";
        else if (type == ETurnType.Friendly)
            m_TurnText.text = m_TurnTextBack.text = "友方回合";
        else if (type == ETurnType.Opposite)
            m_TurnText.text = m_TurnTextBack.text = "敌方回合";

        GlobalMono.Instance.StartCoroutine(DoHide());
    }

    private IEnumerator DoHide()
    {
        yield return new WaitForSeconds(1.8f);
        m_GameObject.SetActive(false);
    }
}
