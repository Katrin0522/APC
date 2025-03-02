namespace APS.Runtime
{
    public static class SharedAPS
    {
        public static string ScriptForSearch = "WindowAPS.cs";
        public static bool DebugBool = true;

        // Logs localization
        public static string LogCreatedTempFolder = "Created Temp folder";
        public static string LogErrorFindPath = "Can`t find path to internal plugin. Reimport APS plugin OR place APS folder in Assets folder";
        public static string LogErrorWindow = "Error initialize with Error, see Console logs";
        public static string LogAnimControllerNotExist = "Custom AnimController doesn`t exist, getting new";
        public static string LogAnimControllerCreated = "Animator Controller created with path: ";
        public static string LogAnimControllerAdded = "Animation Clip added into Animator Controller";
        public static string LogComponentExits = "ComponentAPS exits!";
        public static string LogComponentNotExists = "Animator Controller cant find with path: ";
        
        //Window localization
        public static string LabelObjectAvatar = "GameObject Avatar";
        public static string LabelAnimation = "Animation";
        public static string LabelWorkWithAnimation = "Work with animation";
        public static string LabelTimelineAnimation = "Timeline Animation";
        public static string LabelSelectedFrame = "Selected frame: ";
        public static string ButtonClear = "Clear";
        public static string ButtonClearMode = "Clear mode: ";
        public static string ButtonClearModeOn = "ON";
        public static string ButtonClearModeOff = "OFF";
        public static string ButtonInitizlizeAvatar = "Initialization animation";
        public static string ButtonSavePose = "Save Pose";
        public static string ButtonEnterPlayMode = "Ready! Enter the Play Mode!";
    }
}