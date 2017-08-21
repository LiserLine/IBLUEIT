#if UNITY_CHANGE1 || UNITY_CHANGE2 || UNITY_CHANGE3
#warning UNITY_CHANGE has been set manually
#elif UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_CHANGE1
#elif UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define UNITY_CHANGE2
#else
#define UNITY_CHANGE3
#endif
//use UNITY_CHANGE1 for unity older than "unity 5"
//use UNITY_CHANGE2 for unity 5.0 -> 5.3 
//use UNITY_CHANGE3 for unity 5.3 (fix for new SceneManger system  )


using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
#if UNITY_CHANGE3
using UnityEngine.SceneManagement;

#endif


[Serializable]
public class Images
{
    public Texture2D BackImage;

    public Texture2D BarImage;

    public Texture2D BuildFromImage;
    public Texture2D ButtonActiveImage;
    public Texture2D ClearImage;
    public Texture2D ClearOnNewSceneImage;
    public Texture2D CloseImage;
    public Texture2D CollapseImage;
    public Texture2D DateImage;
    public Texture2D ErrorImage;
    public Texture2D EvenLogImage;
    public Texture2D GraphicsInfoImage;
    public Texture2D InfoImage;

    public Texture2D LogImage;
    public Texture2D OddLogImage;

    public GUISkin ReporterScrollerSkin;
    public Texture2D SearchImage;
    public Texture2D SelectedImage;
    public Texture2D ShowFpsImage;
    public Texture2D ShowMemoryImage;
    public Texture2D ShowSceneImage;
    public Texture2D ShowTimeImage;
    public Texture2D SoftwareImage;
    public Texture2D SystemInfoImage;
    public Texture2D UserImage;
    public Texture2D WarningImage;
}

//To use Reporter just create reporter from menu (Reporter->Create) at first scene your game start.
//then set the ” Scrip execution order ” in (Edit -> Project Settings ) of Reporter.cs to be the highest.

//Now to view logs all what you have to do is to make a circle gesture using your mouse (click and drag) 
//or your finger (touch and drag) on the screen to show all these logs
//no coding is required 

public class Reporter : MonoBehaviour
{
    public enum _LogType
    {
        Assert = LogType.Assert,
        Error = LogType.Error,
        Exception = LogType.Exception,
        Log = LogType.Log,
        Warning = LogType.Warning
    }

    public class Sample
    {
        public float Fps;
        public string FpsText;
        public sbyte LoadedScene;
        public float Memory;
        public float Time;

        public static float MemSize()
        {
            return sizeof(float) + sizeof(byte) + sizeof(float) + sizeof(float);
        }

        public string GetSceneName()
        {
            return LoadedScene == -1 ? "AssetBundleScene" : _scenes[LoadedScene];
        }
    }

    private readonly List<Sample> _samples = new List<Sample>(60 * 60 * 60);

    public class Log
    {
        public string Condition;
        public int Count = 1;
        public _LogType LogType;
        public int SampleId;

        public string Stacktrace;
        //public string   objectName="" ;//object who send error
        //public string   rootName =""; //root of object send error

        public Log CreateCopy()
        {
            return (Log) MemberwiseClone();
        }

        public float GetMemoryUsage()
        {
            return sizeof(int) +
                   sizeof(_LogType) +
                   Condition.Length * sizeof(char) +
                   Stacktrace.Length * sizeof(char) +
                   sizeof(int);
        }
    }

    //contains all uncollapsed log
    private readonly List<Log> _logs = new List<Log>();

    //contains all collapsed logs
    private readonly List<Log> _collapsedLogs = new List<Log>();

    //contain logs which should only appear to user , for example if you switch off show logs + switch off show warnings
    //and your mode is collapse,then this list will contains only collapsed errors
    private readonly List<Log> _currentLog = new List<Log>();

    //used to check if the new coming logs is already exist or new one
    private readonly MultiKeyDictionary<string, string, Log> _logsDic = new MultiKeyDictionary<string, string, Log>();

    //to save memory
    private readonly Dictionary<string, string> _cachedString = new Dictionary<string, string>();

    [HideInInspector]
    //show hide In Game Logs
    public bool Show;

    //collapse logs
    private bool _collapse;

    //to decide if you want to clean logs for new loaded scene
    private bool _clearOnNewSceneLoaded;

    private bool _showTime;

    private bool _showScene;

    private bool _showMemory;

    private bool _showFps;

    private bool _showGraph;

    //show or hide logs
    private bool _showLog = true;

    //show or hide warnings
    private bool _showWarning = true;

    //show or hide errors
    private bool _showError = true;

    //total number of logs
    private int _numOfLogs;

    //total number of warnings
    private int _numOfLogsWarning;

    //total number of errors
    private int _numOfLogsError;

    //total number of collapsed logs
    private int _numOfCollapsedLogs;

    //total number of collapsed warnings
    private int _numOfCollapsedLogsWarning;

    //total number of collapsed errors
    private int _numOfCollapsedLogsError;

    //maximum number of allowed logs to view
    //public int maxAllowedLog = 1000 ;

    private bool _showClearOnNewSceneLoadedButton = true;
    private bool _showTimeButton = true;
    private bool _showSceneButton = true;
    private bool _showMemButton = true;
    private bool _showFpsButton = true;
    private bool _showSearchText = true;

    private string _buildDate;
    private string _logDate;
    private float _logsMemUsage;
    private float _graphMemUsage;
    public float TotalMemUsage => _logsMemUsage + _graphMemUsage;

    private float _gcTotalMemory;

    public string UserData = "";

    //frame rate per second
    public float Fps;

    public string FpsText;

    //List<Texture2D> snapshots = new List<Texture2D>() ;

    private enum ReportView
    {
        None,
        Logs,
        Info,
        Snapshot
    }

    private ReportView _currentView = ReportView.Logs;

    private enum DetailView
    {
        None,
        StackTrace,
        Graph
    }

    //used to check if you have In Game Logs multiple time in different scene
    //only one should work and other should be deleted
    private static bool _created;
    //public delegate void OnLogHandler( string condition, string stack-trace, LogType type );
    //public event OnLogHandler OnLog ;

    public Images Images;

    // gui
    private GUIContent _clearContent;

    private GUIContent _collapseContent;
    private GUIContent _clearOnNewSceneContent;
    private GUIContent _showTimeContent;
    private GUIContent _showSceneContent;
    private GUIContent _userContent;
    private GUIContent _showMemoryContent;
    private GUIContent _softwareContent;
    private GUIContent _dateContent;

    private GUIContent _showFpsContent;

    //GUIContent graphContent;
    private GUIContent _infoContent;

    private GUIContent _searchContent;
    private GUIContent _closeContent;

    private GUIContent _buildFromContent;
    private GUIContent _systemInfoContent;
    private GUIContent _graphicsInfoContent;
    private GUIContent _backContent;

    //GUIContent cameraContent;

    private GUIContent _logContent;
    private GUIContent _warningContent;
    private GUIContent _errorContent;
    private GUIStyle _barStyle;
    private GUIStyle _buttonActiveStyle;

    private GUIStyle _nonStyle;
    private GUIStyle _lowerLeftFontStyle;
    private GUIStyle _backStyle;
    private GUIStyle _evenLogStyle;
    private GUIStyle _oddLogStyle;
    private GUIStyle _logButtonStyle;
    private GUIStyle _selectedLogStyle;
    private GUIStyle _selectedLogFontStyle;
    private GUIStyle _stackLabelStyle;
    private GUIStyle _scrollerStyle;
    private GUIStyle _searchStyle;
    private GUIStyle _sliderBackStyle;
    private GUIStyle _sliderThumbStyle;
    private GUISkin _toolbarScrollerSkin;
    private GUISkin _logScrollerSkin;
    private GUISkin _graphScrollerSkin;

