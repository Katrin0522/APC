using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace APS.Runtime
{
    public class WindowAPS: EditorWindow
    {
        private AnimationClip animationSource;
        private GameObject AvatarObj;
        private GameObject TempAvatar;
        private AnimationWindow animationWindow;
        private ComponentAPS _controllerAps;
        
        private int sliderValue = -1;
        private int prevSliderValue = 0;
        private int allFrames = 0;
        private int counterDublicate = 1; 
        
        private const int minValue = 0;

        private bool isInited = false;
        private bool isChange = false;
        private bool windowInited = false;
        
        
        private int[] buttonValues = new int[5];
        
        private bool[] buttonPressed = new bool[5];
        
        private const int buttonSize = 50;

        private string PathPlugin = "";
        
        
        [MenuItem("Tools/APS Tool")]
        private static void Init()
        {
            // var inspWndType = typeof(SceneView);
            var window = GetWindow<WindowAPS>(title:"APS Tool");
        }

        private bool FirstInit()
        {
            var assemblyPath = FindScriptPath(SharedAPS.ScriptForSearch);
            if (assemblyPath == null)
            {
                ErrorLog(SharedAPS.LogErrorFindPath);
                windowInited = false;
                return false;
            }
            else
            {
                PathPlugin = assemblyPath;
                if (!Directory.Exists(PathPlugin + "/Temp"))
                {
                    var folderTemp = Directory.CreateDirectory(PathPlugin + "/Temp");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Log(SharedAPS.LogCreatedTempFolder);
                }
                windowInited = true;
                return true;
            }
        }
        
        private void OnGUI()
        {
            if (!windowInited)
            {
                if (!FirstInit())
                {
                    GUILayout.Label (SharedAPS.LogErrorWindow, EditorStyles.boldLabel);
                    return;
                }
            }

            GUILayout.Space(5);
            GUILayout.Label (SharedAPS.LabelObjectAvatar, EditorStyles.boldLabel);
            AvatarObj = EditorGUILayout.ObjectField(AvatarObj, typeof(GameObject), true) as GameObject;
            GUILayout.Space(5);
            GUILayout.Label (SharedAPS.LabelAnimation, EditorStyles.boldLabel);
            animationSource = EditorGUILayout.ObjectField(animationSource, typeof(AnimationClip), false) as AnimationClip;
            GUILayout.Space(15);
            
            if (animationSource && AvatarObj)
            {
                EditorGUILayout.LabelField(SharedAPS.LabelWorkWithAnimation, EditorStyles.boldLabel);
                GUILayout.Space(5);

                allFrames = (int)animationSource.length * (int)animationSource.frameRate;
                sliderValue = (int) EditorGUILayout.Slider(SharedAPS.LabelTimelineAnimation, sliderValue, minValue, allFrames);
                GUILayout.Label($"{SharedAPS.LabelSelectedFrame}{sliderValue}");

                GUILayout.BeginHorizontal();
                for (int i = 0; i < 5; i++)
                {
                    if (buttonPressed[i])
                        GUI.backgroundColor = Color.green;
                    else
                        GUI.backgroundColor = Color.white;
                
                    if (GUILayout.Button(buttonValues[i].ToString(), GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                    {
                        if (isChange)
                        {
                            buttonPressed[i] = false;
                            buttonValues[i] = 0;
                        }
                        else
                        {
                            if (buttonPressed[i] == false)
                            {
                                buttonPressed[i] = true;
                                buttonValues[i] = sliderValue;
                            }
                            else
                            {
                                buttonPressed[i] = true;
                                sliderValue = buttonValues[i];
                            }
                        }
                    
                    }

                    GUI.backgroundColor = Color.white;
                }

                if (GUILayout.Button(SharedAPS.ButtonClear, GUILayout.Width(100), GUILayout.Height(buttonSize)))
                {
                    isChange = !isChange;
                }
            
                GUILayout.EndHorizontal();
                GUILayout.Label(SharedAPS.ButtonClearMode + (isChange ? SharedAPS.ButtonClearModeOn : SharedAPS.ButtonClearModeOff));
                
                
                if (prevSliderValue != sliderValue)
                {
                    prevSliderValue = sliderValue;
                    if (EditorApplication.isPlaying)
                    {
                        if (_controllerAps)
                        {
                            _controllerAps.selectedFrame = sliderValue;
                        }
                        else
                        {
                            _controllerAps = AvatarObj.GetComponent<ComponentAPS>();
                            Log(SharedAPS.LogAnimControllerNotExist);
                            _controllerAps.allFrames = allFrames;
                            _controllerAps.selectedFrame = sliderValue;
                        }
                    }
                }

                GUILayout.Space(5);
                
                if (GUILayout.Button(SharedAPS.ButtonInitizlizeAvatar))
                {
                    string controllerPath = PathPlugin + "/Temp/customAPSAnim.controller";
                    
                    AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                    
                    Log(SharedAPS.LogAnimControllerCreated + controllerPath);

                    AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

                    if (controller)
                    {
                        AnimatorState state = controller.AddMotion(animationSource);
                        
                        Log(SharedAPS.LogAnimControllerAdded);
                        AssetDatabase.SaveAssets();

                        ComponentAPS component = AvatarObj.GetComponent<ComponentAPS>();

                        if (component != null)
                        {
                            Log(SharedAPS.LogComponentExits);
                            isInited = true;
                        }
                        else
                        {
                            TempAvatar = Instantiate(AvatarObj, AvatarObj.transform.position, AvatarObj.transform.rotation);
                            TempAvatar.gameObject.name = $"{TempAvatar.gameObject.name}_Temp";
                            TempAvatar.gameObject.SetActive(false);
                            
                            AvatarObj.AddComponent<ComponentAPS>();
                            isInited = true;
                        }
                    }
                    else
                    {
                        ErrorLog(SharedAPS.LogComponentNotExists + controllerPath);
                        isInited = false;
                    }
                }

                if (allFrames != 0 && AvatarObj && (sliderValue != -1) && _controllerAps && TempAvatar)
                {
                    GUILayout.Space(5);
                    if (GUILayout.Button(SharedAPS.ButtonSavePose))
                    {
                        var copiedObject = Instantiate(TempAvatar, TempAvatar.transform.position + (Vector3.left * counterDublicate), TempAvatar.transform.rotation);

                        counterDublicate += 1;
                        copiedObject.gameObject.SetActive(true);
                        copiedObject.gameObject.name = $"{copiedObject.gameObject.name}_{GenerateRandomString(5)}";
                        var componentCopy = copiedObject.AddComponent<ComponentAPS>();
                        componentCopy.selectedFrame = sliderValue;
                        componentCopy.allFrames = allFrames;
                    }
                }
                
            }
            
            GUILayout.Space(20);
            
            if (isInited)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(SharedAPS.ButtonEnterPlayMode, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        
        private static string GenerateRandomString(int length)
        {
            string guid = Guid.NewGuid().ToString("N");
            return guid.Substring(0, length);
        }

        private void OnDestroy()
        {
            
            if (AvatarObj)
            {
                DestroyImmediate(AvatarObj.GetComponent<ComponentAPS>());
            }

            if (TempAvatar)
            {
                DestroyImmediate(TempAvatar);
                string controllerPath = PathPlugin + "/Temp/customAPSAnim.controller";
                File.Delete(controllerPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
        public static string FindScriptPath(string scriptName)
        {
            string assetsPath = Application.dataPath;
            try
            {
                string[] files = Directory.GetFiles(assetsPath, scriptName, SearchOption.AllDirectories);
                if (files.Length > 0)
                {
                    string scriptPath = Path.GetDirectoryName(files[0]);
                    string fixScriptPath = scriptPath.Replace("\\", "/");
                    string relativePath = "Assets" + fixScriptPath.Replace(Application.dataPath, "");
                    return relativePath;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        public static void Log(string message)
        {
            if (SharedAPS.DebugBool)
            {
                Debug.Log("[APS][Log] " + message);
            }
        }
        
        public static void ErrorLog(string message)
        {
            if (SharedAPS.DebugBool)
            {
                Debug.LogError("[APS][Error] " + message);
            }
        }
    }
}