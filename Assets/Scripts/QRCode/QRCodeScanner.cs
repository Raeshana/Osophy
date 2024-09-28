using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class QRCodeScanner : MonoBehaviour
{
    [Header("Scan Settings")]
    [SerializeField]
    private RawImage _background;
    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;
    [SerializeField]
    private RectTransform _scanZone;

    [Header("Player Settings")]
    [SerializeField]
    private PlayerOsopherDict _playerOsopherDict;
    // Testing Feedback
    [SerializeField]
    private TextMeshProUGUI _text;

    [Header("Osopher Settings")]
    private GameOsopherDict _gameOsopherDict; 

    private bool _isCamAvailable;
    private WebCamTexture _camTex;

    void Awake () {
        _gameOsopherDict = GameObject.FindWithTag("OsopherManager").GetComponent<GameOsopherDict>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpCam();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamRenderer();
    }

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

    private void UpdateCamRenderer() {
        if (!_isCamAvailable) {
            return;
        }
        float ratio = (float)_camTex.width / (float)_camTex.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = -_camTex.videoRotationAngle;
        _background.rectTransform.localEulerAngles = new Vector3(0, 180, orientation);
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

    public void OnClickScan() {
        Scan();
    }

    private void Scan() {
        try {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_camTex.GetPixels32(), _camTex.width, _camTex.height);
            if (result != null) {
                // Successfully read QR Code
                _text.text = result.Text; // change text for debugging
                // Check if QR Code text is a valid Osopher
                if (_gameOsopherDict.osopherDict[result.Text]) {
                    _playerOsopherDict.AddOsopher(result.Text);
                }
            } 
            else {
                _text.text = "FAILED TO READ QR CODE";
            }
        }
        catch {
            _text.text = "FAILED";
        }
    }
}
