using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class InventoryItemEditor : EditorWindow {
    
    public InventoryItemList inventoryItemList;
    private int viewIndex = 1;
    Vector2 scroll;
    
    [MenuItem ("Window/Inventory Item Editor %#e")]
    static void  Init () 
    {
        EditorWindow.GetWindow (typeof (InventoryItemEditor));
    }
    
    void  OnEnable () {
        if(EditorPrefs.HasKey("ObjectPath")) 
        {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            inventoryItemList = AssetDatabase.LoadAssetAtPath (objectPath, typeof(InventoryItemList)) as InventoryItemList;
        }
        
    }
    
    void  OnGUI () {
        GUILayout.BeginHorizontal ();
        GUILayout.Label ("Inventory Item Editor", EditorStyles.boldLabel);
        if (inventoryItemList != null) {
            if (GUILayout.Button("Show Item List")) 
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = inventoryItemList;
            }
        }
        if (GUILayout.Button("Open Item List")) 
        {
                OpenItemList();
        }
        GUILayout.EndHorizontal ();
        
        if (inventoryItemList == null) 
        {
            GUILayout.BeginHorizontal ();
            GUILayout.Space(10);
            if (GUILayout.Button("Create New Item List", GUILayout.ExpandWidth(false))) 
            {
                CreateNewItemList();
            }
            if (GUILayout.Button("Open Existing Item List", GUILayout.ExpandWidth(false))) 
            {
                OpenItemList();
            }
            GUILayout.EndHorizontal ();
        }
            
            GUILayout.Space(20);
            
        if (inventoryItemList != null) 
        {
            GUILayout.BeginHorizontal ();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false))) 
            {
                if (viewIndex > 1)
                    viewIndex --;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false))) 
            {
                if (viewIndex < inventoryItemList.itemList.Count) 
                {
                    viewIndex ++;
                }
            }
            
            GUILayout.Space(60);
            
            if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false))) 
            {
                AddItem();
            }
            if (GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false))) 
            {
                DeleteItem(viewIndex - 1);
            }
            
            GUILayout.EndHorizontal ();
            if (inventoryItemList.itemList == null)
                Debug.Log("brainlet alert");
            if (inventoryItemList.itemList.Count > 0) 
            {
                GUILayout.BeginHorizontal ();
                viewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current Item", viewIndex, GUILayout.ExpandWidth(false)), 1, inventoryItemList.itemList.Count);
                //Mathf.Clamp (viewIndex, 1, inventoryItemList.itemList.Count);
                EditorGUILayout.LabelField ("of   " +  inventoryItemList.itemList.Count.ToString() + "  items", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal ();
                
                inventoryItemList.itemList[viewIndex-1].itemName = EditorGUILayout.TextField ("Item Name", inventoryItemList.itemList[viewIndex-1].itemName as string);
                inventoryItemList.itemList[viewIndex-1].itemIcon = EditorGUILayout.ObjectField ("Item Icon", inventoryItemList.itemList[viewIndex-1].itemIcon, typeof (Sprite), false) as Sprite;
                inventoryItemList.itemList[viewIndex-1].isUnique = (bool)EditorGUILayout.Toggle("Unique", inventoryItemList.itemList[viewIndex-1].isUnique, GUILayout.ExpandWidth(false));
               
                GUILayout.Space(10);
                GUILayout.Label ("Item Description");
                scroll = EditorGUILayout.BeginScrollView(scroll);
                inventoryItemList.itemList[viewIndex-1].itemDescription = EditorGUILayout.TextArea (inventoryItemList.itemList[viewIndex-1].itemDescription as string);
                EditorGUILayout.EndScrollView();
            } 
            else 
            {
                GUILayout.Label ("This Inventory List is Empty.");
            }
        }
        if (GUI.changed) 
        {
            EditorUtility.SetDirty(inventoryItemList);
        }
    }
    
    void CreateNewItemList () 
    {
        // There is no overwrite protection here!
        // There is No "Are you sure you want to overwrite your existing object?" if it exists.
        // This should probably get a string from the user to create a new name and pass it ...
        viewIndex = 1;
        inventoryItemList = CreateInventoryItemList.Create();
        if (inventoryItemList) 
        {
            inventoryItemList.itemList = new List<InventoryItem>();
            string relPath = AssetDatabase.GetAssetPath(inventoryItemList);
            EditorPrefs.SetString("ObjectPath", relPath);
        }
    }
    
    void OpenItemList () 
    {
        string absPath = EditorUtility.OpenFilePanel ("Select Inventory Item List", "", "");
        if (absPath.StartsWith(Application.dataPath)) 
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            inventoryItemList = AssetDatabase.LoadAssetAtPath (relPath, typeof(InventoryItemList)) as InventoryItemList;
            if (inventoryItemList.itemList == null)
                inventoryItemList.itemList = new List<InventoryItem>();
            if (inventoryItemList) {
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }
    }

    void AddItem () 
    {
        InventoryItem newItem = new InventoryItem();
        newItem.itemName = "New Item";
        inventoryItemList.itemList.Add (newItem);
        viewIndex = inventoryItemList.itemList.Count;
    }
    
    void DeleteItem (int index) 
    {
        inventoryItemList.itemList.RemoveAt (index);
    }
}