using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace PlayAnimationKat
{
    public class ComponentAnimKatStarter: MonoBehaviour
    {
        [SerializeField] private AnimatorController customAnimatorController;
        [SerializeField] private Animator avatarAnimator;
        [SerializeField] private string nameClip;
        public int SelectedFrame = 0;
        public int PrevSelectedFrame = 0;
        public int AllFrames = 0;
        private void Start()
        {
            string controllerPath = "Assets/PlayAnimationKat/Temp/customACKAnim.controller";

            customAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

            avatarAnimator = this.gameObject.GetComponent<Animator>();

            avatarAnimator.runtimeAnimatorController = customAnimatorController;

            var listAnimationClips = customAnimatorController.animationClips;
            
            nameClip = listAnimationClips[0].name;
        }

        private void Update()
        {
            if (avatarAnimator != null && !string.IsNullOrEmpty(nameClip) && (PrevSelectedFrame != SelectedFrame) && (AllFrames != 0))
            {
                float normalizedTime = (float)SelectedFrame / AllFrames;
                SetAnimationFrame(nameClip, normalizedTime);
                PrevSelectedFrame = SelectedFrame;
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