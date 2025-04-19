using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    //BUG[x]: 在使用的时候 每次切换回物体，都会变成第一个场景
    private int _SceneIndex = -1;

    private GUIContent[] _SceneNames;

    readonly string[] ScenePathSpilit = { "/", ".unity" };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (EditorBuildSettings.scenes.Length == 0)
        {
            EditorGUI.LabelField(position, label.text, "No scenes in build settings");
            return;
        }

        if (_SceneNames == null || _SceneNames.Length == 0)
        {
            GetSceneNameArray(property);
        }

        int lastIndex = _SceneIndex;

        _SceneIndex = EditorGUI.Popup(position, label, _SceneIndex, _SceneNames);

        if (_SceneIndex != lastIndex)
        {
            property.stringValue = _SceneNames[_SceneIndex].text;
        }

        // property.stringValue = _SceneNames[_SceneIndex].text;
    }

    private void GetSceneNameArray(SerializedProperty property)
    {
        var scenes = EditorBuildSettings.scenes;

        _SceneNames = new GUIContent[scenes.Length];

        // 切割 得到场景名字
        for (int i = 0; i < scenes.Length; i++)
        {
            string path = scenes[i].path;
            string[] splitedPath = path.Split(ScenePathSpilit, System.StringSplitOptions.RemoveEmptyEntries);

            string sceneName = "";

            if (splitedPath.Length > 0)
            {
                sceneName = splitedPath[splitedPath.Length - 1];
            }
            else
            {
                sceneName = "{Deleted Scene}";
            }
            _SceneNames[i] = new GUIContent(sceneName);
        }
        if (_SceneNames.Length == 0)
        {
            _SceneNames = new[] { new GUIContent("Check your settings") };
        }

        if (!string.IsNullOrEmpty(property.stringValue))
        {
            for (int i = 0; i < _SceneNames.Length; i++)
            {
                if (property.stringValue == _SceneNames[i].text)
                {
                    _SceneIndex = i;
                    return;
                }
            }
        }
        _SceneIndex = 0;
        property.stringValue = _SceneNames[_SceneIndex].text;
    }
}