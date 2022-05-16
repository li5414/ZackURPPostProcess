using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AnimationEventEditor
{
    public class AnimationEventEditorUtils : MonoBehaviour
    {
        public static void AddAnimationEvent(AnimationClip clip, AnimationEvent evt)
        {
            AnimationEvent[] originEvents = GetAnimationEvents(clip);

            AnimationEvent[] events = new AnimationEvent[originEvents.Length + 1];
            int i = 0;
            for (; i < originEvents.Length; ++i)
            {
                events[i] = originEvents[i];
            }
            events[i] = evt;

            SetAnimationEvents(clip, events);
        }

        public static void RemoveAnimationEvents(AnimationClip clip, AnimationEvent[] originEvents, int index)
        {
            AnimationEvent[] events = new AnimationEvent[originEvents.Length-1];
            for (int i = 0, j = 0; i < originEvents.Length; ++i)
            {
                if (i != index)
                {
                    events[j] = originEvents[i];
                    ++j;
                }
            }
            SetAnimationEvents(clip, events);
        }

        public static AnimationEvent[] GetAnimationEvents(AnimationClip clip)
        {
            return AnimationUtility.GetAnimationEvents(clip);
        }

        public static void SetAnimationEvents(AnimationClip clip, AnimationEvent[] events)
        {
            ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;
            if (modelImporter == null)
                return;

            SerializedObject serializedObject = new SerializedObject(modelImporter);
            SerializedProperty clipAnimations = serializedObject.FindProperty("m_ClipAnimations");

            if (!clipAnimations.isArray)
                return;

            for (int i = 0; i < clipAnimations.arraySize; i++)
            {
                AnimationClipInfoProperties clipInfoProperties = new AnimationClipInfoProperties(clipAnimations.GetArrayElementAtIndex(i));
                if (clipInfoProperties.name == clip.name)
                {
                    clipInfoProperties.SetEvents(clip, events);
                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(clip));
                    break;
                }
            }
        }
        
    }
}