    public Vector2 Size = new Vector2(32, 32);
    public float MaxSize = 20;
    public int NumOfCircleToShow = 1;
    private static string[] _scenes;
    private string _currentScene;
    private string filterText = "";

    private string _deviceModel;
    private string _deviceType;
    private string _deviceName;
    private string _graphicsMemorySize;
#if !UNITY_CHANGE1
    private string _maxTextureSize;
#endif
    private string _systemMemorySize;

    private void Awake()
    {
        if (!Initialized)
            Initialize();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (_logs.Count == 0) //if recompile while in play mode
            Clear();
    }

    private void OnDisable()
    {
    }

    private void AddSample()
    {
        var sample = new Sample
        {
            Fps = Fps,
            FpsText = FpsText,
            LoadedScene = (sbyte) SceneManager.GetActiveScene().buildIndex,
            Time = Time.realtimeSinceStartup,
            Memory = _gcTotalMemory
        };
#if UNITY_CHANGE3
#else
		sample.loadedScene = (byte)Application.loadedLevel;
#endif
        _samples.Add(sample);

        _graphMemUsage = _samples.Count * Sample.MemSize() / 1024 / 1024;
    }

    public bool Initialized;

    public void Initialize()
    {
        if (!_created)
        {
            try
            {
                gameObject.SendMessage("OnPreStart");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#if UNITY_CHANGE3
            _scenes = new string[SceneManager.sceneCountInBuildSettings];
            _currentScene = SceneManager.GetActiveScene().name;
#else
			scenes = new string[Application.levelCount];
			currentScene = Application.loadedLevelName;
#endif
            DontDestroyOnLoad(gameObject);
#if UNITY_CHANGE1
			Application.RegisterLogCallback (new Application.LogCallback (CaptureLog));
			Application.RegisterLogCallbackThreaded (new Application.LogCallback (CaptureLogThread));
#else
            //Application.logMessageReceived += CaptureLog ;
            Application.logMessageReceivedThreaded += CaptureLogThread;
#endif
            _created = true;
            //addSample();
        }
        else
        {
            Debug.LogWarning("tow manager is exists delete the second");
            DestroyImmediate(gameObject, true);
            return;
        }


        //initialize gui and styles for gui purpose

        _clearContent = new GUIContent("", Images.ClearImage, "Clear logs");
        _collapseContent = new GUIContent("", Images.CollapseImage, "Collapse logs");
        _clearOnNewSceneContent = new GUIContent("", Images.ClearOnNewSceneImage, "Clear logs on new scene loaded");
        _showTimeContent = new GUIContent("", Images.ShowTimeImage, "Show Hide Time");
        _showSceneContent = new GUIContent("", Images.ShowSceneImage, "Show Hide Scene");
        _showMemoryContent = new GUIContent("", Images.ShowMemoryImage, "Show Hide Memory");
        _softwareContent = new GUIContent("", Images.SoftwareImage, "Software");
        _dateContent = new GUIContent("", Images.DateImage, "Date");
        _showFpsContent = new GUIContent("", Images.ShowFpsImage, "Show Hide fps");
        _infoContent = new GUIContent("", Images.InfoImage, "Information about application");
        _searchContent = new GUIContent("", Images.SearchImage, "Search for logs");
        _closeContent = new GUIContent("", Images.CloseImage, "Hide logs");
        _userContent = new GUIContent("", Images.UserImage, "User");

        _buildFromContent = new GUIContent("", Images.BuildFromImage, "Build From");
        _systemInfoContent = new GUIContent("", Images.SystemInfoImage, "System Info");
        _graphicsInfoContent = new GUIContent("", Images.GraphicsInfoImage, "Graphics Info");
        _backContent = new GUIContent("", Images.BackImage, "Back");


        //snapshotContent = new GUIContent("",images.cameraImage,"show or hide logs");
        _logContent = new GUIContent("", Images.LogImage, "show or hide logs");
        _warningContent = new GUIContent("", Images.WarningImage, "show or hide warnings");
        _errorContent = new GUIContent("", Images.ErrorImage, "show or hide errors");


        _currentView = (ReportView) PlayerPrefs.GetInt("Reporter_currentView", 1);
        Show = PlayerPrefs.GetInt("Reporter_show") == 1;
        _collapse = PlayerPrefs.GetInt("Reporter_collapse") == 1;
        _clearOnNewSceneLoaded = PlayerPrefs.GetInt("Reporter_clearOnNewSceneLoaded") == 1;
        _showTime = PlayerPrefs.GetInt("Reporter_showTime") == 1;
        _showScene = PlayerPrefs.GetInt("Reporter_showScene") == 1;
        _showMemory = PlayerPrefs.GetInt("Reporter_showMemory") == 1;
        _showFps = PlayerPrefs.GetInt("Reporter_showFps") == 1;
        _showGraph = PlayerPrefs.GetInt("Reporter_showGraph") == 1;
        _showLog = PlayerPrefs.GetInt("Reporter_showLog", 1) == 1;
        _showWarning = PlayerPrefs.GetInt("Reporter_showWarning", 1) == 1;
        _showError = PlayerPrefs.GetInt("Reporter_showError", 1) == 1;
        filterText = PlayerPrefs.GetString("Reporter_filterText");
        Size.x = Size.y = PlayerPrefs.GetFloat("Reporter_size", 32);


        _showClearOnNewSceneLoadedButton = PlayerPrefs.GetInt("Reporter_showClearOnNewSceneLoadedButton", 1) == 1;
        _showTimeButton = PlayerPrefs.GetInt("Reporter_showTimeButton", 1) == 1;
        _showSceneButton = PlayerPrefs.GetInt("Reporter_showSceneButton", 1) == 1;
        _showMemButton = PlayerPrefs.GetInt("Reporter_showMemButton", 1) == 1;
        _showFpsButton = PlayerPrefs.GetInt("Reporter_showFpsButton", 1) == 1;
        _showSearchText = PlayerPrefs.GetInt("Reporter_showSearchText", 1) == 1;


        InitializeStyle();

        Initialized = true;

        if (Show)
            DoShow();

        _deviceModel = SystemInfo.deviceModel;
        _deviceType = SystemInfo.deviceType.ToString();
        _deviceName = SystemInfo.deviceName;
        _graphicsMemorySize = SystemInfo.graphicsMemorySize.ToString();
#if !UNITY_CHANGE1
        _maxTextureSize = SystemInfo.maxTextureSize.ToString();
#endif
        _systemMemorySize = SystemInfo.systemMemorySize.ToString();
    }

    private void InitializeStyle()
    {
        var paddingX = (int) (Size.x * 0.2f);
        var paddingY = (int) (Size.y * 0.2f);
        _nonStyle = new GUIStyle
        {
            clipping = TextClipping.Clip,
            border = new RectOffset(0, 0, 0, 0),
            normal = {background = null},
            fontSize = (int) (Size.y / 2),
            alignment = TextAnchor.MiddleCenter
        };

        _lowerLeftFontStyle = new GUIStyle
        {
            clipping = TextClipping.Clip,
            border = new RectOffset(0, 0, 0, 0),
            normal = {background = null},
            fontSize = (int) (Size.y / 2),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.LowerLeft
        };

        _barStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            normal = {background = Images.BarImage},
            active = {background = Images.ButtonActiveImage},
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(1, 1, 1, 1),
            clipping = TextClipping.Clip,
            fontSize = (int) (Size.y / 2)
        };
        //barStyle.padding = new RectOffset(paddingX,paddingX,paddingY,paddingY); 
        //barStyle.wordWrap = true ;




        _buttonActiveStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            normal = {background = Images.ButtonActiveImage},
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(1, 1, 1, 1),
            fontSize = (int) (Size.y / 2)
        };
        //buttonActiveStyle.padding = new RectOffset(4,4,4,4);

