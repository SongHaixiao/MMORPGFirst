using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Common.Data;

public class MapTools
{
    [MenuItem("Map Tools/Export Teleportes")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if(current.isDirty)
        {
            EditorUtility.DisplayDialog("Tip", "Please save current scene!", "Confirm");
            return;
        }

        List<TeleporterObject> allTeleporters = new List<TeleporterObject>();

        foreach(var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if(!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene {0} not existed !", sceneFile);
                continue;
            }

            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();

            foreach(var teleporter in teleporters)
            {
                if(!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("Error", string.Format("Map : {0} Teleporter : [{1}] is not existed !",
                                                            map.Value.Resource, teleporter.ID), "Confirm");
                    return;
                }

                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if (def.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("Error", string.Format("Map : {0} Teleporter : [{1}] MapID : {2} Error!",
                                                            map.Value.Resource, teleporter.ID, def.MapID), " Confirm");
                    return;
                }

                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }

        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("Tip", "Teleporters are Exported !", "Confirm");

    }
}
