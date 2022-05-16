using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zack.Editor;
using System.ComponentModel;
using Skill;

namespace AnimationEventEditor
{
   public partial class AnimationEventEditor
   {
       /// <summary>
       /// 动画事件编辑器支持的动画状态
       /// </summary>
       public enum AEEditorAnimatorState
       {
           [Description("Attack1"), AnimatorLayer(0)]
           Attack1 = 0,
           [Description("Attack2"), AnimatorLayer(0)]
           Attack2,
       }

       public enum AEEditorActionType
       {
           [Description("音效事件")]
           AddSound = 0,
       }
       
       /// <summary>
       /// 遍历人物
       /// </summary>
       /// <returns></returns>
       string[] browserCharacters()
       {
           string folderPath = Parameters.k_CharacterPrefabFilePath;
           
           List<string> folders = new List<string>();
           foreach (string path in Directory.GetDirectories(folderPath))
           {
               folders.Add(Path.GetFileName(path));
           }
           return folders.ToArray();
       }

       int getSelectedIndex(string id, string[] ids)
       {
           int index = -1;
           if (id != string.Empty && ids != null)
           {
               for (int i = 0; i < ids.Length; ++i)
               {
                   if (id == ids[i])
                   {
                       index = i;
                       break;
                   }
               }
           }

           return index;
       }
       
        // 更新动画帧数
        void UpdateAnimationInfo()
        {
            string stateName = this._SelectedState.GetDescription();
            AnimationClip clip = getAnimationClip(stateName);
            this._AnimationClip = clip;
            this._TotalFrames = getAnimationStateFrames(clip);
            this._AnimationEvents = AnimationEventEditorUtils.GetAnimationEvents(clip);
        }
        
        // 根据帧数获取动画状态
        string getAnimationStateName(int frame)
        {
            return this._SelectedState.GetDescription();
        }

        // 根据帧数获取动画layer
        int getAnimationStateLayer(int frame)
        {
            return EditorUtils.GetAnimatorLayer(this._SelectedState);
        }

        // 添加Action
        public void addAnimationEventAction(AEEditorActionType actionType)
        {
            switch (actionType)
            {
            case AEEditorActionType.AddSound:
                {
                    // 添加音效事件
                    AnimationEvent evt = new AnimationEvent();
                    evt.time = this._AnimationClip.length * 0.5f;   // TODO
                    evt.functionName = "OnSoundEvent";  // TODO
                    evt.stringParameter = "attacksound";    // TODO
                    AnimationEventEditorUtils.AddAnimationEvent(this._AnimationClip, evt);
                    UpdateAnimationInfo();
                }
                break;
            }
        }
      
   }
   

}