        _backStyle = new GUIStyle
        {
            normal = {background = Images.EvenLogImage},
            clipping = TextClipping.Clip,
            fontSize = (int) (Size.y / 2)
        };

        _evenLogStyle = new GUIStyle
        {
            normal = {background = Images.EvenLogImage},
            fixedHeight = Size.y,
            clipping = TextClipping.Clip,
            alignment = TextAnchor.UpperLeft,
            imagePosition = ImagePosition.ImageLeft,
            fontSize = (int) (Size.y / 2)
        };
        //evenLogStyle.wordWrap = true;

        _oddLogStyle = new GUIStyle
        {
            normal = {background = Images.OddLogImage},
            fixedHeight = Size.y,
            clipping = TextClipping.Clip,
            alignment = TextAnchor.UpperLeft,
            imagePosition = ImagePosition.ImageLeft,
            fontSize = (int) (Size.y / 2)
        };
        //oddLogStyle.wordWrap = true ;

        _logButtonStyle = new GUIStyle
        {
            fixedHeight = Size.y,
            clipping = TextClipping.Clip,
            alignment = TextAnchor.UpperLeft,
            fontSize = (int) (Size.y / 2),
            padding = new RectOffset(paddingX, paddingX, paddingY, paddingY)
        };
        //logButtonStyle.wordWrap = true;
        //logButtonStyle.imagePosition = ImagePosition.ImageLeft ;
        //logButtonStyle.wordWrap = true;

        _selectedLogStyle = new GUIStyle
        {
            normal = {background = Images.SelectedImage},
            fixedHeight = Size.y,
            clipping = TextClipping.Clip,
            alignment = TextAnchor.UpperLeft
        };
        _selectedLogStyle.normal.textColor = Color.white;
        _selectedLogStyle.fontSize = (int) (Size.y / 2);
        //selectedLogStyle.wordWrap = true;

        _selectedLogFontStyle = new GUIStyle
        {
            normal = {background = Images.SelectedImage},
            fixedHeight = Size.y,
            clipping = TextClipping.Clip,
            alignment = TextAnchor.UpperLeft
        };
        _selectedLogFontStyle.normal.textColor = Color.white;
        //selectedLogStyle.wordWrap = true;
        _selectedLogFontStyle.fontSize = (int) (Size.y / 2);
        _selectedLogFontStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        _stackLabelStyle = new GUIStyle
        {
            wordWrap = true,
            fontSize = (int) (Size.y / 2),
            padding = new RectOffset(paddingX, paddingX, paddingY, paddingY)
        };

        _scrollerStyle = new GUIStyle {normal = {background = Images.BarImage}};

        _searchStyle = new GUIStyle
        {
            clipping = TextClipping.Clip,
            alignment = TextAnchor.LowerCenter,
            fontSize = (int) (Size.y / 2),
            wordWrap = true
        };


        _sliderBackStyle = new GUIStyle
        {
            normal = {background = Images.BarImage},
            fixedHeight = Size.y,
            border = new RectOffset(1, 1, 1, 1)
        };

        _sliderThumbStyle = new GUIStyle
        {
            normal = {background = Images.SelectedImage},
            fixedWidth = Size.x
        };

        var skin = Images.ReporterScrollerSkin;

        _toolbarScrollerSkin = Instantiate(skin);
        _toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        _toolbarScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
        _toolbarScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
        _toolbarScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

        _logScrollerSkin = Instantiate(skin);
        _logScrollerSkin.verticalScrollbar.fixedWidth = Size.x * 2f;
        _logScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
        _logScrollerSkin.verticalScrollbarThumb.fixedWidth = Size.x * 2f;
        _logScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

