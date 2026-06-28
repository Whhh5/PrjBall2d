using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIKeyCodeDownButtonClick : MonoBehaviour
{
    private Button m_OnClickBtn = null;
    [SerializeField]
    private KeyCode m_KeyCode = KeyCode.None;
    protected void Awake()
    {
        m_OnClickBtn = GetComponent<Button>();
    }
    protected void OnEnable()
    {
        if (m_KeyCode != KeyCode.None)
            ABBInputMgr.Instance.AddListanerDown(m_KeyCode, OnClick_KeyCodeDown);
    }
    protected void OnDisable()
    {
        if (m_KeyCode != KeyCode.None)
            ABBInputMgr.Instance.RemoveListanerDown(m_KeyCode, OnClick_KeyCodeDown);
    }
    private void OnClick_KeyCodeDown()
    {
        m_OnClickBtn.onClick.Invoke();
    }
}
