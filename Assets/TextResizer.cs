using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextResizer : MonoBehaviour
{
    [AddComponentMenu("Layout/TMP Container Size Fitter")]

    [SerializeField]
    private TMPro.TextMeshProUGUI m_TMProUGUI;
    public TMPro.TextMeshProUGUI TextMeshPro
    {
        get
        {
            if (m_TMProUGUI == null && transform.GetComponentInChildren<TMPro.TextMeshProUGUI>())
            {
                m_TMProUGUI = transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                m_TMPRectTransform = m_TMProUGUI.rectTransform;
            }
            return m_TMProUGUI;
        }
    }

    protected RectTransform m_RectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = GetComponent<RectTransform>();
            }
            return m_RectTransform;
        }
    }

    protected RectTransform m_TMPRectTransform;
    public RectTransform TMPRectTransform { get { return m_TMPRectTransform; } }

    protected float m_PreferredHeight;
    public float PreferredHeight { get { return m_PreferredHeight; } }

    protected virtual void SetHeight()
    {
        if (TextMeshPro == null)
            return;

        m_PreferredHeight = TextMeshPro.preferredHeight;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, PreferredHeight);

        SetDirty();
    }

    protected virtual void OnEnable()
    {
        SetHeight();
    }

    protected virtual void Start()
    {
        SetHeight();
    }

    protected virtual void Update()
    {
        if (PreferredHeight != TextMeshPro.preferredHeight)
        {
            SetHeight();
        }
    }

    #region MarkLayoutForRebuild
    public virtual bool IsActive()
    {
        return isActiveAndEnabled;
    }

    protected void SetDirty()
    {
        if (!IsActive())
            return;

        UnityEngine.UI.LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }


    protected virtual void OnRectTransformDimensionsChange()
    {
        SetDirty();
    }


#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        SetDirty();
    }
#endif

    #endregion
}


