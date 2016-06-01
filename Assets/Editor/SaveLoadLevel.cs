using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class SaveLoadLevel : EditorWindow
{

    //function for window initialisation 
    [MenuItem("Window/Save-Load Level  %m")]
    static void TileBucketWindow()
    {
        EditorWindow.GetWindow(typeof(SaveLoadLevel));
    }

    //draws the UI
    void OnGUI()
    {
        if (GUI.Button(new Rect(5, 5, 100, 50), "Save Level"))
        {
            save();
        }
        if (GUI.Button(new Rect(5, 60, 100, 50), "Load Level"))
        {
            load();
        }
        if (GUI.Button(new Rect(200, 5, 100, 50), "Reset Level"))
        {
            reset();
        }
    }

    //saves the level
    private void save()
    {
        LevelTile[] level = TileBucket.instance.saveLevel();
        string path = EditorUtility.SaveFilePanel("Save Level", (Directory.GetCurrentDirectory() + "\\Assets\\Levels"), "NewLevel", "xml");
        if (path == string.Empty)
        {
            return;
        }

        XMLeditor xml = new XMLeditor(path);

        string[] data = new string[level.Length];
        int[] posX = new int[level.Length];
        int[] posY = new int[level.Length];

        for (int i = 0; i < level.Length; i++)
        {
            data[i] = level[i].tileName;
            posX[i] = (int)level[i].position.x;
            posY[i] = (int)level[i].position.y;
        }

        xml.saveLevelFile(path, data, posX, posY);
    }

    //loads the level
    private void load()
    {
        string path = EditorUtility.OpenFilePanel("Open Level", (Directory.GetCurrentDirectory() + "\\Assets\\Levels"), "xml");
        if (path == string.Empty)
        {
            return;
        }
        XMLeditor xml = new XMLeditor(path);
        List<LevelTile> levelTiles = new List<LevelTile>();

        for (int i = 0; i < xml.numberOfElements(); i++)
        {
            if (xml.findType(i) == "Tile")
            {
                levelTiles.Add(new LevelTile(
                               xml.findValue(i, "TileType"),
                               new Vector2(
                               Convert.ToInt32(xml.findValue(i, "Xpos")),
                               Convert.ToInt32(xml.findValue(i, "Ypos")))));
            }
        }

        TileBucket.instance.loadLevel(levelTiles.ToArray());
    }

    //resets the level
    private void reset()
    {
        TileBucket.instance.resetEditor();
    }
}
