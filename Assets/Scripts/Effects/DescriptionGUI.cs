using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionGUI : MonoBehaviour
{
    public Color _Color = Color.blue;
    public string[] _Descriptions;
    
    private GUIStyle k_Label;
    void Awake()
    {
        k_Label = new GUIStyle("label");
        k_Label.fontSize = 20;
        k_Label.normal.textColor = _Color;
        k_Label.hover.textColor = _Color;
        k_Label.alignment = TextAnchor.LowerLeft;
        k_Label.stretchHeight = true;
        k_Label.margin.top = 0;
        k_Label.margin.bottom = 0;
    }
    
    
    void OnGUI()
    {
        if (this._Descriptions == null || this._Descriptions.Length <= 0)
        {
            return;
        }

        float y = 0;
        for (int i = 0; i < this._Descriptions.Length; ++i)
        {
            addPropertyGUI(this._Descriptions[i], ref y);
        }
    }

    private float margin = 10;
    private float height = 30;
    private float width = Screen.width;
    void addPropertyGUI(string content, ref float y)
    {
        GUI.Label(new Rect(margin, y, width, height), content, k_Label);
        y += height;
    }
}
