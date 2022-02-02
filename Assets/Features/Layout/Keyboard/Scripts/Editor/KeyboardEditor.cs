using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Keyboard))]
public class KeyboardEditor : Editor
{
    //KeyboardLayout selectedLayout;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("Generate Key Layout", EditorStyles.boldLabel);

        //selectedLayout = EditorGUILayout.ObjectField("Layout", selectedLayout, typeof(KeyboardLayout), false) as KeyboardLayout;
        var keyboard = this.target as Keyboard;

        if (keyboard.Layout != null && GUILayout.Button("Generate Layout \n(Will destroy existing keys)"))
        {
            generateLayout(keyboard);
        }
    }

    private void generateLayout(Keyboard keyboard)
    {
        var buttons = new List<KeyboardButton>();
        var keysContainer = getKeysContainer(keyboard);
        keysContainer.ClearChildrenImmediate();

        var totalSize = keyboard.SizeReference.localScale;
        
        float buttonSizeY = totalSize.y / keyboard.Layout.Rows.Length;
        float buttonSizeX = totalSize.x / keyboard.Layout.Rows.Max(i => i.Keys.Length);
        float buttonSize = Mathf.Min(buttonSizeX, buttonSizeY);
        Vector3 position = new Vector3();
        position.z = 0;

        var rows = keyboard.Layout.Rows;
        var rowOffset = (rows.Length - 1) * buttonSize / 2;
        for(int r=0;r<rows.Length;r++)
        {
            position.y = r * buttonSize - rowOffset;
            var row = rows[rows.Length - 1 - r]; //Reverse order

            position.x = -row.Keys.Sum(i => i.Width * buttonSize) / 2;
            for (int k=0;k<row.Keys.Length;k++)
            {
                var key = row.Keys[k];
                float keySize = buttonSize * key.Width;
                position.x += keySize;
                buttons.Add(addKey(keyboard, keysContainer, key, position - Vector3.right * (keySize / 2), buttonSize * 0.9f));
            }
        }

        keyboard.ButtonContainer = keysContainer;
        keyboard.Buttons = buttons.ToArray();
        EditorUtility.SetDirty(keyboard);
    }

    private KeyboardButton addKey(Keyboard keyboard, Transform container, KeyboardKey key, Vector3 position, float size)
    {
        var keyPrefab = PrefabUtility.InstantiatePrefab(keyboard.Layout.KeyPrefab) as GameObject;
        var keyButton = keyPrefab.GetComponent<KeyboardButton>();
        string keyText = string.IsNullOrEmpty(key.DisplayText) ? key.MainKey : key.DisplayText;
        keyButton.Keyboard = keyboard;
        keyButton.gameObject.name = "Key " + keyText.ToUpper();
        keyButton.transform.SetParent(container, false);
        keyButton.transform.localPosition = keyButton.LayoutLocalPosition = position;
        keyButton.transform.localScale = keyButton.LayoutLocalScale = Vector3.one * size;
        keyButton.SetWidth(key.Width);

        keyButton.transform.Find("Primary Label").GetComponent<TMPro.TMP_Text>().text = keyText;
        keyButton.transform.Find("Secondary Label").GetComponent<TMPro.TMP_Text>().text = key.SecondaryKey;
        keyButton.KeyInfo = key;
        Debug.Log("Setting keyinfo: " + key.MainKey + " " + key.SecondaryKey);
        EditorUtility.SetDirty(keyButton);

        return keyButton;
    }

    private Transform getKeysContainer(Keyboard keyboard)
    {
        var keysContainer = keyboard.transform.Find("Keys");
        if (keysContainer == null)
        {
            var newContainer = new GameObject("Keys");
            keysContainer = newContainer.transform;
            keysContainer.SetParent(keyboard.transform, false);
        }
        return keysContainer;
    }
}
