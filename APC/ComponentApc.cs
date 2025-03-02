using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace APC
{
    public class ComponentApc: MonoBehaviour
    {
        [SerializeField] private AnimatorController customAnimatorController;
        [SerializeField] private Animator avatarAnimator;
        [SerializeField] private string nameClip;
        public int selectedFrame = 0;
        public int prevSelectedFrame = 0;
        public int allFrames = 0;
        private void Start()
        {
            string controllerPath = "Assets/APC/Temp/customAPCAnim.controller";

            customAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

            avatarAnimator = this.gameObject.GetComponent<Animator>();

            avatarAnimator.runtimeAnimatorController = customAnimatorController;

            var listAnimationClips = customAnimatorController.animationClips;
            
            nameClip = listAnimationClips[0].name;
        }

        private void Update()
        {
            if (avatarAnimator != null && !string.IsNullOrEmpty(nameClip) && (prevSelectedFrame != selectedFrame) && (allFrames != 0))
            {
                float normalizedTime = (float)selectedFrame / allFrames;
                SetAnimationFrame(nameClip, normalizedTime);
                prevSelectedFrame = selectedFrame;
            }
        }
        
        private void SetAnimationFrame(string animationName, float normalizedTime)
        {
            
            avatarAnimator.Play(animationName, 0, normalizedTime);
            avatarAnimator.Update(0);
            avatarAnimator.speed = 0f;
            
        }
    }
}