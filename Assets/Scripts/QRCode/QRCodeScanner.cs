using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.Android;

public class QRCodeScanner : MonoBehaviour
{
    [Header("Scan Settings ----------")]
    [SerializeField]
    private RawImage _background;
    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;
    [SerializeField]
    private RectTransform _scanZone;

    [Header("Player Settings ----------")]
    [SerializeField]
    private PlayerOsopherDict _playerOsopherDict;
    // Testing Feedback
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private PlayerDebater playerDebater;

    [Header("Osopher Settings ----------")]
    [SerializeField]
    private GameOsopherDict _gameOsopherDict; 
    [SerializeField]
    private GameObject[] _panels;
    private ChangeCardPanel _changeCardPanel;
    [SerializeField]
    private int _osopherNum;
    private int _osopherNumCurr;

    [Header("Camera Settings ----------")]
    private bool _isCamAvailable;
    private WebCamTexture _camTex;

    [Header("Scene Settings ----------")]
    [SerializeField]
    private SceneController _sceneController;

    // used to change the color of instructions box in InitialQRScanningScene
    public UnityEngine.UI.Image imageComponent;

    /// <summary>
    /// Initialize _osopherNumCurr
    /// </summary>
    void Awake() {
        _osopherNumCurr = _osopherNum;
    }

    /// <summary>
    /// Sets up the camera
    /// </summary>
    void Start()
    {
        SetUpCam();
        _changeCardPanel = GetComponent<ChangeCardPanel>();
        if (_panels == null || _panels.Length == 0) {
            Debug.LogError("Panels not set up correctly.");
        }
    }

    /// <summary>
    /// Ensures that camera feedback is real-time
    /// </summary>
    void Update()
    {
        UpdateCamRenderer();
    }

