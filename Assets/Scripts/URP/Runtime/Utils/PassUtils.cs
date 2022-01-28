using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zack.UniversalRP.PostProcessing
{
    public class PassUtils
    {
        /// <summary>
        /// ��������ĳ���ؼ���
        /// </summary>
        /// <param name="material"></param>
        /// <param name="keywords">Shader�ؼ����б���Ҫע���һ���ؼ��ֻ���"_"���棬���Բ���keywords�����С�Ҫ������һ��ֻҪDisable��������Keyword����</param>
        /// <param name="index"></param>
        public static void EnableKeyword(Material material, string[] keywords, int index)
        {
            for(int i=0; i<keywords.Length; ++i)
            {
                material.DisableKeyword(keywords[i]);
            }
            if (index > 0)
            {
                material.EnableKeyword(keywords[index - 1]);
            }
        }

    }
}

