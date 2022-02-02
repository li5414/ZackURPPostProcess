using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private Renderer m_Renderer;
    static MaterialPropertyBlock m_PropertyBlock;
    const string k_BoundsName = "_Bounds";

    void Awake()
    {
        if (m_PropertyBlock == null)
        {
            m_PropertyBlock = new MaterialPropertyBlock();
        }
        m_Renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Renderer.GetPropertyBlock(m_PropertyBlock);
        m_PropertyBlock.SetVector(k_BoundsName, m_Renderer.bounds.size);
        m_Renderer.SetPropertyBlock(m_PropertyBlock);
    }
}
