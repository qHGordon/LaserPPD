using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System;
using LaserPPD.Core;

public enum en_Change_MoveType
{
    Up = 0,         //	向上
    Down,           //	向下
    Left,           //	向左
    Right,          //	向右
    Fornt,          //
    Back,           //
}

public enum en_CSType
{
    None = 0,
    Loop,
    Destroy,
    Disable,
}
// Sprite 精灵的变换 ; 移动,缩放,透明度; 
public class ChangeSprite :MonoBehaviour
{
    public float lifeTime = 2f;
    public float delayTime = 0f;
    public en_CSType csType = en_CSType.Destroy;
    public bool autoPlay = true;
    public float[] pauseTime;
    // 透明曲线
    public bool transparentEnable;
    public AnimationCurve animationCurve_Trans;
    // 移动曲线
    public bool moveEnable;
    public float moveSpeed;
    public Vector3 moveDir = Vector3.up;
    public AnimationCurve animationCurve_MoveSpeed;
    //
    public bool localPositionEnable = false;
    public AnimationCurve animationCurve_PosX;
    public AnimationCurve animationCurve_PosY;
    public AnimationCurve animationCurve_PosZ;

    // 旋转
    public Vector3 rotateSpeed = Vector3.zero;
    // 缩放曲线
    public bool scaleEnable;
    public AnimationCurve animationCurve_Scale;
    // 缩放曲线3D
    public bool scale3DEnable;
    public AnimationCurve animationCurve_ScaleX;
    public AnimationCurve animationCurve_ScaleY;
    public AnimationCurve animationCurve_ScaleZ;

    Vector3 oldPos;
    int runCnt;
    float runTime = 0;
    float normalTime;
    float currDelayTime;
    float scale;
    public bool active = true;
    bool isPause;

    // 组件
    public SpriteRenderer[] spriteRenderers;    // 子对象的 
    Image[] images;
    Text[] texts;
    Tilemap[] tilemaps;

    //
    void Awake () {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer> ();
        images = GetComponentsInChildren<Image> ();
        texts = GetComponentsInChildren<Text> ();
        tilemaps = GetComponentsInChildren<Tilemap> ();

        oldPos = transform.position;
    }

    void OnEnable () {
        GameStart ();
        active = autoPlay;
    }
    public void GetSprites () {
        Awake ();
    }
    public void GameStart () {
        active = true;
        isPause = false;
        runCnt = 0;
        runTime = 0;
        normalTime = 0;
        Update_NormalTime ();
        currDelayTime = delayTime;
    }
    public void PlayContinue () {
        isPause = false;
    }
    public void PlayContinue (bool force) {
        if (force && runCnt < pauseTime.Length) {
            runTime = lifeTime * pauseTime[runCnt];
            runCnt++;
        }
        isPause = false;
    }

    // Update is called once per frame
    void Update () {
        if (active == false || isPause)
            return;
        if (currDelayTime > 0) {
            currDelayTime -= Time.deltaTime;
            return;
        }
        if (runTime >= lifeTime) {
            switch (csType) {
            case en_CSType.None:
                active = false;
                return;
            case en_CSType.Loop:
                runTime = 0;
                break;
            case en_CSType.Destroy:
                active = false;
                Destroy (gameObject);
                return;
            case en_CSType.Disable:
                active = false;
                gameObject.SetActive (false);
                return;
            }
        }
        runTime += Time.deltaTime;
        normalTime = Mathf.Clamp (runTime / lifeTime, 0f, 1f);
        if (runCnt < pauseTime.Length) {
            if (normalTime >= pauseTime[runCnt]) {
                normalTime = pauseTime[runCnt];
                isPause = true;
                runCnt++;
            }
        }
        //
        Update_NormalTime ();
    }

    void Update_NormalTime () {
        if (transparentEnable) {
            scale = animationCurve_Trans.Evaluate (normalTime);
            for (int i = 0; i < spriteRenderers.Length; i++) {
                if (spriteRenderers[i] == null)
                    continue;
                spriteRenderers[i].color = new Color (spriteRenderers[i].color.r, spriteRenderers[i].color.g, spriteRenderers[i].color.b, scale);
            }
            for (int i = 0; i < images.Length; i++) {
                if (images[i] == null)
                    continue;
                images[i].color = new Color (images[i].color.r, images[i].color.g, images[i].color.b, scale);
            }
            for (int i = 0; i < texts.Length; i++) {
                if (texts[i] == null)
                    continue;
                texts[i].color = new Color (texts[i].color.r, texts[i].color.g, texts[i].color.b, scale);
            }
            for (int i = 0; i < tilemaps.Length; i++) {
                if (tilemaps[i] == null)
                    continue;
                tilemaps[i].color = new Color (tilemaps[i].color.r, tilemaps[i].color.g, tilemaps[i].color.b, scale);
            }
        }
        if (moveEnable) {
            scale = animationCurve_MoveSpeed.Evaluate (normalTime);
            //transform.position = oldPos + moveDir * scale;
            transform.Translate (moveDir * scale * moveSpeed * Time.deltaTime);
        }
        if (rotateSpeed != Vector3.zero) {
            transform.Rotate (rotateSpeed * Time.deltaTime);
        }
        if (scaleEnable) {
            scale = animationCurve_Scale.Evaluate (normalTime);
            transform.localScale = new Vector3 (scale, scale, scale);
        }
        if (scale3DEnable) {
            transform.localScale = new Vector3 (animationCurve_ScaleX.Evaluate (normalTime), animationCurve_ScaleY.Evaluate (normalTime), animationCurve_ScaleZ.Evaluate (normalTime));
        }
        if (localPositionEnable) {
            transform.localPosition = new Vector3 (animationCurve_PosX.Evaluate (normalTime), animationCurve_PosY.Evaluate (normalTime), animationCurve_PosZ.Evaluate (normalTime));
        }
    }
}
