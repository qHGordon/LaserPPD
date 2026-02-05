using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_AnimSetOne : MonoBehaviour {
    public Text text_Title;
	public Menu_SetOne[] setOne;
    public Button button_Default;


    AnimOne animInfo;    
    int Id;
    int selectId;

    const int SETID_picType = 0;
    const int SETID_animMode = 1;
    const int SETID_color = 2;
    const int SETID_pointSta = 3;
    const int SETID_loop = 4;
    const int SETID_x = 5;
    const int SETID_y = 6;
    const int SETID_width = 7;
    const int SETID_height = 8;
    const int SETID_speed = 9;
    const int SETID_delayTime = 10;

    public void Awake0(int no) {
        Id = no;

        button_Default.onClick.AddListener (OnClick_Default);

        setOne[SETID_picType].SetValueInit(AnimOne.tab_picType);
        setOne[SETID_animMode].SetValueInit(AnimOne.tab_animMode);
        setOne[SETID_color].SetValueInit(AnimOne.tab_color);
        setOne[SETID_pointSta].SetValueInit(AnimOne.tab_pointSta);
        setOne[SETID_loop].SetValueInit(AnimOne.tab_loop);
        setOne[SETID_x].SetValueInit(AnimOne.tab_Pos);
        setOne[SETID_y].SetValueInit(AnimOne.tab_Pos);
        setOne[SETID_width].SetValueInit (AnimOne.SET_c_Size);
        setOne[SETID_height].SetValueInit (AnimOne.SET_c_Size);
        setOne[SETID_speed].SetValueInit(AnimOne.tab_speed);
        setOne[SETID_delayTime].SetValueInit (AnimOne.tab_delayTime);

        for (int i = 0; i < setOne.Length; i++) {
            setOne[i].Init (i, OnClick_SetOne);
        }
    }

	// Use this for initialization
	public void GameStart (AnimOne settineOne) {
        animInfo = settineOne;

        if (Set.setVal.Language == (int)en_Language.Chinese) {
            text_Title.text = "动画" + (Id + 1).ToString ();
        } else {
            text_Title.text = "Animation " + (Id + 1).ToString ();
        }
        
        RunStart ();
	}

	void RunStart () {
        Update_Languae ();
        Update_SetValue ();

        selectId = -1;
        Update_SelectId ();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Update_Languae () {
        if (Set.setVal.Language == (int)en_Language.Chinese) {

            setOne[SETID_picType].SetName ("图案类型");
            setOne[SETID_animMode].SetName ("运动模式");
            setOne[SETID_color].SetName ("颜色");
            setOne[SETID_pointSta].SetName ("点状态");
            setOne[SETID_loop].SetName ("循环");
            setOne[SETID_x].SetName ("开始坐标_x");
            setOne[SETID_y].SetName ("开始坐标_y");
            setOne[SETID_width].SetName ("宽度");
            setOne[SETID_height].SetName ("高度");
            setOne[SETID_speed].SetName ("速度");
            setOne[SETID_delayTime].SetName ("延时步数");

            // 图案类型
            setOne[SETID_picType].SetValueName ((int)enPicType.Rol, "横线");
            setOne[SETID_picType].SetValueName ((int)enPicType.Col, "竖线");
            setOne[SETID_picType].SetValueName ((int)enPicType.Rectangle, "矩形");
            setOne[SETID_picType].SetValueName ((int)enPicType.Prismatic, "棱形");
            // 运动模式
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.LeftToRight, "从左到右");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.RightToLeft, "从右到左");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.LRRL, "左右来回");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.UpToDown, "从上到下");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.DownToUp, "从下到上");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.UDDU, "上下来回");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.SmallToBig, "从小到大");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.BigToSmall, "从大到小");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.SBBS, "大小来回");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.TurnLeft, "左转圈");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.TurnRight, "右转圈");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.TurnLR, "左右来回转");
            // 颜色
            setOne[SETID_color].SetValueName ((int)enCOLOR.NONE, "黑色");
            setOne[SETID_color].SetValueName ((int)enCOLOR.R, "红色");
            setOne[SETID_color].SetValueName ((int)enCOLOR.G, "绿色");
            setOne[SETID_color].SetValueName ((int)enCOLOR.B, "蓝色");
            setOne[SETID_color].SetValueName ((int)enCOLOR.RG, "红绿色");
            setOne[SETID_color].SetValueName ((int)enCOLOR.RB, "红蓝色");
            setOne[SETID_color].SetValueName ((int)enCOLOR.GB, "蓝绿色");
            setOne[SETID_color].SetValueName ((int)enCOLOR.RGB, "白色");

            // 点状态
            setOne[SETID_pointSta].SetValueName ((int)enPointSta.None, "无");
            setOne[SETID_pointSta].SetValueName ((int)enPointSta.Die, "死亡点");
            setOne[SETID_pointSta].SetValueName ((int)enPointSta.Target, "目标点");
            setOne[SETID_pointSta].SetValueName ((int)enPointSta.Rest, "休息点");
            setOne[SETID_pointSta].SetValueName ((int)enPointSta.MoveRest, "可变休息点");

            // 循环
            setOne[SETID_loop].SetValueName (0, "否");
            setOne[SETID_loop].SetValueName (1, "是");
        } else {
            setOne[SETID_picType].SetName ("Graphic type");
            setOne[SETID_animMode].SetName ("Move type");
            setOne[SETID_color].SetName ("Color");
            setOne[SETID_pointSta].SetName ("Point status");
            setOne[SETID_loop].SetName ("Loop");
            setOne[SETID_x].SetName ("Start point-x");
            setOne[SETID_y].SetName ("Start point-y");
            setOne[SETID_width].SetName ("Width");
            setOne[SETID_height].SetName ("Height");
            setOne[SETID_speed].SetName ("Speed value");
            setOne[SETID_delayTime].SetName ("Delay steps");

            // 图案类型
            setOne[SETID_picType].SetValueName ((int)enPicType.Rol, "Horizontal line");
            setOne[SETID_picType].SetValueName ((int)enPicType.Col, "Vertical line");
            setOne[SETID_picType].SetValueName ((int)enPicType.Rectangle, "Rectangle");
            setOne[SETID_picType].SetValueName ((int)enPicType.Prismatic, "Prismatic");
            // 运动模式
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.LeftToRight, "Left to right");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.RightToLeft, "Right to left");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.LRRL, "Left-right loop");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.UpToDown, "Top to bottom");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.DownToUp, "Bottom to top");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.UDDU, "Top-bottom loop");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.SmallToBig, "Small to large");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.BigToSmall, "Large to small");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.SBBS, "Small-large loop");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.TurnLeft, "Left turn circle");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.TurnRight, "Right turn circle");
            setOne[SETID_animMode].SetValueName ((int)enAnimMode.TurnLR, "Left-Right turn loop");
            // 颜色
            setOne[SETID_color].SetValueName ((int)enCOLOR.NONE, "Black");
            setOne[SETID_color].SetValueName ((int)enCOLOR.R, "Red");
            setOne[SETID_color].SetValueName ((int)enCOLOR.G, "Green");
            setOne[SETID_color].SetValueName ((int)enCOLOR.B, "Blue");
            setOne[SETID_color].SetValueName ((int)enCOLOR.RG, "Red-Green");
            setOne[SETID_color].SetValueName ((int)enCOLOR.RB, "Red-Blue");
            setOne[SETID_color].SetValueName ((int)enCOLOR.GB, "Blue-Green");
            setOne[SETID_color].SetValueName ((int)enCOLOR.RGB, "White");

            // 点状态
            setOne[SETID_pointSta].SetValueName ((int)enPointSta.None, "None");
            setOne[SETID_pointSta].SetValueName ((int)enPointSta.Die, "Die");
            setOne[SETID_pointSta].SetValueName ((int)enPointSta.Target, "Target");
            setOne[SETID_pointSta].SetValueName ((int)enPointSta.Rest, "Rest");
            setOne[SETID_pointSta].SetValueName ((int)enPointSta.MoveRest, "Variable Rest");

            // 循环
            setOne[SETID_loop].SetValueName (0, "No");
            setOne[SETID_loop].SetValueName (1, "Yes");
        }
    }

    void Update_SetValue () {
        setOne[SETID_picType].UpdateValue ((int)animInfo.picType);
        setOne[SETID_animMode].UpdateValue ((int)animInfo.animMode);
        setOne[SETID_color].UpdateValue ((int)animInfo.color);
        setOne[SETID_pointSta].UpdateValue ((int)animInfo.pointSta);
        setOne[SETID_loop].UpdateValue (animInfo.loop);
        setOne[SETID_x].UpdateValue (animInfo.x);
        setOne[SETID_y].UpdateValue (animInfo.y);
        setOne[SETID_width].UpdateValue (animInfo.width);
        setOne[SETID_height].UpdateValue (animInfo.height);
        setOne[SETID_speed].UpdateValue (animInfo.speed);
        setOne[SETID_delayTime].UpdateValue (animInfo.delayTime);
    }

    public string GetAutoDescripts () {
        return setOne[SETID_picType].GetValueName () + "-" + setOne[SETID_animMode].GetValueName ();
    }
    public void GetSetting(AnimOne settingOne) {
        settingOne.picType = (enPicType)setOne[SETID_picType].GetValue ();
        settingOne.animMode = (enAnimMode) setOne[SETID_animMode].GetValue ();
        settingOne.color = (enCOLOR)setOne[SETID_color].GetValue ();
        settingOne.pointSta = (enPointSta)setOne[SETID_pointSta].GetValue ();
        settingOne.loop = setOne[SETID_loop].GetValue ();
        settingOne.x = setOne[SETID_x].GetValue ();
        settingOne.y = setOne[SETID_y].GetValue ();
        settingOne.width = setOne[SETID_width].GetValue ();
        settingOne.height = setOne[SETID_height].GetValue ();
        settingOne.speed = setOne[SETID_speed].GetValue ();
        settingOne.delayTime = setOne[SETID_delayTime].GetValue ();
    }

    public bool Equals (AnimOne settingOne) {
        if (settingOne == null)
            return false;
        if (settingOne.picType != (enPicType)setOne[SETID_picType].GetValue ())
            return false;
        if (settingOne.animMode != (enAnimMode)setOne[SETID_animMode].GetValue ())
            return false;
        if (settingOne.color != (enCOLOR)setOne[SETID_color].GetValue ())
            return false;
        if (settingOne.pointSta != (enPointSta)setOne[SETID_pointSta].GetValue ())
            return false;
        if (settingOne.loop != setOne[SETID_loop].GetValue ())
            return false;
        if (settingOne.x != setOne[SETID_x].GetValue ())
            return false;
        if (settingOne.y != setOne[SETID_y].GetValue ())
            return false;
        if (settingOne.width != setOne[SETID_width].GetValue ())
            return false;
        if (settingOne.height != setOne[SETID_height].GetValue ())
            return false;
        if (settingOne.speed != setOne[SETID_speed].GetValue ())
            return false;
        if (settingOne.delayTime != setOne[SETID_delayTime].GetValue ())
            return false;
        return true;
    }

    void Update_SelectId () {
        for (int i = 0; i < setOne.Length; i++) {
            if (i == selectId) {
                setOne[i].SetSelect (true);
            } else {
                setOne[i].SetSelect (false);
            }
        }
    }

    public void OnClick_SetOne (int id) {
        if (selectId == id)
            return;
        selectId = id;
        Update_SelectId ();
    }
    public void OnClick_Default () {
        AnimOne animTemp = new AnimOne();
        animTemp.Default ();
        GameStart (animTemp); 
    }
}
