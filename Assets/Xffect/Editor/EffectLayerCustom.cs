using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EffectLayer))]
public class EffectLayerCustom : Editor
{
    public static bool DisplaySpriteConfig = false;
    public static bool DisplayRibbonConfig = false;
    public static bool DisplayOriginalVelocityConfig = false;
    public static bool DisplayEmitterConfig = false;
    public static bool DisplayUVConfig = false;
    public static bool DisplayRotateConfig = false;
    public static bool DisplayScaleConfig = false;
    public static bool DisplayColorConfig = false;

    public static bool DisplayAffectorConfig = false;
    public static bool DisplayLinearForceAffectorConfig = false;
    public static bool DisplayJetAffectorConfig = false;
    public static bool DisplayVortexAffectorConfig = false;
    public static bool DisplayAttractionAffectorConfig = false;

    public static float PreLableWidth = 150;

    string[] RenderTypes;
    string[] SpriteTypes;
    string[] SpriteUVStretchTypes;
    string[] OriginalPoint;
    string[] EmitTypes;
    string[] AffectorTypes;
    string[] UVTypes;
    string[] RotateTypes;
    string[] ScaleTypes;
    string[] ColorTypes;
    string[] ColorGradualTypes;
    string[] StretchTypes;
    
    int AffectorIndex;
    bool SameXY;

    public EffectLayerCustom()
    {
        RenderTypes = new string[2];
        RenderTypes[0] = "Sprite";
        RenderTypes[1] = "RibbonTrail";

        SpriteTypes = new string[3];
        SpriteTypes[0] = "Billboard";
        SpriteTypes[1] = "Billboard Self";
        SpriteTypes[2] = "XZ Plane";

        SpriteUVStretchTypes = new string[2];
        SpriteUVStretchTypes[0] = "VERTICAL";
        SpriteUVStretchTypes[1] = "HORIZONTAL";


        OriginalPoint = new string[9];
        OriginalPoint[0] = "Center";
        OriginalPoint[1] = "Left Up";
        OriginalPoint[2] = "Left Bottom";
        OriginalPoint[3] = "Right Bottom";
        OriginalPoint[4] = "Right Up";
        OriginalPoint[5] = "Bottom Center";
        OriginalPoint[6] = "Top Center";
        OriginalPoint[7] = "Left Center";
        OriginalPoint[8] = "Right Center";

        EmitTypes = new string[5];
        EmitTypes[0] = "Point";
        EmitTypes[1] = "Box";
        EmitTypes[2] = "Sphere Surface";
        EmitTypes[3] = "Circle";
        EmitTypes[4] = "Line";

        AffectorTypes = new string[4];
        AffectorTypes[0] = "LinearForceAffector";
        AffectorTypes[1] = "JetAffector";
        AffectorTypes[2] = "VortexAffector";
        AffectorTypes[3] = "AttractionAffector";


        UVTypes = new string[2];
        UVTypes[0] = "No UV Animation";
        UVTypes[1] = "Build UV Animation";
        //UVTypes[2] = "Load UV Animation From File";

        RotateTypes = new string[3];
        RotateTypes[0] = "None";
        RotateTypes[1] = "Simple";
        RotateTypes[2] = "Curve";

        ScaleTypes = new string[3];
        ScaleTypes[0] = "None";
        ScaleTypes[1] = "Simple";
        ScaleTypes[2] = "Curve";

        ColorTypes = new string[3];
        ColorTypes[0] = "None";
        ColorTypes[1] = "2Gradual";
        ColorTypes[2] = "4Gradual";

        ColorGradualTypes = new string[3];
        ColorGradualTypes[0] = "Clamp";
        ColorGradualTypes[1] = "Loop";
        ColorGradualTypes[2] = "Reverse";

        StretchTypes = new string[2];
        StretchTypes[0] = "Up to Bottom";
        StretchTypes[1] = "Left to Right";
    }

