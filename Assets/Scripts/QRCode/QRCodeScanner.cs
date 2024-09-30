using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.UIElements;

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

    [Header("Osopher Settings ----------")]
    [SerializeField]
    private GameOsopherDict _gameOsopherDict; 
    [SerializeField]
    private GameObject[] _panels;
    private ChangeCardPanel _changeCardPanel;
    private int _osopherNum = 3;

    [Header("Camera Settings ----------")]
    private bool _isCamAvailable;
    private WebCamTexture _camTex;

    [Header("Scene Settings ----------")]
    [SerializeField]
    private SceneController _sceneController;

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

    /// <summary>
    /// Determines if a camera is available
    /// Plays camera feedback to background
    /// </summary>
    private void SetUpCam() {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0) {
            _isCamAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++) {
            // Uncomment for mobile build
            // if (!devices[i].isFrontFacing) {
            //     _camTex = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
            // }

            // Comment for mobile build
            _camTex = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
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
        _background.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
        // bool mirrored = _camTex.videoVerticallyMirrored;
        // if (mirrored)
        // {
        //     _background.rectTransform.localEulerAngles = new Vector3(0, 180, orientation);
        // }
        // else
        // {
        //     _background.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
        // }
    }

    /// <summary>
    /// Scan wrapper function for neatness
    /// </summary>
    public void OnClickScan() {
        Scan();
    }

    /// <summary>
    /// Reads in a QR code
    /// If valid QR code:
    /// Checks if it is an Osopher
    /// If true, Adds them to the player Osopher dict and
    /// Decrements the number of remaining Osophers to scan
    /// If false, outputs this as feedback
    /// If invalid QR code:
    /// Outputs this as feedback
    /// </summary>
    private void Scan() {
        try {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_camTex.GetPixels32(), _camTex.width, _camTex.height);

            if (result != null) {
                // Successfully read QR Code
                _text.text = result.Text; // change text for debugging

                // Check if QR Code text is a valid Osopher
                if (_gameOsopherDict.FindOsopher(result.Text)) {
                    _playerOsopherDict.AddOsopher(result.Text);
                    // Change card panel
                    _changeCardPanel.ChangeSprite(_panels[3-_osopherNum], _gameOsopherDict.GetOsopherSO(result.Text));
                    
                    // Manage scenes using osopher num
                    _osopherNum--;
                    if (_osopherNum == 0) {
                        _sceneController.GoToNextScene();
                    }
                }
                else {
                    _text.text = "Invalid Osopher!";
                }
                ResetCamera();
            } 
            else {
                _text.text = "FAILED TO READ QR CODE";
            }
            Debug.Log(result.Text);
        }
        catch {
            _text.text = "FAILED";
        }
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
}
