using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zack.Editor;
using System.ComponentModel;
using Skill;
using UnityEditor.Animations;

namespace AnimationEventEditor
{
   public partial class AnimationEventEditor
   {
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

       // 遍历获取animator的所有AnimatorState信息
       void browserAnimatorStates(Animator animator, out string[] stateNames, out int[] layers, out string[] showStateNames)
       {
            List<string> stateNameVec = new List<string>();
            List<int> layerVec = new List<int>();
            List<string> showStateNameVec = new List<string>();

            if (animator)
            {
                AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
                AnimatorOverrideController overrideController = null;
                if (!controller)
                {
                    // 使用AnimatorOverrideController
                    overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                    controller = overrideController.runtimeAnimatorController as AnimatorController;
                }

                for (int layer = 0; layer < controller.layers.Length; ++layer)
                {
                    ChildAnimatorState[] childAnimatorStates = controller.layers[layer].stateMachine.states;
                    for (int i = 0; i < childAnimatorStates.Length; ++i)
                    {
                        AnimatorState animatorState = childAnimatorStates[i].state;
                        stateNameVec.Add(animatorState.name);
                        layerVec.Add(layer);
                        showStateNameVec.Add($"{animatorState.name} ({layer})");
                    }
                }
            }

           stateNames = stateNameVec.ToArray();
           layers = layerVec.ToArray();
           showStateNames = showStateNameVec.ToArray();
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
            string stateName = this._SelectedState;
            AnimationClip clip = getAnimationClip(stateName);
            this._AnimationClip = clip;
            this._TotalFrames = getAnimationStateFrames(clip);
            this._AnimationEvents = AnimationEventEditorUtils.GetAnimationEvents(clip);
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

