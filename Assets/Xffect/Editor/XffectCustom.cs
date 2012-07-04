using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Xffect))]
public class XffectCustom : Editor
{
    string LayerName = "EffectLayer";
    public override void OnInspectorGUI()
    {
        Xffect ctarget = (Xffect)target;
        EditorGUILayout.BeginVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        LayerName = EditorGUILayout.TextField("name:",LayerName);
        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        ctarget.LifeTime = EditorGUILayout.FloatField("life:", ctarget.LifeTime);
        if (GUILayout.Button("Add Layer"))
        {
            GameObject layer = new GameObject(LayerName);
            layer.AddComponent("EffectLayer");
            layer.transform.parent = Selection.activeTransform;
        }
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