    private void RequestCamPermissions() {
        // Check if the user has granted camera permission
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            // Request the camera permission
            Permission.RequestUserPermission(Permission.Camera);
        }
    }

    /// <summary>
    /// Determines if a camera is available
    /// Plays camera feedback to background
    /// </summary>
    private void SetUpCam() {
        RequestCamPermissions();

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0) {
            _isCamAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++) {
            // Android/ IOS Builds
            if (!devices[i].isFrontFacing) {
                _camTex = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
            }

            // PC Build
            // _camTex = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
        }

        _camTex.Play();
        _background.texture = _camTex;
        _isCamAvailable = true;
    }

    /// <summary>
    /// Updates camera feedback
    /// </summary>
    private void UpdateCamRenderer() {
        if (!_isCamAvailable) {
            return;
        }
        float ratio = (float)_camTex.width / (float)_camTex.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = -_camTex.videoRotationAngle;

        // PC/ Android Builds
        // _background.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);

        // IOS Builds
        _background.rectTransform.localEulerAngles = new Vector3(0, 180, orientation);
    }

    /// <summary>
    /// Restarts camera on every scan
    /// Not necessary, used for debugging
    /// </summary>
    private void ResetCamera() {
        _camTex.Stop();
        _camTex.Play();
    }

    /// <summary>
    /// Disables camera after use
    /// </summary>
    private void OnDisable() {
        if (_camTex != null) {
            _camTex.Stop();
        }
    }

    /// <summary>
    /// Calls this function when scan is clicked during
    /// inital scanning
    /// </summary>
    public void OnlickInitialScan() {
        Scan(InitialScan);
    }

    /// <summary>
    /// Calls this function when scan is clicked during
    /// gameplay scanning
    /// </summary>
    public void OnlickGameplayScan() {
        Scan(GameplayScan);
    }

    /// <summary>
    /// Allows initial scan to be passed as a parameter
    /// </summary>
    /// <param name="text"></param>
    private delegate void InitialScanDelegate(string text);

    /// <summary>
    /// Called during initial scanning phase
    /// If an Osopher is valid, 
    /// Adds them to the player Osopher dict and
    /// Decrements the number of remaining Osophers to scan
    /// </summary>
    /// <param name="text"></param>
    private void InitialScan(string text) {
        if (_gameOsopherDict.FindOsopher(text)) {
            _playerOsopherDict.AddOsopher(text);
            ChangePanel(text);
            ManageOsopherNumInitial();
        }
        else {
            _text.text = "Oops, this isn't an Osopher! Scan again!";

            // CHANGE INSTRUCTIONS IMAGE COLOUR TO RED
        }
    }

    /// <summary>
    /// Allows gameplay scan to be passed as a parameter
    /// </summary>
    /// <param name="text"></param>
    private delegate void GameplayScanDelegate(string text);

    /// <summary>
    /// Called during gameplay scan phase
    /// If an Osopher is valid, 
    /// 
    /// Decrements the number of remaining Osophers to scan
    /// </summary>
    /// <param name="text"></param>
    private void GameplayScan(string text) {
        if (_gameOsopherDict.FindOsopher(text)) {
            if (_playerOsopherDict.FindOsopher(text)) {
                playerDebater.AssignDebater(text);
                ChangePanel(text);
                ManageOsopherNumGameplay();
            }   
            else {
                _text.text = "Oops, you don't have this Osopher! Scan again!";

                // CHANGE INSTRUCTIONS IMAGE COLOUR TO RED
            }
        }
        else {
            _text.text = "Oops, this isn't an Osopher! Scan again!";

            // CHANGE INSTRUCTIONS IMAGE COLOUR TO RED
        }
    }

    /// <summary>
    /// Change blank panels to scanned Osophers
    /// </summary>
    /// <param name="text"> Text read from QR code </param>
    private void ChangePanel(string text) {
        Debug.Log("Osopher num: " + _osopherNum + "Osopher num curr: " + _osopherNumCurr);
        _changeCardPanel.ChangeSprite(_panels[_osopherNum-_osopherNumCurr], _gameOsopherDict.GetOsopherSO(text));
    }

    /// <summary>
    /// Decrement _osopherNum when a valid
    /// Osopher is scanned in
    /// If all osophers were scanned, go to next scene
    /// </summary>
    private void ManageOsopherNumInitial() {
        _osopherNumCurr--;
        if (_osopherNumCurr <= 0) {
            _playerOsopherDict.UpdatePlayerOsophers();
            StartCoroutine(WaitForPlayerUpdates());
        }
    }

    /// <summary>
    /// Decrement _osopherNum when a valid
    /// Osopher is scanned in
    /// If all osophers were scanned, go to next scene
    /// </summary>
    private void ManageOsopherNumGameplay() {
        _osopherNumCurr--;
        if (_osopherNumCurr <= 0) {
            StartCoroutine(WaitForPlayerUpdates());
        }
    }

    private IEnumerator WaitForPlayerUpdates() {
        yield return new WaitForSeconds(2);
        _sceneController.GoToNextScene();
    }

    /// <summary>
    /// Reads in a QR code
    /// If valid QR code:
    /// Checks if it is an Osopher
    /// If true, calls function depending on 
    /// InitalScanning phase or GameplayScanning phase
    /// If false, outputs this as feedback
    /// If invalid QR code:
    /// Outputs this as feedback
    /// </summary>
    private void Scan(InitialScanDelegate function) {
        try {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_camTex.GetPixels32(), _camTex.width, _camTex.height);

            if (result != null) { // Successfully read QR Code
                _text.text = result.Text; // Change text for debugging

                function(result.Text); // Call InitialScan or GameplayScan

                // CHANGE INSTRUCTIONS IMAGE COLOUR TO GREEN
                imageComponent.color = new Color(0.388f, 0.647f, 0.231f, 1.0f);

                //ResetCamera(); // Reset camera for debugging
                //Debug.Log(result.Text);
            } 
            else {
                _text.text = "Oops! Scan again!";

                // CHANGE INSTRUCTIONS IMAGE COLOUR TO RED
                imageComponent.color = new Color(0.910f, 0.341f, 0.239f, 1.0f);
            }
        } catch (Exception ex) {
            _text.text = "Oops! Scan again!";
            Debug.LogError("Scan Error: " + ex.Message + "\n" + ex.StackTrace);

            // CHANGE INSTRUCTIONS IMAGE COLOUR TO RED
            imageComponent.color = new Color(0.910f, 0.341f, 0.239f, 1.0f);
    }
    }
}
