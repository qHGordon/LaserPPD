using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorFont : EditorWindow {
    [MenuItem("Assets/Font Auto Edit Num", priority = 0)]
    static void FontAutoEdit_Num() {
        FontAutoEdit('0');
    }
    [MenuItem("Assets/Font Auto Edit Small Char", priority = 1)]
    static void FontAutoEdit_SmallChar() {
        FontAutoEdit('a');
    }
    [MenuItem("Assets/Font Auto Edit Big Char", priority = 2)]
    static void FontAutoEdit_BigChar() {
        FontAutoEdit('A');
    }

    // Use this for initialization
    static void FontAutoEdit (char startChar) {
        //Debug.Log("AutoConfig");

        //获取选中文件或者夹
        string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        //Debug.Log("Path: " + path);

        Font font_target = AssetDatabase.LoadAssetAtPath<Font>(path);
        //Debug.Log("font_target: " + font_target);
        if (font_target == null){
			Debug.LogError("没有找到Font!");
            return;
		}
        Material material = font_target.material;
        if (material == null){
			Debug.LogError("没有找到Material!");
            return;
		}
        //Debug.Log("material: " + material);
        Texture texture = material.mainTexture;
        if (texture == null){
			Debug.LogError("没有找到Texture!");
            return;
		}
        //Debug.Log("texture: " + texture);

        path = AssetDatabase.GetAssetPath(texture);
        //Debug.Log("texturePath: " + path);

        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
        //Debug.Log("objects: " + objects.Length);

        // 重写配置
        Sprite sprite;
		int index = 0;
        List<CharacterInfo> charInfoList = new List<CharacterInfo>();
        for (int i = 0; i < objects.Length; i++) {
            //Debug.Log("objectType: "+ objects[i].GetType());
            if (objects[i].GetType() != typeof(Sprite))
                continue;
            sprite = (Sprite)objects[i];

            CharacterInfo info = new CharacterInfo();
            info.index = startChar + index;
			index++;

            // 配置中的数据的坐标是左上角
            info.uvTopLeft = new Vector2(sprite.rect.x / texture.width, 1);
            info.uvTopRight = new Vector2(sprite.rect.right / texture.width, 1);
            info.uvBottomLeft = new Vector2(sprite.rect.x / texture.width , 0);
            info.uvBottomRight = new Vector2(sprite.rect.right / texture.width, 0);

            info.minX = 0;
            info.minY = -(int)sprite.rect.height;
            info.maxX = (int)sprite.rect.width;
            info.maxY = 0;

            //info.uv.x = sprite.rect.x / texture.width;
            //info.uv.y = 0;
            //info.uv.width = sprite.rect.width / texture.width;
            //info.uv.height = 1;

            //info.vert.x = 0;
            //info.vert.y = 0;
            //info.vert.width = sprite.rect.width;
            //info.vert.height = -sprite.rect.height;
            info.advance = (int)sprite.rect.width;

            //Debug.Log("Add: ");
            charInfoList.Add(info);
        }
        font_target.characterInfo = charInfoList.ToArray();

        EditorUtility.SetDirty(font_target);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }



}
