using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCardPanel : MonoBehaviour
{
    public void ChangeSprite(GameObject _panel, OsopherSO _osopher) {
        Image _image = _panel.GetComponent<Image>();
        _image.sprite = _osopher.osopherImage;
    }
}
