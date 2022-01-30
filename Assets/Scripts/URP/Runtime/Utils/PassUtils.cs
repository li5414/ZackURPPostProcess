using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zack.UniversalRP.PostProcessing
{
    public class PassUtils
    {
        /// <summary>
        /// 开启材质某个关键字
        /// </summary>
        /// <param name="material"></param>
        /// <param name="keywords">Shader关键字列表，但要注意第一个关键字会用"_"代替，所以不在keywords数组中。要开启第一个只要Disable其他所有Keyword即可</param>
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