    public override void OnInspectorGUI()
    {
        EffectLayer ctarget = (EffectLayer)target;

        EditorGUIUtility.LookLikeControls(PreLableWidth);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("render type:");
        ctarget.RenderType = EditorGUILayout.Popup(ctarget.RenderType, RenderTypes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("client transform:");
        ctarget.ClientTransform = (Transform)EditorGUILayout.ObjectField(ctarget.ClientTransform, typeof(Transform),true);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("sync to client :");
        ctarget.SyncClient = EditorGUILayout.Toggle(ctarget.SyncClient);
        EditorGUILayout.EndHorizontal();


               
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("material :");
        ctarget.Material = (Material)EditorGUILayout.ObjectField(ctarget.Material, typeof(Material),false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(); 
        EditorGUILayout.BeginHorizontal();
        ctarget.StartTime = EditorGUILayout.FloatField("start time(s):", ctarget.StartTime);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        ctarget.MaxFps = EditorGUILayout.FloatField("Max FPS:", ctarget.MaxFps);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        if (ctarget.RenderType == 0)
            SpriteConfig(ctarget);
        else
            RibbonTrailConfig(ctarget);
        if (ctarget.RenderType == 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            RotateConfig(ctarget);
            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            ScaleConfig(ctarget);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        ColorConfig(ctarget);
        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        UVConfig(ctarget);
        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        EmitterConfig(ctarget);
        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        OriginalVelocityConfig(ctarget);
        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        AffectorConfig(ctarget);
        EditorGUILayout.Space();
        EditorGUILayout.Separator();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    protected void SpriteConfig(EffectLayer ctarget)
    {
        DisplaySpriteConfig = EditorGUILayout.Foldout(DisplaySpriteConfig, "Sprite Configuration");

        //DisplaySpriteConfig = EditorGUILayout.InspectorTitlebar(DisplaySpriteConfig, SpriteTitle);

        if (DisplaySpriteConfig)
        {
            EditorGUILayout.BeginVertical();
            
            ctarget.SpriteType = EditorGUILayout.Popup("sprite type:",ctarget.SpriteType, SpriteTypes);
            if (ctarget.SpriteType == 1)
            {//billboard self
                ctarget.SpriteUVStretch = EditorGUILayout.Popup("uv stretch:", ctarget.SpriteUVStretch, SpriteUVStretchTypes);
            }
            ctarget.OriPoint = EditorGUILayout.Popup("original point:", ctarget.OriPoint, OriginalPoint);
            ctarget.SpriteWidth = EditorGUILayout.FloatField("width:", ctarget.SpriteWidth);
            ctarget.SpriteHeight = EditorGUILayout.FloatField("height:", ctarget.SpriteHeight);
            EditorGUILayout.EndVertical();
        }
    }

    protected void RotateConfig(EffectLayer ctarget)
    {
        DisplayRotateConfig = EditorGUILayout.Foldout(DisplayRotateConfig, "Rotate Configuration");

        if (DisplayRotateConfig)
        {
            EditorGUILayout.BeginVertical();
            ctarget.RandomOriRot = EditorGUILayout.Toggle("random original rotation?", ctarget.RandomOriRot);
            if (ctarget.RandomOriRot)
            {
                ctarget.OriRotationMin = EditorGUILayout.IntField("original rotation min:", ctarget.OriRotationMin);
                ctarget.OriRotationMax = EditorGUILayout.IntField("original rotation max:", ctarget.OriRotationMax);
            }
            else
            {
                ctarget.OriRotationMin = EditorGUILayout.IntField("original rotation:", ctarget.OriRotationMin);
                ctarget.OriRotationMax = ctarget.OriRotationMin;
            }
            EditorGUILayout.Space();
            ctarget.RotateType = (RSTYPE)EditorGUILayout.Popup("rotate change type", (int)ctarget.RotateType, RotateTypes);
            if (ctarget.RotateType == RSTYPE.NONE)
            {
                ctarget.RotAffectorEnable = false;
            }
            else if (ctarget.RotateType == RSTYPE.SIMPLE)
            {
                ctarget.RotAffectorEnable = true;
                ctarget.DeltaRot = EditorGUILayout.FloatField("delta angle per second:", ctarget.DeltaRot);
            }
            else
            {
                ctarget.RotAffectorEnable = true;
                ctarget.RotateCurve = EditorGUILayout.CurveField("rotation curve:", ctarget.RotateCurve);
            }
            EditorGUILayout.EndVertical();
        }
    }

    protected void ColorConfig(EffectLayer ctarget)
    {
        DisplayColorConfig = EditorGUILayout.Foldout(DisplayColorConfig, "Color Configuration");

        if (DisplayColorConfig)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();
            ctarget.ColorAffectType = EditorGUILayout.Popup("color change type:", ctarget.ColorAffectType, ColorTypes);
            if (ctarget.ColorAffectType == 0)
            {
                ctarget.Color1 = EditorGUILayout.ColorField("original color:", ctarget.Color1);
                ctarget.ColorAffectorEnable = false;
            }
            else if (ctarget.ColorAffectType == 1)
            {
                ctarget.ColorAffectorEnable = true;
                ctarget.ColorGradualTimeLength = EditorGUILayout.FloatField("gradual time(-1 node life):", ctarget.ColorGradualTimeLength);
                ctarget.ColorGradualType = (COLOR_GRADUAL_TYPE)EditorGUILayout.Popup("gradual type:", (int)ctarget.ColorGradualType, ColorGradualTypes);
                EditorGUILayout.Space();
                ctarget.Color1 = EditorGUILayout.ColorField("color1:", ctarget.Color1);
                ctarget.Color2 = EditorGUILayout.ColorField("color2:", ctarget.Color2);
            }
            else
            {
                ctarget.ColorAffectorEnable = true;
                ctarget.ColorGradualTimeLength = EditorGUILayout.FloatField("gradual time(-1 node life):", ctarget.ColorGradualTimeLength);
                ctarget.ColorGradualType = (COLOR_GRADUAL_TYPE)EditorGUILayout.Popup("gradual type:", (int)ctarget.ColorGradualType, ColorGradualTypes);
                EditorGUILayout.Space();
                ctarget.Color1 = EditorGUILayout.ColorField("color1:", ctarget.Color1);
                ctarget.Color2 = EditorGUILayout.ColorField("color2:", ctarget.Color2);
                ctarget.Color3 = EditorGUILayout.ColorField("color3:", ctarget.Color3);
                ctarget.Color4 = EditorGUILayout.ColorField("color4:", ctarget.Color4);
            }
            EditorGUILayout.EndVertical();
        }
    }

    protected void ScaleConfig(EffectLayer ctarget)
    {
        DisplayScaleConfig = EditorGUILayout.Foldout(DisplayScaleConfig, "Scale Configuration");

        if (DisplayScaleConfig)
        {
            EditorGUILayout.BeginVertical();
            ctarget.RandomOriScale = EditorGUILayout.Toggle("random original scale?", ctarget.RandomOriScale);
            if (ctarget.RandomOriScale)
            {
                ctarget.OriScaleXMin  = EditorGUILayout.FloatField("original x scale min:", ctarget.OriScaleXMin);
                ctarget.OriScaleXMax = EditorGUILayout.FloatField("original x scale max:", ctarget.OriScaleXMax);
                EditorGUILayout.Space();
                ctarget.OriScaleYMin = EditorGUILayout.FloatField("original y scale min:", ctarget.OriScaleYMin);
                ctarget.OriScaleYMax = EditorGUILayout.FloatField("original y scale max:", ctarget.OriScaleYMax);
            }
            else
            {
                ctarget.OriScaleXMin = EditorGUILayout.FloatField("original x scale:", ctarget.OriScaleXMin);
                ctarget.OriScaleYMin = EditorGUILayout.FloatField("original y scale:", ctarget.OriScaleYMin);
                ctarget.OriScaleXMax = ctarget.OriScaleXMin;
                ctarget.OriScaleYMax = ctarget.OriScaleYMin;
            }
            EditorGUILayout.Space();
            ctarget.ScaleType = (RSTYPE)EditorGUILayout.Popup("scale change type:", (int)ctarget.ScaleType, ScaleTypes);
            if (ctarget.ScaleType == RSTYPE.NONE)
            {
                ctarget.ScaleAffectorEnable = false;
            }
            else if (ctarget.ScaleType == RSTYPE.SIMPLE)
            {
                ctarget.ScaleAffectorEnable = true;
                ctarget.DeltaScaleX = EditorGUILayout.FloatField("delta scaleX per second:", ctarget.DeltaScaleX);
                ctarget.DeltaScaleY = EditorGUILayout.FloatField("delta scaleY per second:", ctarget.DeltaScaleY);
            }
            else
            {
                ctarget.ScaleAffectorEnable = true;
                ctarget.ScaleXCurve = EditorGUILayout.CurveField("scaleX curve:", ctarget.ScaleXCurve);
                ctarget.ScaleYCurve = EditorGUILayout.CurveField("scaleY curve:", ctarget.ScaleYCurve);
            }
            EditorGUILayout.EndVertical();
        }
    }

    protected void RibbonTrailConfig(EffectLayer ctarget)
    {
        DisplayRibbonConfig = EditorGUILayout.Foldout(DisplayRibbonConfig, "RibbonTrail Configuration");

        if (DisplayRibbonConfig)
        {
            EditorGUILayout.BeginVertical();
            ctarget.RibbonWidth = EditorGUILayout.FloatField("width:", ctarget.RibbonWidth);
            ctarget.MaxRibbonElements = EditorGUILayout.IntField("max elements:", ctarget.MaxRibbonElements);
            ctarget.RibbonLen = EditorGUILayout.FloatField("trail length:", ctarget.RibbonLen);
            ctarget.StretchType = EditorGUILayout.Popup("uv stretch type:", ctarget.StretchType,StretchTypes);
            EditorGUILayout.EndVertical();
        }
    }

    protected void EmitterConfig(EffectLayer ctarget)
    {
        DisplayEmitterConfig = EditorGUILayout.Foldout(DisplayEmitterConfig, "Emitter Configuration");

        if (DisplayEmitterConfig)
        {
            EditorGUILayout.BeginVertical();

            ctarget.EmitType = EditorGUILayout.Popup("emitter type:", ctarget.EmitType, EmitTypes);
            if (ctarget.EmitType == 0)
            {
                ctarget.EmitPoint = EditorGUILayout.Vector3Field("emit position:", ctarget.EmitPoint);
            }
            else if (ctarget.EmitType == 1)
            {
                ctarget.EmitPoint = EditorGUILayout.Vector3Field("box center:", ctarget.EmitPoint);
                ctarget.BoxSize = EditorGUILayout.Vector3Field("box size:", ctarget.BoxSize);
            }
            else if (ctarget.EmitType == 2)
            {
                ctarget.EmitPoint = EditorGUILayout.Vector3Field("sphere center:", ctarget.EmitPoint);
                ctarget.Radius = EditorGUILayout.FloatField("radius:", ctarget.Radius);
            }
            else if (ctarget.EmitType == 3)
            {
                ctarget.EmitPoint = EditorGUILayout.Vector3Field("circle center:", ctarget.EmitPoint);
                ctarget.CircleDir = EditorGUILayout.Vector3Field("circle direction:", ctarget.CircleDir);
                ctarget.Radius = EditorGUILayout.FloatField("radius:", ctarget.Radius);
            }
            else if (ctarget.EmitType == 4)
            {
                ctarget.EmitPoint = EditorGUILayout.Vector3Field("line center:", ctarget.EmitPoint);
                ctarget.LineLengthLeft = EditorGUILayout.FloatField("line left length:", ctarget.LineLengthLeft);
                ctarget.LineLengthRight = EditorGUILayout.FloatField("line right length:", ctarget.LineLengthRight);
            }
            EditorGUILayout.Space();

            ctarget.MaxENodes = EditorGUILayout.IntField("max nodes:", ctarget.MaxENodes);
            ctarget.IsNodeLifeLoop = EditorGUILayout.Toggle("is node life loop:", ctarget.IsNodeLifeLoop);
            if (!ctarget.IsNodeLifeLoop)
            {
                ctarget.NodeLifeMin = EditorGUILayout.FloatField("node life min:", ctarget.NodeLifeMin);
                ctarget.NodeLifeMax = EditorGUILayout.FloatField("node life max:", ctarget.NodeLifeMax);
            }

            ctarget.IsEmitByDistance = EditorGUILayout.Toggle("emit by distance:", ctarget.IsEmitByDistance);
            if (ctarget.IsEmitByDistance)
            {
                ctarget.DiffDistance = EditorGUILayout.FloatField("diff distance:", ctarget.DiffDistance);
            }
            else
            {
                ctarget.ChanceToEmit = EditorGUILayout.Slider("chance to emit per loop:", ctarget.ChanceToEmit, 1, 100);
                ctarget.EmitDuration = EditorGUILayout.FloatField("emit duration:", ctarget.EmitDuration);
                ctarget.EmitRate = EditorGUILayout.IntField("emit rate,based on emit duration:", ctarget.EmitRate);
                ctarget.EmitLoop = EditorGUILayout.IntField("emit loop count,-1 means forever:", ctarget.EmitLoop);
                ctarget.EmitDelay = EditorGUILayout.FloatField("delay after each loop:", ctarget.EmitDelay);
            }
            EditorGUILayout.EndVertical();
        }
    }
    protected void OriginalVelocityConfig(EffectLayer ctarget)
    {
        DisplayOriginalVelocityConfig = EditorGUILayout.Foldout(DisplayOriginalVelocityConfig, "Direction Configuration");

        if (DisplayOriginalVelocityConfig)
        {
            EditorGUILayout.BeginVertical();
            ctarget.IsRandomDir = EditorGUILayout.Toggle("is random direction:", ctarget.IsRandomDir);
            if (ctarget.IsRandomDir)
            {
                ctarget.OriVelocityAxis = EditorGUILayout.Vector3Field("original axis:", ctarget.OriVelocityAxis);
                ctarget.AngleAroundAxis = EditorGUILayout.IntField("angle around axis:", ctarget.AngleAroundAxis);
                ctarget.OriSpeed = EditorGUILayout.FloatField("original speed:", ctarget.OriSpeed);
            }
            else
            {
                ctarget.OriVelocityAxis = EditorGUILayout.Vector3Field("original direction:", ctarget.OriVelocityAxis);
                ctarget.OriSpeed = EditorGUILayout.FloatField("original velocity:", ctarget.OriSpeed);
            }
            if (ctarget.SpriteType == 1 || ctarget.SpriteType == 2)
                ctarget.AlongVelocity = EditorGUILayout.Toggle("dir along velocity?", ctarget.AlongVelocity);
            EditorGUILayout.EndVertical();
        }
    }


    protected void AffectorConfig(EffectLayer ctarget)
    {
        DisplayAffectorConfig = EditorGUILayout.Foldout(DisplayAffectorConfig, "Affector Configuration");

        DisplayLinearForceAffectorConfig = ctarget.LinearForceAffectorEnable;
        DisplayJetAffectorConfig = ctarget.JetAffectorEnable;
        DisplayVortexAffectorConfig = ctarget.VortexAffectorEnable;
        DisplayAttractionAffectorConfig = ctarget.AttractionAffectorEnable;
        
        
        
        if (DisplayAffectorConfig)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PrefixLabel("affector types:");
            AffectorIndex = EditorGUILayout.Popup(AffectorIndex, AffectorTypes);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Affector"))
            {
                if (AffectorIndex == 0)
                    DisplayLinearForceAffectorConfig = true;
                else if (AffectorIndex == 1)
                    DisplayJetAffectorConfig = true;
                else if (AffectorIndex == 2)
                    DisplayVortexAffectorConfig = true;
                else if (AffectorIndex == 3)
                    DisplayAttractionAffectorConfig = true;
            }
            if (GUILayout.Button("Delete Affector"))
            {
                if (AffectorIndex == 0)
                    DisplayLinearForceAffectorConfig = false;
                else if (AffectorIndex == 1)
                    DisplayJetAffectorConfig = false;
                else if (AffectorIndex == 2)
                    DisplayVortexAffectorConfig = false;
                else if (AffectorIndex == 3)
                    DisplayAttractionAffectorConfig = false;
            }
            EditorGUILayout.EndHorizontal();

            if (DisplayLinearForceAffectorConfig)
                LinearForceAffectorConfig(ctarget);
            if (DisplayJetAffectorConfig)
                JetAffectorConfig(ctarget);
            if (DisplayVortexAffectorConfig)
                VortexAffectorConfig(ctarget);
            if (DisplayAttractionAffectorConfig)
                AttractionAffectorConfig(ctarget);

            ctarget.LinearForceAffectorEnable = DisplayLinearForceAffectorConfig;
            ctarget.JetAffectorEnable = DisplayJetAffectorConfig;
            ctarget.VortexAffectorEnable = DisplayVortexAffectorConfig;
            ctarget.AttractionAffectorEnable = DisplayAttractionAffectorConfig;

            EditorGUILayout.EndVertical();
        }
    }

    protected void LinearForceAffectorConfig(EffectLayer ctarget)
    {
        DisplayLinearForceAffectorConfig = EditorGUILayout.Foldout(DisplayLinearForceAffectorConfig, "Linear Force Affector Configuration");
        if (DisplayLinearForceAffectorConfig)
        {
            EditorGUILayout.BeginVertical();
            ctarget.LinearForce = EditorGUILayout.Vector3Field("linear force:", ctarget.LinearForce);
            ctarget.LinearMagnitude = EditorGUILayout.FloatField("magnitude:", ctarget.LinearMagnitude);
            EditorGUILayout.EndVertical();
        }
    }

    protected void JetAffectorConfig(EffectLayer ctarget)
    {
        DisplayJetAffectorConfig = EditorGUILayout.Foldout(DisplayJetAffectorConfig, "Jet Force Affector Configuration");
        if (DisplayJetAffectorConfig)
        {
            EditorGUILayout.BeginVertical();
            ctarget.JetMin = EditorGUILayout.FloatField("jet min:", ctarget.JetMin);
            ctarget.JetMax = EditorGUILayout.FloatField("jet max:", ctarget.JetMax);
            EditorGUILayout.EndVertical();
        }
    }

    protected void VortexAffectorConfig(EffectLayer ctarget)
    {
        DisplayVortexAffectorConfig = EditorGUILayout.Foldout(DisplayVortexAffectorConfig, "Vortex Affector Configuration");
        if (DisplayVortexAffectorConfig)
        {
            EditorGUILayout.BeginVertical();
            ctarget.UseVortexCurve = EditorGUILayout.Toggle("use curve?", ctarget.UseVortexCurve);
            if (ctarget.UseVortexCurve)
            {
                ctarget.VortexCurve = EditorGUILayout.CurveField("magnitude curve:", ctarget.VortexCurve);
                ctarget.VortexDirection = EditorGUILayout.Vector3Field("direction:", ctarget.VortexDirection);
            }
            else
            {
                ctarget.VortexMag = EditorGUILayout.FloatField("magnitude:", ctarget.VortexMag);
                ctarget.VortexDirection = EditorGUILayout.Vector3Field("direction:", ctarget.VortexDirection);
            }

            EditorGUILayout.EndVertical();
        }
    }

    protected void AttractionAffectorConfig(EffectLayer ctarget)
    {
        DisplayAttractionAffectorConfig = EditorGUILayout.Foldout(DisplayAttractionAffectorConfig, "Attraction Affector Configuration");
        if (DisplayAttractionAffectorConfig)
        {
            EditorGUILayout.BeginVertical();
            ctarget.UseAttractCurve = EditorGUILayout.Toggle("use curve?", ctarget.UseAttractCurve);
            if (ctarget.UseAttractCurve)
            {
                ctarget.AttractionCurve = EditorGUILayout.CurveField("curve:", ctarget.AttractionCurve);
                ctarget.AttractionPosition = EditorGUILayout.Vector3Field("attraction position:", ctarget.AttractionPosition);
            }
            else
            {
                ctarget.AttractMag = EditorGUILayout.FloatField("magnitude:", ctarget.AttractMag);
                ctarget.AttractionPosition = EditorGUILayout.Vector3Field("attraction position:", ctarget.AttractionPosition);
            }

            EditorGUILayout.EndVertical();
        }
    }
    protected void UVConfig(EffectLayer ctarget)
    {
        DisplayUVConfig = EditorGUILayout.Foldout(DisplayUVConfig, "UV Configuration");

        if (DisplayUVConfig)
        {
            EditorGUILayout.BeginVertical();
            ctarget.UVType = EditorGUILayout.Popup(ctarget.UVType, UVTypes);
            if (ctarget.UVType == 0)
            {
                ctarget.OriLowerLeftUV = EditorGUILayout.Vector2Field("original lower left uv:", ctarget.OriLowerLeftUV);
                ctarget.OriUVDimensions = EditorGUILayout.Vector2Field("original uv dimensions:", ctarget.OriUVDimensions);
                ctarget.UVAffectorEnable = false;
            }
            else if (ctarget.UVType == 1)
            {
                ctarget.Cols = EditorGUILayout.IntField("x tile:", ctarget.Cols);
                ctarget.Rows = EditorGUILayout.IntField("y tile:", ctarget.Rows);
                ctarget.UVTime = EditorGUILayout.FloatField("time(-1 means node life):", ctarget.UVTime);
                ctarget.LoopCircles = EditorGUILayout.IntField("loop(-1 means infinite):", ctarget.LoopCircles);
                ctarget.UVAffectorEnable = true;
            }
            else
            {// support in the future.
                ctarget.EanPath = EditorGUILayout.TextField("file name:", ctarget.EanPath);
                ctarget.EanIndex = EditorGUILayout.IntField("animation index", ctarget.EanIndex);
                ctarget.UVTime = EditorGUILayout.FloatField("animation time:", ctarget.UVTime);
                ctarget.LoopCircles = EditorGUILayout.IntField("loop(-1 means infinite):", ctarget.LoopCircles);
                ctarget.UVAffectorEnable = true;
            }
            EditorGUILayout.EndVertical();
        }
    }
}
