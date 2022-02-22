using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils
{
    public class IdentityGenerator
    {
        private int _IdGenerator = 0;

        public IdentityGenerator()
        {
            this._IdGenerator = 0;
        }
        
        /// <summary>
        /// 生成事件id
        /// </summary>
        /// <returns></returns>
        public int GenerateId()
        {
            if (this._IdGenerator >= int.MaxValue)
            {
                this._IdGenerator = 0;
            }
            return ++this._IdGenerator;
        }
    }

}

