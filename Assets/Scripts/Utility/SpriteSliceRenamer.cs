using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class SpriteSliceRenamer : EditorWindow
{
    string newName;
    string oldName;

    int startIndex;
    int length;

    TextureImporter importer;
    static Object selectedObject
    {
        get
        {
            var selectedObject = Selection.activeObject;
            if (!AssetDatabase.Contains(selectedObject))
            {
                Debug.LogError("Selected object not found in Asset Database.");
                return null;
            }
            return selectedObject;
        }
    }

    [MenuItem("Assets/Rename Spritesheet")]
    public static void ShowSpriteSliceRenamerWindow()
    {
        EditorWindow.GetWindow<SpriteSliceRenamer>(true, "Rename Texture", true);
    }

    [MenuItem("Assets/Rename Spritesheet", true)]
    public static bool IsSelectionTexture()
    {
        if (Selection.activeObject == null)
        {
            return false;
        }

        if (Selection.objects.Length > 1)
        {
            return false;
        }

        return Selection.activeObject.GetType() == typeof(Texture2D);
    }

    void OnEnable()
    {
        ResetSettingsToSelection();
    }

    private void OnDisable()
    {
        importer = null;
    }

    void OnSelectionChange()
    {
        ResetSettingsToSelection();
    }

    void ResetSettingsToSelection()
    {
        importer = null;
        if (!IsSelectionTexture()) return;

        oldName = string.Format("{0}_%d", System.IO.Path.GetFileNameWithoutExtension(selectedObject.name));
        newName = oldName;

        startIndex = -1;
        length = 0;

        string path = AssetDatabase.GetAssetPath(selectedObject);
        importer = (TextureImporter)AssetImporter.GetAtPath(path);

        if (importer.textureType != TextureImporterType.Sprite)
            return;

        if (importer.spriteImportMode != SpriteImportMode.Multiple)
            return;
    }

    bool ShowInvalidSelection()
    {
        if (!IsSelectionTexture())
        {
            EditorGUILayout.HelpBox("The selected assets is not a texture.", MessageType.Warning);
            return true;
        }

        if (importer == null)
            return true;

        if (importer.textureType != TextureImporterType.Sprite)
        {
            EditorGUILayout.HelpBox("The selected texture import mode is not a sprite", MessageType.Warning);
            return true;
        }

        if (importer.spriteImportMode != SpriteImportMode.Multiple)
        {
            EditorGUILayout.HelpBox("The selected sprite import mode is not Multiple", MessageType.Warning);
            return true;
        }

        return false;
    }

    void OnGUI()
    {
        EditorGUILayout.HelpBox("This tool is used to rename sprite slices with a new name, the %i is used to get the slice index (probably it's based on which is created first)", MessageType.None);

        if (ShowInvalidSelection())
            return;

        oldName = EditorGUILayout.TextField("Previous Name", oldName);
        newName = EditorGUILayout.TextField("New Name", newName);
        EditorGUILayout.Space();

        startIndex = EditorGUILayout.IntField("Start Index", startIndex);
        length = EditorGUILayout.IntField("Length", length);

        if (GUILayout.Button("Rename"))
            RenameSelectedSprite();

        EditorGUILayout.BeginVertical();
        
        for (int i = startIndex; i < startIndex + length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(importer.spritesheet[i].name);
            EditorGUILayout.LabelField(" -> ");
            EditorGUILayout.LabelField(ReplaceName(importer.spritesheet[i].name, i - startIndex));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    string ReplaceName(string name, int index = -1)
    {
        string pattern = oldName;
        pattern = Regex.Replace(pattern, @"%d", @"(?<d>\d+)");
        
        string newName = Regex.Replace(this.newName, @"%d", @"${d}");
        if (index >= 0) newName = Regex.Replace(this.newName, @"%i", index.ToString());
        
        return Regex.Replace(name, pattern, newName);
    }

    void RenameSelectedSprite()
    {
        RenameSlices();
        AssetDatabase.Refresh();
    }

    void RenameSlices()
    {
        SpriteMetaData[] spriteDatas = importer.spritesheet;

        for (int i = startIndex; i < startIndex + length; i++)
        {
            spriteDatas[i].name = ReplaceName(spriteDatas[i].name, i - startIndex);
        }

        importer.spritesheet = spriteDatas;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }
}