        _graphScrollerSkin = Instantiate(skin);
        _graphScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        _graphScrollerSkin.horizontalScrollbar.fixedHeight = Size.x * 2f;
        _graphScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
        _graphScrollerSkin.horizontalScrollbarThumb.fixedHeight = Size.x * 2f;
        //inGameLogsScrollerSkin.verticalScrollbarThumb.fixedWidth = size.x * 2;
        //inGameLogsScrollerSkin.verticalScrollbar.fixedWidth = size.x * 2;
    }

    private void Start()
    {
        _logDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        StartCoroutine("ReadInfo");
    }

    //clear all logs
    private void Clear()
    {
        _logs.Clear();
        _collapsedLogs.Clear();
        _currentLog.Clear();
        _logsDic.Clear();
        //_selectedIndex = -1;
        _selectedLog = null;
        _numOfLogs = 0;
        _numOfLogsWarning = 0;
        _numOfLogsError = 0;
        _numOfCollapsedLogs = 0;
        _numOfCollapsedLogsWarning = 0;
        _numOfCollapsedLogsError = 0;
        _logsMemUsage = 0;
        _graphMemUsage = 0;
        _samples.Clear();
        GC.Collect();
        _selectedLog = null;
    }

    private Rect _screenRect;
    private Rect _toolBarRect;
    private Rect _logsRect;
    private Rect _stackRect;
    private Rect _graphRect;
    private Rect _graphMinRect;
    private Rect _graphMaxRect;
    private Rect _buttomRect;
    private Vector2 _stackRectTopLeft;
    private Rect _detailRect;

    private Vector2 _scrollPosition;
    private Vector2 _scrollPosition2;
    private Vector2 _toolbarScrollPosition;

    //int 	selectedIndex = -1;
    private Log _selectedLog;

    private float _toolbarOldDrag;
    private float _oldDrag;
    private float _oldDrag2;
    private float _oldDrag3;
    private int _startIndex;

    //calculate what is the currentLog : collapsed or not , hide or view warnings ......
    private void CalculateCurrentLog()
    {
        var filter = !string.IsNullOrEmpty(filterText);
        var _filterText = "";
        if (filter)
            _filterText = filterText.ToLower();
        _currentLog.Clear();
        if (_collapse)
            foreach (var log in _collapsedLogs)
            {
                if (log.LogType == _LogType.Log && !_showLog)
                    continue;
                if (log.LogType == _LogType.Warning && !_showWarning)
                    continue;
                if (log.LogType == _LogType.Error && !_showError)
                    continue;
                if (log.LogType == _LogType.Assert && !_showError)
                    continue;
                if (log.LogType == _LogType.Exception && !_showError)
                    continue;

                if (filter)
                {
                    if (log.Condition.ToLower().Contains(_filterText))
                        _currentLog.Add(log);
                }
                else
                {
                    _currentLog.Add(log);
                }
            }
        else
            foreach (var log in _logs)
            {
                if (log.LogType == _LogType.Log && !_showLog)
                    continue;
                if (log.LogType == _LogType.Warning && !_showWarning)
                    continue;
                if (log.LogType == _LogType.Error && !_showError)
                    continue;
                if (log.LogType == _LogType.Assert && !_showError)
                    continue;
                if (log.LogType == _LogType.Exception && !_showError)
                    continue;

                if (filter)
                {
                    if (log.Condition.ToLower().Contains(_filterText))
                        _currentLog.Add(log);
                }
                else
                {
                    _currentLog.Add(log);
                }
            }

        if (_selectedLog == null) return;
        var newSelectedIndex = _currentLog.IndexOf(_selectedLog);
        if (newSelectedIndex == -1)
        {
            var collapsedSelected = _logsDic[_selectedLog.Condition][_selectedLog.Stacktrace];
            newSelectedIndex = _currentLog.IndexOf(collapsedSelected);
            if (newSelectedIndex != -1)
                _scrollPosition.y = newSelectedIndex * Size.y;
        }
        else
        {
            _scrollPosition.y = newSelectedIndex * Size.y;
        }
    }

    private Rect _countRect;
    private Rect _timeRect;
    private Rect _timeLabelRect;
    private Rect _sceneRect;
    private Rect _sceneLabelRect;
    private Rect _memoryRect;
    private Rect _memoryLabelRect;
    private Rect _fpsRect;
    private Rect _fpsLabelRect;
    private readonly GUIContent _tempContent = new GUIContent();


    private Vector2 _infoScrollPosition;
    private Vector2 _oldInfoDrag;

    private void DrawInfo()
    {
        GUILayout.BeginArea(_screenRect, _backStyle);

        var drag = GetDrag();
        if (drag.x != 0f && _downPos != Vector2.zero)
            _infoScrollPosition.x -= drag.x - _oldInfoDrag.x;
        if (drag.y != 0f && _downPos != Vector2.zero)
            _infoScrollPosition.y += drag.y - _oldInfoDrag.y;
        _oldInfoDrag = drag;

        GUI.skin = _toolbarScrollerSkin;
        _infoScrollPosition = GUILayout.BeginScrollView(_infoScrollPosition);
        GUILayout.Space(Size.x);
        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_buildFromContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(_buildDate, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_systemInfoContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(_deviceModel, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(_deviceType, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(_deviceName, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_graphicsInfoContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(SystemInfo.graphicsDeviceName, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(_graphicsMemorySize, _nonStyle, GUILayout.Height(Size.y));
#if !UNITY_CHANGE1
        GUILayout.Space(Size.x);
        GUILayout.Label(_maxTextureSize, _nonStyle, GUILayout.Height(Size.y));
#endif
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Space(Size.x);
        GUILayout.Space(Size.x);
        GUILayout.Label("Screen Width " + Screen.width, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label("Screen Height " + Screen.height, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_showMemoryContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(_systemMemorySize + " mb", _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Space(Size.x);
        GUILayout.Space(Size.x);
        GUILayout.Label("Mem Usage Of Logs " + _logsMemUsage.ToString("0.000") + " mb", _nonStyle,
            GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        //GUILayout.Label( "Mem Usage Of Graph " + graphMemUsage.ToString("0.000")  + " mb", nonStyle , GUILayout.Height(size.y));
        //GUILayout.Space( size.x);
        GUILayout.Label("GC Memory " + _gcTotalMemory.ToString("0.000") + " mb", _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_softwareContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(SystemInfo.operatingSystem, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_dateContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(DateTime.Now.ToString(), _nonStyle, GUILayout.Height(Size.y));
        GUILayout.Label(" - Application Started At " + _logDate, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_showTimeContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(Time.realtimeSinceStartup.ToString("000"), _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_showFpsContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(FpsText, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_userContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(UserData, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_showSceneContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label(_currentScene, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Box(_showSceneContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.Label("Unity Version = " + Application.unityVersion, _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        /*GUILayout.BeginHorizontal();
		GUILayout.Space( size.x);
		GUILayout.Box( graphContent ,nonStyle ,  GUILayout.Width(size.x) , GUILayout.Height(size.y));
		GUILayout.Space( size.x);
		GUILayout.Label( "frame " + samples.Count , nonStyle , GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();*/

        drawInfo_enableDisableToolBarButtons();

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Label("Size = " + Size.x.ToString("0.0"), _nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        var _size = GUILayout.HorizontalSlider(Size.x, 16, 64, _sliderBackStyle, _sliderThumbStyle,
            GUILayout.Width(Screen.width * 0.5f));
        if (Size.x != _size)
        {
            Size.x = Size.y = _size;
            InitializeStyle();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        if (GUILayout.Button(_backContent, _barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            _currentView = ReportView.Logs;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }


    private void drawInfo_enableDisableToolBarButtons()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);
        GUILayout.Label("Hide or Show tool bar buttons", _nonStyle, GUILayout.Height(Size.y));
        GUILayout.Space(Size.x);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(Size.x);

        if (GUILayout.Button(_clearOnNewSceneContent, _showClearOnNewSceneLoadedButton ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            _showClearOnNewSceneLoadedButton = !_showClearOnNewSceneLoadedButton;

        if (GUILayout.Button(_showTimeContent, _showTimeButton ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            _showTimeButton = !_showTimeButton;
        _tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(_tempRect, Time.realtimeSinceStartup.ToString("0.0"), _lowerLeftFontStyle);
        if (GUILayout.Button(_showSceneContent, _showSceneButton ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            _showSceneButton = !_showSceneButton;
        _tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(_tempRect, _currentScene, _lowerLeftFontStyle);
        if (GUILayout.Button(_showMemoryContent, _showMemButton ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            _showMemButton = !_showMemButton;
        _tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(_tempRect, _gcTotalMemory.ToString("0.0"), _lowerLeftFontStyle);

        if (GUILayout.Button(_showFpsContent, _showFpsButton ? _buttonActiveStyle : _barStyle, GUILayout.Width(Size.x * 2),
            GUILayout.Height(Size.y * 2)))
            _showFpsButton = !_showFpsButton;
        _tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(_tempRect, FpsText, _lowerLeftFontStyle);
        /*if( GUILayout.Button( graphContent , (showGraph)?buttonActiveStyle:barStyle , GUILayout.Width(size.x*2) ,GUILayout.Height(size.y*2)))
		{
			showGraph = !showGraph ;
		}
		tempRect = GUILayoutUtility.GetLastRect();
		GUI.Label( tempRect , samples.Count.ToString() , lowerLeftFontStyle );*/
        if (GUILayout.Button(_searchContent, _showSearchText ? _buttonActiveStyle : _barStyle, GUILayout.Width(Size.x * 2),
            GUILayout.Height(Size.y * 2)))
            _showSearchText = !_showSearchText;
        _tempRect = GUILayoutUtility.GetLastRect();
        GUI.TextField(_tempRect, filterText, _searchStyle);


        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void DrawReport()
    {
        _screenRect.x = 0f;
        _screenRect.y = 0f;
        _screenRect.width = Screen.width;
        _screenRect.height = Screen.height;
        GUILayout.BeginArea(_screenRect, _backStyle);
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        /*GUILayout.Box( cameraContent ,nonStyle ,  GUILayout.Width(size.x) , GUILayout.Height(size.y));
		GUILayout.FlexibleSpace();*/
        GUILayout.Label("Select Photo", _nonStyle, GUILayout.Height(Size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Coming Soon", _nonStyle, GUILayout.Height(Size.y));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(_backContent, _barStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y)))
            _currentView = ReportView.Logs;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DrawToolBar()
    {
        _toolBarRect.x = 0f;
        _toolBarRect.y = 0f;
        _toolBarRect.width = Screen.width;
        _toolBarRect.height = Size.y * 2f;

        //toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        //toolbarScrollerSkin.horizontalScrollbar.fixedHeight= 0f  ;

        GUI.skin = _toolbarScrollerSkin;
        var drag = GetDrag();
        if (drag.x != 0 && _downPos != Vector2.zero && _downPos.y > Screen.height - Size.y * 2f)
            _toolbarScrollPosition.x -= drag.x - _toolbarOldDrag;
        _toolbarOldDrag = drag.x;
        GUILayout.BeginArea(_toolBarRect);
        _toolbarScrollPosition = GUILayout.BeginScrollView(_toolbarScrollPosition);
        GUILayout.BeginHorizontal(_barStyle);

        if (GUILayout.Button(_clearContent, _barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            Clear();
        if (GUILayout.Button(_collapseContent, _collapse ? _buttonActiveStyle : _barStyle, GUILayout.Width(Size.x * 2),
            GUILayout.Height(Size.y * 2)))
        {
            _collapse = !_collapse;
            CalculateCurrentLog();
        }
        if (_showClearOnNewSceneLoadedButton && GUILayout.Button(_clearOnNewSceneContent,
                _clearOnNewSceneLoaded ? _buttonActiveStyle : _barStyle, GUILayout.Width(Size.x * 2),
                GUILayout.Height(Size.y * 2)))
            _clearOnNewSceneLoaded = !_clearOnNewSceneLoaded;

        if (_showTimeButton && GUILayout.Button(_showTimeContent, _showTime ? _buttonActiveStyle : _barStyle,
                GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            _showTime = !_showTime;
        if (_showSceneButton)
        {
            _tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(_tempRect, Time.realtimeSinceStartup.ToString("0.0"), _lowerLeftFontStyle);
            if (GUILayout.Button(_showSceneContent, _showScene ? _buttonActiveStyle : _barStyle,
                GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
                _showScene = !_showScene;
            _tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(_tempRect, _currentScene, _lowerLeftFontStyle);
        }
        if (_showMemButton)
        {
            if (GUILayout.Button(_showMemoryContent, _showMemory ? _buttonActiveStyle : _barStyle,
                GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
                _showMemory = !_showMemory;
            _tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(_tempRect, _gcTotalMemory.ToString("0.0"), _lowerLeftFontStyle);
        }
        if (_showFpsButton)
        {
            if (GUILayout.Button(_showFpsContent, _showFps ? _buttonActiveStyle : _barStyle, GUILayout.Width(Size.x * 2),
                GUILayout.Height(Size.y * 2)))
                _showFps = !_showFps;
            _tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(_tempRect, FpsText, _lowerLeftFontStyle);
        }
        /*if( GUILayout.Button( graphContent , (showGraph)?buttonActiveStyle:barStyle , GUILayout.Width(size.x*2) ,GUILayout.Height(size.y*2)))
		{
			showGraph = !showGraph ;
		}
		tempRect = GUILayoutUtility.GetLastRect();
		GUI.Label( tempRect , samples.Count.ToString() , lowerLeftFontStyle );*/

        if (_showSearchText)
        {
            GUILayout.Box(_searchContent, _barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2));
            _tempRect = GUILayoutUtility.GetLastRect();
            var newFilterText = GUI.TextField(_tempRect, filterText, _searchStyle);
            if (newFilterText != filterText)
            {
                filterText = newFilterText;
                CalculateCurrentLog();
            }
        }

        if (GUILayout.Button(_infoContent, _barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
            _currentView = ReportView.Info;


        GUILayout.FlexibleSpace();


        var logsText = " ";
        if (_collapse)
            logsText += _numOfCollapsedLogs;
        else
            logsText += _numOfLogs;
        var logsWarningText = " ";
        if (_collapse)
            logsWarningText += _numOfCollapsedLogsWarning;
        else
            logsWarningText += _numOfLogsWarning;
        var logsErrorText = " ";
        if (_collapse)
            logsErrorText += _numOfCollapsedLogsError;
        else
            logsErrorText += _numOfLogsError;

        GUILayout.BeginHorizontal(_showLog ? _buttonActiveStyle : _barStyle);
        if (GUILayout.Button(_logContent, _nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            _showLog = !_showLog;
            CalculateCurrentLog();
        }
        if (GUILayout.Button(logsText, _nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            _showLog = !_showLog;
            CalculateCurrentLog();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(_showWarning ? _buttonActiveStyle : _barStyle);
        if (GUILayout.Button(_warningContent, _nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            _showWarning = !_showWarning;
            CalculateCurrentLog();
        }
        if (GUILayout.Button(logsWarningText, _nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            _showWarning = !_showWarning;
            CalculateCurrentLog();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(_showError ? _buttonActiveStyle : _nonStyle);
        if (GUILayout.Button(_errorContent, _nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            _showError = !_showError;
            CalculateCurrentLog();
        }
        if (GUILayout.Button(logsErrorText, _nonStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            _showError = !_showError;
            CalculateCurrentLog();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button(_closeContent, _barStyle, GUILayout.Width(Size.x * 2), GUILayout.Height(Size.y * 2)))
        {
            Show = false;
            var gui = gameObject.GetComponent<ReporterGUI>();
            DestroyImmediate(gui);

            try
            {
                gameObject.SendMessage("OnHideReporter");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }


        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }


    private Rect _tempRect;

    private void DrawLogs()
    {
        GUILayout.BeginArea(_logsRect, _backStyle);

        GUI.skin = _logScrollerSkin;
        //setStartPos();
        var drag = GetDrag();

        if (drag.y != 0 && _logsRect.Contains(new Vector2(_downPos.x, Screen.height - _downPos.y)))
            _scrollPosition.y += drag.y - _oldDrag;
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        _oldDrag = drag.y;


        var totalVisibleCount = (int) (Screen.height * 0.75f / Size.y);
        var totalCount = _currentLog.Count;
        /*if( totalCount < 100 )
			inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 0;
		else 
			inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 64;*/

        totalVisibleCount = Mathf.Min(totalVisibleCount, totalCount - _startIndex);
        var index = 0;
        var beforeHeight = (int) (_startIndex * Size.y);
        //selectedIndex = Mathf.Clamp( selectedIndex , -1 , totalCount -1);
        if (beforeHeight > 0)
        {
            //fill invisible gap before scroller to make proper scroller pos
            GUILayout.BeginHorizontal(GUILayout.Height(beforeHeight));
            GUILayout.Label("---");
            GUILayout.EndHorizontal();
        }

        var endIndex = _startIndex + totalVisibleCount;
        endIndex = Mathf.Clamp(endIndex, 0, totalCount);
        var scrollerVisible = totalVisibleCount < totalCount;
        for (var i = _startIndex; _startIndex + index < endIndex; i++)
        {
            if (i >= _currentLog.Count)
                break;
            var log = _currentLog[i];

            if (log.LogType == _LogType.Log && !_showLog)
                continue;
            if (log.LogType == _LogType.Warning && !_showWarning)
                continue;
            if (log.LogType == _LogType.Error && !_showError)
                continue;
            if (log.LogType == _LogType.Assert && !_showError)
                continue;
            if (log.LogType == _LogType.Exception && !_showError)
                continue;

            if (index >= totalVisibleCount)
                break;

            GUIContent content = null;
            if (log.LogType == _LogType.Log)
                content = _logContent;
            else if (log.LogType == _LogType.Warning)
                content = _warningContent;
            else
                content = _errorContent;
            //content.text = log.condition ;

            var currentLogStyle = (_startIndex + index) % 2 == 0 ? _evenLogStyle : _oddLogStyle;
            if (log == _selectedLog)
                currentLogStyle = _selectedLogStyle;

            _tempContent.text = log.Count.ToString();
            var w = 0f;
            if (_collapse)
                w = _barStyle.CalcSize(_tempContent).x + 3;
            _countRect.x = Screen.width - w;
            _countRect.y = Size.y * i;
            if (beforeHeight > 0)
                _countRect.y += 8; //i will check later why
            _countRect.width = w;
            _countRect.height = Size.y;

            if (scrollerVisible)
                _countRect.x -= Size.x * 2;

            var sample = _samples[log.SampleId];
            _fpsRect = _countRect;
            if (_showFps)
            {
                _tempContent.text = sample.FpsText;
                w = currentLogStyle.CalcSize(_tempContent).x + Size.x;
                _fpsRect.x -= w;
                _fpsRect.width = Size.x;
                _fpsLabelRect = _fpsRect;
                _fpsLabelRect.x += Size.x;
                _fpsLabelRect.width = w - Size.x;
            }


            _memoryRect = _fpsRect;
            if (_showMemory)
            {
                _tempContent.text = sample.Memory.ToString("0.000");
                w = currentLogStyle.CalcSize(_tempContent).x + Size.x;
                _memoryRect.x -= w;
                _memoryRect.width = Size.x;
                _memoryLabelRect = _memoryRect;
                _memoryLabelRect.x += Size.x;
                _memoryLabelRect.width = w - Size.x;
            }
            _sceneRect = _memoryRect;
            if (_showScene)
            {
                _tempContent.text = sample.GetSceneName();
                w = currentLogStyle.CalcSize(_tempContent).x + Size.x;
                _sceneRect.x -= w;
                _sceneRect.width = Size.x;
                _sceneLabelRect = _sceneRect;
                _sceneLabelRect.x += Size.x;
                _sceneLabelRect.width = w - Size.x;
            }
            _timeRect = _sceneRect;
            if (_showTime)
            {
                _tempContent.text = sample.Time.ToString("0.000");
                w = currentLogStyle.CalcSize(_tempContent).x + Size.x;
                _timeRect.x -= w;
                _timeRect.width = Size.x;
                _timeLabelRect = _timeRect;
                _timeLabelRect.x += Size.x;
                _timeLabelRect.width = w - Size.x;
            }


            GUILayout.BeginHorizontal(currentLogStyle);
            if (log == _selectedLog)
            {
                GUILayout.Box(content, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
                GUILayout.Label(log.Condition, _selectedLogFontStyle);
                //GUILayout.FlexibleSpace();
                if (_showTime)
                {
                    GUI.Box(_timeRect, _showTimeContent, currentLogStyle);
                    GUI.Label(_timeLabelRect, sample.Time.ToString("0.000"), currentLogStyle);
                }
                if (_showScene)
                {
                    GUI.Box(_sceneRect, _showSceneContent, currentLogStyle);
                    GUI.Label(_sceneLabelRect, sample.GetSceneName(), currentLogStyle);
                }
                if (_showMemory)
                {
                    GUI.Box(_memoryRect, _showMemoryContent, currentLogStyle);
                    GUI.Label(_memoryLabelRect, sample.Memory.ToString("0.000") + " mb", currentLogStyle);
                }
                if (_showFps)
                {
                    GUI.Box(_fpsRect, _showFpsContent, currentLogStyle);
                    GUI.Label(_fpsLabelRect, sample.FpsText, currentLogStyle);
                }
            }
            else
            {
                if (GUILayout.Button(content, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y)))
                    _selectedLog = log;
                if (GUILayout.Button(log.Condition, _logButtonStyle))
                    _selectedLog = log;
                //GUILayout.FlexibleSpace();
                if (_showTime)
                {
                    GUI.Box(_timeRect, _showTimeContent, currentLogStyle);
                    GUI.Label(_timeLabelRect, sample.Time.ToString("0.000"), currentLogStyle);
                }
                if (_showScene)
                {
                    GUI.Box(_sceneRect, _showSceneContent, currentLogStyle);
                    GUI.Label(_sceneLabelRect, sample.GetSceneName(), currentLogStyle);
                }
                if (_showMemory)
                {
                    GUI.Box(_memoryRect, _showMemoryContent, currentLogStyle);
                    GUI.Label(_memoryLabelRect, sample.Memory.ToString("0.000") + " mb", currentLogStyle);
                }
                if (_showFps)
                {
                    GUI.Box(_fpsRect, _showFpsContent, currentLogStyle);
                    GUI.Label(_fpsLabelRect, sample.FpsText, currentLogStyle);
                }
            }
            if (_collapse)
                GUI.Label(_countRect, log.Count.ToString(), _barStyle);
            GUILayout.EndHorizontal();
            index++;
        }

        var afterHeight = (int) ((totalCount - (_startIndex + totalVisibleCount)) * Size.y);
        if (afterHeight > 0)
        {
            //fill invisible gap after scroller to make proper scroller pos
            GUILayout.BeginHorizontal(GUILayout.Height(afterHeight));
            GUILayout.Label(" ");
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        _buttomRect.x = 0f;
        _buttomRect.y = Screen.height - Size.y;
        _buttomRect.width = Screen.width;
        _buttomRect.height = Size.y;

        if (_showGraph)
            DrawGraph();
        else
            DrawStack();
    }


    private readonly float _graphSize = 4f;
    private int _startFrame;
    private int _currentFrame;
    private Vector3 _tempVector1;
    private Vector3 _tempVector2;
    private Vector2 _graphScrollerPos;
    private float _maxFpsValue;
    private float _minFpsValue;
    private float _maxMemoryValue;
    private float _minMemoryValue;

    private void DrawGraph()
    {
        _graphRect = _stackRect;
        _graphRect.height = Screen.height * 0.25f; //- size.y ;


        //startFrame = samples.Count - (int)(Screen.width / graphSize) ;
        //if( startFrame < 0 ) startFrame = 0 ;
        GUI.skin = _graphScrollerSkin;

        var drag = GetDrag();
        if (_graphRect.Contains(new Vector2(_downPos.x, Screen.height - _downPos.y)))
        {
            if (drag.x != 0)
            {
                _graphScrollerPos.x -= drag.x - _oldDrag3;
                _graphScrollerPos.x = Mathf.Max(0, _graphScrollerPos.x);
            }

            var p = _downPos;
            if (p != Vector2.zero)
                _currentFrame = _startFrame + (int) (p.x / _graphSize);
        }

        _oldDrag3 = drag.x;
        GUILayout.BeginArea(_graphRect, _backStyle);

        _graphScrollerPos = GUILayout.BeginScrollView(_graphScrollerPos);
        _startFrame = (int) (_graphScrollerPos.x / _graphSize);
        if (_graphScrollerPos.x >= _samples.Count * _graphSize - Screen.width)
            _graphScrollerPos.x += _graphSize;

        GUILayout.Label(" ", GUILayout.Width(_samples.Count * _graphSize));
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        _maxFpsValue = 0;
        _minFpsValue = 100000;
        _maxMemoryValue = 0;
        _minMemoryValue = 100000;
        for (var i = 0; i < Screen.width / _graphSize; i++)
        {
            var index = _startFrame + i;
            if (index >= _samples.Count)
                break;
            var s = _samples[index];
            if (_maxFpsValue < s.Fps) _maxFpsValue = s.Fps;
            if (_minFpsValue > s.Fps) _minFpsValue = s.Fps;
            if (_maxMemoryValue < s.Memory) _maxMemoryValue = s.Memory;
            if (_minMemoryValue > s.Memory) _minMemoryValue = s.Memory;
        }

        //GUI.BeginGroup(graphRect);


        if (_currentFrame != -1 && _currentFrame < _samples.Count)
        {
            var selectedSample = _samples[_currentFrame];
            GUILayout.BeginArea(_buttomRect, _backStyle);
            GUILayout.BeginHorizontal();

            GUILayout.Box(_showTimeContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.Time.ToString("0.0"), _nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(_showSceneContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.GetSceneName(), _nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(_showMemoryContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.Memory.ToString("0.000"), _nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(_showFpsContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.FpsText, _nonStyle);
            GUILayout.Space(Size.x);

            /*GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
			GUILayout.Label( currentFrame.ToString() ,nonStyle  );*/
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        _graphMaxRect = _stackRect;
        _graphMaxRect.height = Size.y;
        GUILayout.BeginArea(_graphMaxRect);
        GUILayout.BeginHorizontal();

        GUILayout.Box(_showMemoryContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Label(_maxMemoryValue.ToString("0.000"), _nonStyle);

        GUILayout.Box(_showFpsContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
        GUILayout.Label(_maxFpsValue.ToString("0.000"), _nonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        _graphMinRect = _stackRect;
        _graphMinRect.y = _stackRect.y + _stackRect.height - Size.y;
        _graphMinRect.height = Size.y;
        GUILayout.BeginArea(_graphMinRect);
        GUILayout.BeginHorizontal();

        GUILayout.Box(_showMemoryContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));

        GUILayout.Label(_minMemoryValue.ToString("0.000"), _nonStyle);


        GUILayout.Box(_showFpsContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));

        GUILayout.Label(_minFpsValue.ToString("0.000"), _nonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        //GUI.EndGroup();
    }

    private void DrawStack()
    {
        if (_selectedLog != null)
        {
            var drag = GetDrag();
            if (drag.y != 0 && _stackRect.Contains(new Vector2(_downPos.x, Screen.height - _downPos.y)))
                _scrollPosition2.y += drag.y - _oldDrag2;
            _oldDrag2 = drag.y;


            GUILayout.BeginArea(_stackRect, _backStyle);
            _scrollPosition2 = GUILayout.BeginScrollView(_scrollPosition2);
            Sample selectedSample = null;
            try
            {
                selectedSample = _samples[_selectedLog.SampleId];
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(_selectedLog.Condition, _stackLabelStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(Size.y * 0.25f);
            GUILayout.BeginHorizontal();
            GUILayout.Label(_selectedLog.Stacktrace, _stackLabelStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(Size.y);
            GUILayout.EndScrollView();
            GUILayout.EndArea();


            GUILayout.BeginArea(_buttomRect, _backStyle);
            GUILayout.BeginHorizontal();

            GUILayout.Box(_showTimeContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.Time.ToString("0.000"), _nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(_showSceneContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.GetSceneName(), _nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(_showMemoryContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.Memory.ToString("0.000"), _nonStyle);
            GUILayout.Space(Size.x);

            GUILayout.Box(_showFpsContent, _nonStyle, GUILayout.Width(Size.x), GUILayout.Height(Size.y));
            GUILayout.Label(selectedSample.FpsText, _nonStyle);
            /*GUILayout.Space( size.x );
			GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
			GUILayout.Label( selectedLog.sampleId.ToString() ,nonStyle  );*/
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        else
        {
            GUILayout.BeginArea(_stackRect, _backStyle);
            GUILayout.EndArea();
            GUILayout.BeginArea(_buttomRect, _backStyle);
            GUILayout.EndArea();
        }
    }


    public void OnGuiDraw()
    {
        if (!Show)
            return;

        _screenRect.x = 0;
        _screenRect.y = 0;
        _screenRect.width = Screen.width;
        _screenRect.height = Screen.height;

        GetDownPos();


        _logsRect.x = 0f;
        _logsRect.y = Size.y * 2f;
        _logsRect.width = Screen.width;
        _logsRect.height = Screen.height * 0.75f - Size.y * 2f;

        _stackRectTopLeft.x = 0f;
        _stackRect.x = 0f;
        _stackRectTopLeft.y = Screen.height * 0.75f;
        _stackRect.y = Screen.height * 0.75f;
        _stackRect.width = Screen.width;
        _stackRect.height = Screen.height * 0.25f - Size.y;


        _detailRect.x = 0f;
        _detailRect.y = Screen.height - Size.y * 3;
        _detailRect.width = Screen.width;
        _detailRect.height = Size.y * 3;

        if (_currentView == ReportView.Info)
        {
            DrawInfo();
        }
        else if (_currentView == ReportView.Logs)
        {
            DrawToolBar();
            DrawLogs();
        }
    }

    private readonly List<Vector2> _gestureDetector = new List<Vector2>();
    private Vector2 _gestureSum = Vector2.zero;
    private float _gestureLength;
    private int _gestureCount;

    private bool IsGestureDone()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length != 1)
            {
                _gestureDetector.Clear();
                _gestureCount = 0;
            }
            else
            {
                if (Input.touches[0].phase == TouchPhase.Canceled || Input.touches[0].phase == TouchPhase.Ended)
                {
                    _gestureDetector.Clear();
                }
                else if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    var p = Input.touches[0].position;
                    if (_gestureDetector.Count == 0 || (p - _gestureDetector[_gestureDetector.Count - 1]).magnitude > 10)
                        _gestureDetector.Add(p);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                _gestureDetector.Clear();
                _gestureCount = 0;
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    var p = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    if (_gestureDetector.Count == 0 || (p - _gestureDetector[_gestureDetector.Count - 1]).magnitude > 10)
                        _gestureDetector.Add(p);
                }
            }
        }

        if (_gestureDetector.Count < 10)
            return false;

        _gestureSum = Vector2.zero;
        _gestureLength = 0;
        var prevDelta = Vector2.zero;
        for (var i = 0; i < _gestureDetector.Count - 2; i++)
        {
            var delta = _gestureDetector[i + 1] - _gestureDetector[i];
            var deltaLength = delta.magnitude;
            _gestureSum += delta;
            _gestureLength += deltaLength;

            var dot = Vector2.Dot(delta, prevDelta);
            if (dot < 0f)
            {
                _gestureDetector.Clear();
                _gestureCount = 0;
                return false;
            }

            prevDelta = delta;
        }

        var gestureBase = (Screen.width + Screen.height) / 4;

        if (!(_gestureLength > gestureBase) || !(_gestureSum.magnitude < gestureBase / 2)) return false;
        _gestureDetector.Clear();
        _gestureCount++;
        return _gestureCount >= NumOfCircleToShow;
    }

    private float _lastClickTime = -1;

    private bool IsDoubleClickDone()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length != 1)
            {
                _lastClickTime = -1;
            }
            else
            {
                if (Input.touches[0].phase != TouchPhase.Began) return false;
                if (_lastClickTime == -1)
                {
                    _lastClickTime = Time.realtimeSinceStartup;
                }
                else if (Time.realtimeSinceStartup - _lastClickTime < 0.2f)
                {
                    _lastClickTime = -1;
                    return true;
                }
                else
                {
                    _lastClickTime = Time.realtimeSinceStartup;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                if (_lastClickTime == -1)
                {
                    _lastClickTime = Time.realtimeSinceStartup;
                }
                else if (Time.realtimeSinceStartup - _lastClickTime < 0.2f)
                {
                    _lastClickTime = -1;
                    return true;
                }
                else
                {
                    _lastClickTime = Time.realtimeSinceStartup;
                }
        }
        return false;
    }

    //calculate  pos of first click on screen
    private Vector2 _startPos;

    private Vector2 _downPos;

    private Vector2 GetDownPos()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Began)
            {
                _downPos = Input.touches[0].position;
                return _downPos;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                _downPos.x = Input.mousePosition.x;
                _downPos.y = Input.mousePosition.y;
                return _downPos;
            }
        }

        return Vector2.zero;
    }
    //calculate drag amount , this is used for scrolling

    private Vector2 _mousePosition;

    private Vector2 GetDrag()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length != 1)
                return Vector2.zero;
            return Input.touches[0].position - _downPos;
        }
        if (Input.GetMouseButton(0))
        {
            _mousePosition = Input.mousePosition;
            return _mousePosition - _downPos;
        }
        return Vector2.zero;
    }

    //calculate the start index of visible log
    private void CalculateStartIndex()
    {
        _startIndex = (int) (_scrollPosition.y / Size.y);
        _startIndex = Mathf.Clamp(_startIndex, 0, _currentLog.Count);
    }

    // For FPS Counter
    private int _frames;

    private bool _firstTime = true;
    private float _lastUpdate;
    private const int RequiredFrames = 10;
    private const float UpdateInterval = 0.25f;

#if UNITY_CHANGE1
	float lastUpdate2 = 0;
#endif

    private void DoShow()
    {
        Show = true;
        _currentView = ReportView.Logs;
        gameObject.AddComponent<ReporterGUI>();


        try
        {
            gameObject.SendMessage("OnShowReporter");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void Update()
    {
        FpsText = Fps.ToString("0.000");
        _gcTotalMemory = (float) GC.GetTotalMemory(false) / 1024 / 1024;
        //addSample();

#if UNITY_CHANGE3
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex != -1 && string.IsNullOrEmpty(_scenes[sceneIndex]))
            _scenes[SceneManager.GetActiveScene().buildIndex] = SceneManager.GetActiveScene().name;
#else
		int sceneIndex = Application.loadedLevel;
		if (sceneIndex != -1 && string.IsNullOrEmpty(scenes[Application.loadedLevel]))
			scenes[Application.loadedLevel] = Application.loadedLevelName;
#endif

        CalculateStartIndex();
        if (!Show && IsGestureDone())
            DoShow();


        if (_threadedLogs.Count > 0)
            lock (_threadedLogs)
            {
                foreach (var l in _threadedLogs)
                {
                    AddLog(l.Condition, l.Stacktrace, (LogType) l.LogType);
                }
                _threadedLogs.Clear();
            }

#if UNITY_CHANGE1
		float elapsed2 = Time.realtimeSinceStartup - lastUpdate2;
		if (elapsed2 > 1) {
			lastUpdate2 = Time.realtimeSinceStartup;
			//be sure no body else take control of log 
			Application.RegisterLogCallback (new Application.LogCallback (CaptureLog));
			Application.RegisterLogCallbackThreaded (new Application.LogCallback (CaptureLogThread));
		}
#endif

        // FPS Counter
        if (_firstTime)
        {
            _firstTime = false;
            _lastUpdate = Time.realtimeSinceStartup;
            _frames = 0;
            return;
        }
        _frames++;
        var dt = Time.realtimeSinceStartup - _lastUpdate;
        if (dt > UpdateInterval && _frames > RequiredFrames)
        {
            Fps = _frames / dt;
            _lastUpdate = Time.realtimeSinceStartup;
            _frames = 0;
        }
    }


    private void CaptureLog(string condition, string stacktrace, LogType type)
    {
        AddLog(condition, stacktrace, type);
    }

    private void AddLog(string condition, string stacktrace, LogType type)
    {
        var memUsage = 0f;
        var _condition = "";
        if (_cachedString.ContainsKey(condition))
        {
            _condition = _cachedString[condition];
        }
        else
        {
            _condition = condition;
            _cachedString.Add(_condition, _condition);
            memUsage += string.IsNullOrEmpty(_condition) ? 0 : _condition.Length * sizeof(char);
            memUsage += IntPtr.Size;
        }
        string _stacktrace;
        if (_cachedString.ContainsKey(stacktrace))
        {
            _stacktrace = _cachedString[stacktrace];
        }
        else
        {
            _stacktrace = stacktrace;
            _cachedString.Add(_stacktrace, _stacktrace);
            memUsage += string.IsNullOrEmpty(_stacktrace) ? 0 : _stacktrace.Length * sizeof(char);
            memUsage += IntPtr.Size;
        }

        var newLogAdded = false;

        AddSample();
        var log = new Log
        {
            LogType = (_LogType) type,
            Condition = _condition,
            Stacktrace = _stacktrace,
            SampleId = _samples.Count - 1
        };
        memUsage += log.GetMemoryUsage();
        //memUsage += samples.Count * 13 ;

        _logsMemUsage += memUsage / 1024 / 1024;

        if (TotalMemUsage > MaxSize)
        {
            Clear();
            Debug.Log("Memory Usage Reach" + MaxSize + " mb So It is Cleared");
            return;
        }

        var isNew = false;
        //string key = _condition;// + "_!_" + _stacktrace ;
        if (_logsDic.ContainsKey(_condition, stacktrace))
        {
            _logsDic[_condition][stacktrace].Count++;
        }
        else
        {
            isNew = true;
            _collapsedLogs.Add(log);
            _logsDic[_condition][stacktrace] = log;

            switch (type)
            {
                case LogType.Log:
                    _numOfCollapsedLogs++;
                    break;
                case LogType.Warning:
                    _numOfCollapsedLogsWarning++;
                    break;
                default:
                    _numOfCollapsedLogsError++;
                    break;
            }
        }

        switch (type)
        {
            case LogType.Log:
                _numOfLogs++;
                break;
            case LogType.Warning:
                _numOfLogsWarning++;
                break;
            default:
                _numOfLogsError++;
                break;
        }


        _logs.Add(log);
        if (!_collapse || isNew)
        {
            var skip = log.LogType == _LogType.Log && !_showLog || log.LogType == _LogType.Warning && !_showWarning || log.LogType == _LogType.Error && !_showError || log.LogType == _LogType.Assert && !_showError || log.LogType == _LogType.Exception && !_showError;

            if (!skip)
                if (string.IsNullOrEmpty(filterText) || log.Condition.ToLower().Contains(filterText.ToLower()))
                {
                    _currentLog.Add(log);
                    newLogAdded = true;
                }
        }

        if (newLogAdded)
        {
            CalculateStartIndex();
            var totalCount = _currentLog.Count;
            var totalVisibleCount = (int) (Screen.height * 0.75f / Size.y);
            if (_startIndex >= totalCount - totalVisibleCount)
                _scrollPosition.y += Size.y;
        }

        try
        {
            gameObject.SendMessage("OnLog", log);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private readonly List<Log> _threadedLogs = new List<Log>();

    private void CaptureLogThread(string condition, string stacktrace, LogType type)
    {
        var log = new Log {Condition = condition, Stacktrace = stacktrace, LogType = (_LogType) type};
        lock (_threadedLogs)
        {
            _threadedLogs.Add(log);
        }
    }

    //new scene is loaded
    private void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
    {
        if (_clearOnNewSceneLoaded)
            Clear();

#if UNITY_CHANGE3
        _currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Scene " + SceneManager.GetActiveScene().name + " is loaded");
#else
		currentScene = Application.loadedLevelName;
		Debug.Log("Scene " + Application.loadedLevelName + " is loaded");
#endif
    }

    //save user config
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Reporter_currentView", (int) _currentView);
        PlayerPrefs.SetInt("Reporter_show", Show ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_collapse", _collapse ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_clearOnNewSceneLoaded", _clearOnNewSceneLoaded ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showTime", _showTime ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showScene", _showScene ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showMemory", _showMemory ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showFps", _showFps ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showGraph", _showGraph ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showLog", _showLog ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showWarning", _showWarning ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showError", _showError ? 1 : 0);
        PlayerPrefs.SetString("Reporter_filterText", filterText);
        PlayerPrefs.SetFloat("Reporter_size", Size.x);

        PlayerPrefs.SetInt("Reporter_showClearOnNewSceneLoadedButton", _showClearOnNewSceneLoadedButton ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showTimeButton", _showTimeButton ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showSceneButton", _showSceneButton ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showMemButton", _showMemButton ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showFpsButton", _showFpsButton ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showSearchText", _showSearchText ? 1 : 0);

        PlayerPrefs.Save();
    }

    //read build information 
    private IEnumerator ReadInfo()
    {
        const string prefFile = "build_info.txt";
        var url = prefFile;

        if (prefFile.IndexOf("://", StringComparison.Ordinal) == -1)
        {
            var streamingAssetsPath = Application.streamingAssetsPath;
            if (streamingAssetsPath == "")
                streamingAssetsPath = Application.dataPath + "/StreamingAssets/";
            url = Path.Combine(streamingAssetsPath, prefFile);
        }

        if (Application.platform != RuntimePlatform.WebGLPlayer)
            if (!url.Contains("://"))
                url = "file://" + url;


        // float startTime = Time.realtimeSinceStartup;
        var www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
            Debug.LogError(www.error);
        else
            _buildDate = www.text;
    }
}