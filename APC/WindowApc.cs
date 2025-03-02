using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace APC
{
    public class WindowApc: EditorWindow
    {
        private AnimationClip animationSource;
        private GameObject AvatarObj;
        private GameObject TempAvatar;
        private AnimationWindow animationWindow;
        private ComponentApc _controllerApc;
        
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
        
        
        [MenuItem("Tools/APC Tool")]
        private static void Init()
        {
            var inspWndType = typeof(SceneView);
            var window = GetWindow<WindowApc>(inspWndType);
        }

        private bool FirstInit()
        {
            var assemblyPath = FindScriptPath("WindowApc.cs");
            if (assemblyPath == null)
            {
                Debug.LogError("[APC] Reimport APC plugin");
                windowInited = false;
                return false;
            }
            else
            {
                PathPlugin = assemblyPath;
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
                    GUILayout.Label ("Error init with error: 'Reimport APC plugin'", EditorStyles.boldLabel);
                    return;
                }
            }

            GUILayout.Space(5);
            GUILayout.Label ("Обьект аватара", EditorStyles.boldLabel);
            AvatarObj = EditorGUILayout.ObjectField(AvatarObj, typeof(GameObject), true) as GameObject;
            GUILayout.Space(5);
            GUILayout.Label ("Анимация", EditorStyles.boldLabel);
            animationSource = EditorGUILayout.ObjectField(animationSource, typeof(AnimationClip), false) as AnimationClip;
            GUILayout.Space(15);
            
            if (animationSource && AvatarObj)
            {
                EditorGUILayout.LabelField("Работа с анимацией", EditorStyles.boldLabel);
                GUILayout.Space(5);

                allFrames = (int)animationSource.length * (int)animationSource.frameRate;
                sliderValue = (int) EditorGUILayout.Slider("Таймлайн анимации", sliderValue, minValue, allFrames);
                GUILayout.Label($"Выбранный кадр: {sliderValue}");

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

                if (GUILayout.Button("Очистка", GUILayout.Width(100), GUILayout.Height(buttonSize)))
                {
                    isChange = !isChange;
                }
            
                GUILayout.EndHorizontal();
                GUILayout.Label("Режим Очистки: " + (isChange ? "ВКЛ" : "ВЫКЛ"));
                
                
                if (prevSliderValue != sliderValue)
                {
                    prevSliderValue = sliderValue;
                    if (EditorApplication.isPlaying)
                    {
                        if (_controllerApc)
                        {
                            _controllerApc.selectedFrame = sliderValue;
                        }
                        else
                        {
                            _controllerApc = AvatarObj.GetComponent<ComponentApc>();
                            Debug.Log("[APC] Custom AnimController doesn`t exist, getting new");
                            _controllerApc.allFrames = allFrames;
                            _controllerApc.selectedFrame = sliderValue;
                        }
                    }
                }

                GUILayout.Space(5);
                
                if (GUILayout.Button("Инициализация анимации"))
                {
                    string controllerPath = PathPlugin + "/Temp/customAPCAnim.controller";
                    
                    AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                    
                    Debug.Log("[APC] Animator Controller создан по пути: " + controllerPath);

                    AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

                    if (controller)
                    {
                        AnimatorState state = controller.AddMotion(animationSource);
                        
                        Debug.Log("[APC] Animation Clip добавлен в Animator Controller.");
                        AssetDatabase.SaveAssets();

                        ComponentApc component = AvatarObj.GetComponent<ComponentApc>();

                        if (component != null)
                        {
                            Debug.Log("[APC] ComponentApc уже существует!");
                            isInited = true;
                        }
                        else
                        {
                            TempAvatar = Instantiate(AvatarObj, AvatarObj.transform.position, AvatarObj.transform.rotation);
                            TempAvatar.gameObject.name = $"{TempAvatar.gameObject.name}_Temp";
                            TempAvatar.gameObject.SetActive(false);
                            
                            AvatarObj.AddComponent<ComponentApc>();
                            isInited = true;
                        }
                    }
                    else
                    {
                        Debug.LogError("[APC] Animator Controller не найден по пути: " + controllerPath);
                        isInited = false;
                    }
                }

                if (allFrames != 0 && AvatarObj && (sliderValue != -1) && _controllerApc && TempAvatar)
                {
                    GUILayout.Space(5);
                    if (GUILayout.Button("Дублировать позу"))
                    {
                        var copiedObject = Instantiate(TempAvatar, TempAvatar.transform.position + (Vector3.left * counterDublicate), TempAvatar.transform.rotation);

                        counterDublicate += 1;
                        copiedObject.gameObject.SetActive(true);
                        copiedObject.gameObject.name = $"{copiedObject.gameObject.name}_{GenerateRandomString(5)}";
                        var componentCopy = copiedObject.AddComponent<ComponentApc>();
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
                GUILayout.Label("Готово! Включай Play!", EditorStyles.boldLabel);
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
                DestroyImmediate(AvatarObj.GetComponent<ComponentApc>());
            }

            if (TempAvatar)
            {
                DestroyImmediate(TempAvatar);
                string controllerPath = PathPlugin + "/Temp/customAPCAnim.controller";
                File.Delete(controllerPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
        public static string FindScriptPath(string scriptName)
        {
            string assetsPath = Application.dataPath; // Путь к папке Assets
            string[] files = Directory.GetFiles(assetsPath, scriptName, SearchOption.AllDirectories);


            if (files.Length > 0)
            {
                string scriptPath = Path.GetDirectoryName(files[0]);
                string fixScriptPath = scriptPath.Replace("\\", "/");
                string relativePath = "Assets" + fixScriptPath.Replace(Application.dataPath, "");
                return relativePath;
            }
            else
            {
                return null;
            }

        }
    }
}