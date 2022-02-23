using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    public class SkillUtils
    {
        /// <summary>
        /// 设置特效速度
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="speed"></param>
        public static void SetEffectSpeed(GameObject prefab, float speed)
        {
            // Animator
            Animator[] animators = prefab.GetComponentsInChildren<Animator>();
            for (int i=0; i<animators.Length; ++i)
            {
                animators[i].speed = speed;
            }

            // Animation
            Animation[] animations = prefab.GetComponentsInChildren<Animation>();
            for (int i = 0; i < animations.Length; ++i)
            {
                foreach (AnimationState state in animations[i])
                {
                    state.speed = speed;
                }
            }

            // ParticleSystem
            ParticleSystem[] particles = prefab.GetComponentsInChildren<ParticleSystem>();
            ParticleSystem.MainModule mainModule;
            for (int i = 0; i < particles.Length; ++i)
            {
                mainModule = particles[i].main;
                mainModule.simulationSpeed = speed;
            }
        }
        
        
    };
    

}


