using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDraw : MonoBehaviour {
	public RawImage rawImage;
    public Transform pointLayer;
    public GameObject pointNull_Prefab;
    public GameObject pointLed_Prefab;
    //	
    public Texture2D texture;

    PresetPic presetPic;
    
    void Start0 () {
		presetPic = PresetPic.LoadSetting ("D-001");
        //if (presetPic != null) {
        //	DrawPic (presetPic);
        //}
        //
        StartCoroutine ("CaptureCamera");
    }

    int radis = 0;
    float runTime = 0;
    float angle = 0;
    void Update0 () {
        if (Input.GetKeyDown (KeyCode.Space)) {
            if (presetPic != null) {
                DrawPic (presetPic);
            }
            Debug.Log ("presetPic: " + presetPic + ", W: " + Set.setVal.Width + ", H: " + Set.setVal.Height);
        }

        runTime += Time.deltaTime;
        if (runTime >= 0.05f) {
            runTime -= 0.05f;
            if (++radis >= 50) {
                radis = 0;
            }
            //DrawYuan (0, 0, radis, 2);
            //
            angle += 5;
            if (angle >= 360) {
                angle -= 360;
            }
            XieXian (0, 0, angle, 20, 1);
        }
    }

    List<GameObject> list_Point = new List<GameObject> ();    
    public void DrawYuan (int x, int y, int r, int w) {
        ClearList ();

        int widthOne = 50;        
        
        GameObject prefab = pointLed_Prefab;
        float dis;
        for (int i = x - r; i <= x + r; i++) {
            for (int j = y - r; j <= y + r; j++) {
                dis = Vector2.Distance (new Vector2 (i, j), new Vector2 (x, y));
                if (dis >= r - w && dis <= r) {
                    GameObject led = Instantiate (prefab, pointLayer);
                    led.transform.localPosition = new Vector3 (x + j * widthOne, y + i * widthOne);
                    list_Point.Add (led);
                }
            }
        }
    }

    public void XieXian (int x, int y, float angle, int radis, int w) {
        ClearList ();

        int widthOne = 50;
        angle = angle * Mathf.PI / 180;
        float raiox = Mathf.Cos (angle);
        float raioy = Mathf.Sin (angle);
        float lx, ly;

        GameObject prefab = pointLed_Prefab;
        float dis;

        for (int r = -radis; r <= radis; r++) {
            lx = x + r * raiox;
            ly = y + r * raioy;

            for (int i = (int)lx - w; i <= lx + w; i++) {
                for (int j = (int)ly - w; j <= ly + w; j++) {
                    dis = Vector2.Distance (new Vector2 (i, j), new Vector2 (lx, ly));
                    if (dis < w) {
                        GameObject led = Instantiate (prefab, pointLayer);
                        led.transform.localPosition = new Vector3 (lx + j * widthOne, ly + i * widthOne);
                        list_Point.Add (led);
                    }
                }
            }
        }
    }

    void ClearList () {
        for (int i = 0; i < list_Point.Count; i++) {
            if (list_Point[i] != null) {
                Destroy (list_Point[i]);
            }
        }
        list_Point.Clear ();
    }
    public void DrawPic (PresetPic presetPic) {
        ClearList ();

        if (texture == null) {
            texture = new Texture2D ((int)rawImage.rectTransform.sizeDelta.x, (int)rawImage.rectTransform.sizeDelta.y, TextureFormat.RGB24, false);
            rawImage.texture = texture;
        }
        if (presetPic == null)
            return;        
        GameObject prefab;
        byte[] dataBuf = presetPic.dataBuff;
        int widthOne = 50;
        int id;
        int bufId;

        int offsetY = (Set.setVal.Height - 1) * widthOne;
        int width = widthOne * Set.setVal.Width;
        int height = widthOne * Set.setVal.Height;

        pointLayer.gameObject.SetActive (true);
        for (int i = 0; i < Set.setVal.Height && i < PresetPic.PIC_HEIGHT; i++) {
            for (int j = 0; j < Set.setVal.Width && j < PresetPic.PIC_WIDTH; j++) {
                id = i * Set.setVal.Width + j;
                bufId = i * PresetPic.PIC_WIDTH + j;
                if (bufId >= dataBuf.Length)
                    continue;
                if (dataBuf[bufId] == 0) {                    
                    prefab = pointNull_Prefab;
                } else {                    
                    prefab = pointLed_Prefab;
                }
                GameObject led = Instantiate (prefab, pointLayer);
                led.transform.localPosition = new Vector3 (j * widthOne - (width - widthOne) * 0.5f, i * widthOne + (height - widthOne) * 0.5f - offsetY);
                list_Point.Add (led);
            }
        }
        float scalex = (rawImage.rectTransform.sizeDelta.x - 4) / width;
        float scaley = (rawImage.rectTransform.sizeDelta.y - 4) / height;
        float scale = Mathf.Clamp (Mathf.Min (scalex, scaley), 0, 1);
        pointLayer.transform.localScale = Vector3.one * scale;

        rawImage.gameObject.SetActive (false);
        StartCoroutine ("CaptureCamera");
    }

    IEnumerator CaptureCamera () {
        //pointLayer.gameObject.SetActive (false);
        yield return new WaitForEndOfFrame ();        
        if (texture == null) {
            texture = new Texture2D ((int)rawImage.rectTransform.sizeDelta.x, (int)rawImage.rectTransform.sizeDelta.y, TextureFormat.RGB24, false);
            rawImage.texture = texture;
        }
        float x = transform.position.x - rawImage.rectTransform.sizeDelta.x * 0.5f;
        float y = transform.position.y - rawImage.rectTransform.sizeDelta.y * 0.5f;        
        texture.ReadPixels (new Rect(x, y, texture.width, texture.height), 0, 0);
        texture.Apply ();
        pointLayer.gameObject.SetActive (false);
        rawImage.gameObject.SetActive (true);

        ClearList ();
    }
}
