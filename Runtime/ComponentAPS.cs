using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static APS.Runtime.WindowAPS;

namespace APS.Runtime
{
    public class ComponentAPS: MonoBehaviour
    {
        [SerializeField] private AnimatorController customAnimatorController;
        [SerializeField] private Animator avatarAnimator;
        [SerializeField] private string nameClip;
        [HideInInspector]
        public int selectedFrame = 0;
        [HideInInspector]
        public int prevSelectedFrame = 0;
        [HideInInspector]
        public int allFrames = 0;
        private bool componentInited;
        [HideInInspector]
        public string pathPlugin = "";
        
        private bool FirstInit()
        {
            var assemblyPath = FindScriptPath("WindowAPS.cs");
            
            if (assemblyPath == null)
            {
                ErrorLog(SharedAPS.LogErrorFindPath);
                componentInited = false;
                return false;
            }
            
            pathPlugin = assemblyPath;
            componentInited = true;
            return true;
        }
        
        private void Start()
        {
            if (!FirstInit())
            {
                return;
            }
            string controllerPath = pathPlugin + "/Temp/customAPSAnim.controller";

            customAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

            avatarAnimator = this.gameObject.GetComponent<Animator>();

            avatarAnimator.runtimeAnimatorController = customAnimatorController;

            var listAnimationClips = customAnimatorController.animationClips;
            
            nameClip = listAnimationClips[0].name;
        }

        private void Update()
        {
            if (!componentInited) { return; }
            if (avatarAnimator != null && !string.IsNullOrEmpty(nameClip) && (prevSelectedFrame != selectedFrame) && (allFrames != 0))
            {
                float normalizedTime = (float)selectedFrame / allFrames;
                SetAnimationFrame(nameClip, normalizedTime);
                prevSelectedFrame = selectedFrame;
            }
        }
        
        private void SetAnimationFrame(string animationName, float normalizedTime)
        {
            if (!componentInited) { return; }
            avatarAnimator.Play(animationName, 0, normalizedTime);
            avatarAnimator.Update(0);
            avatarAnimator.speed = 0f;
        }
    }
}