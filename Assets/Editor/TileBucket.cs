using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class TileBucket : EditorWindow
{
    private string[] tiles;
    private GameObject[] prefabs;
    private Vector2 scrollPos;
    private int selectedAsset = -1;
    private Vector2 mousePos;
    private bool addObject = false;
    private bool removeObject = false;
    private List<Vector2> foreground = new List<Vector2>();
    private List<Vector2> background = new List<Vector2>();
    private List<GameObject> objectList = new List<GameObject>();
    private GameObject tilesObject;

    //sets this object as a singleton
    private static TileBucket instanceOf;
    public static TileBucket instance
    {
        get
        {
            if (instanceOf == null)
            {
                instanceOf = GameObject.FindObjectOfType<TileBucket>();
            }
            return instanceOf;
        }
    }

    // Use this for initialization
    void Awake()
    {
        instanceOf = this;
        linkTileObject();
        findFiles();
    }

    //links to the prefab objects
    private void linkTileObject()
    {        
        if (GameObject.Find("Tiles") == true)
        {
            tilesObject = GameObject.Find("Tiles").gameObject;

            loadAllTilesInObject();
        }
        else
        {
            tilesObject = new GameObject("Tiles");
        }
    }

    //loads all tiles on the Tiles object
    private void loadAllTilesInObject()
    {
        foreach (Transform child in tilesObject.transform)
        {
            if (child.tag == "Obstacle" ||
                child.tag == "Building" ||
                child.tag == "Enemy" ||
                child.tag == "NPC" ||
                child.tag == "PlayerTile")
            {
                foreground.Add(new Vector2(child.gameObject.transform.position.x, child.gameObject.transform.position.y));
            }
            else
            {
                background.Add(new Vector2(child.gameObject.transform.position.x, child.gameObject.transform.position.y));
            }
            objectList.Add(child.gameObject);
        }
    }
    
    //finds all files in the resource directory
    private void findFiles()
    {
        List<string> tilesList = new List<string>();

        string dataPath = Directory.GetCurrentDirectory() + "\\Assets\\Resources";
        DirectoryInfo directory = new DirectoryInfo(dataPath);

        FileInfo[] info = directory.GetFiles("*.prefab");

        foreach (FileInfo file in info)
        {
            if (file.Name != "ProgramPrefab.prefab")
            {
                tilesList.Add(file.Name.Replace(".prefab", ""));
            }
        }

        tiles = tilesList.ToArray();
        prefabs = new GameObject[tiles.Length];

        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabs[i] = Resources.Load(tiles[i]) as GameObject;
        }
    }

    //function for window initialisation 
    [MenuItem("Window/Tile Bucket  %t")]
    static void TileBucketWindow()
    {
        EditorWindow.GetWindow(typeof(TileBucket));
    }

    // called when the box is opened
    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += mouseUpdate;
    }

    // called when the box is closed
    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= mouseUpdate;
    }

    //draws the UI
    void OnGUI()
    {
        //draws the scrolling box
        scrollPos = GUI.BeginScrollView(new Rect(2, 2, position.width, position.height), scrollPos, new Rect(0, 0, 0, (int)Math.Ceiling(tiles.Length / Math.Floor(position.width / 77)) * 77));

        //draws buttons
        int horizontal = -1;
        int vertical = 0;
        for (int i = 0; i < tiles.Length - 1; i++)
        {
            horizontal++;
            if ((horizontal * 77) + 77 > position.width)
            {
                vertical++;
                horizontal = 0;
            }
            if (selectedAsset == i)
            {
                GUI.enabled = false;
            }
            if (GUI.Button(new Rect((horizontal * 77), (vertical * 77), 75, 75), prefabs[i].GetComponent<SpriteRenderer>().sprite.texture))
            {
                selectedAsset = i;
            }
            GUI.enabled = true;
        }
        GUI.EndScrollView();
    }

    //updates the mouse variables
    private void mouseUpdate(SceneView sceneView)
    {
        Event currentEvent = Event.current;
        Ray worldRays = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
        mousePos = worldRays.origin;
        mousePos = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        if (currentEvent.type == EventType.layout)
        {
            HandleUtility.AddDefaultControl(controlID);
        }
        if (currentEvent.type == EventType.mouseDown)
        {
            if (currentEvent.button == 0)
            {
                addObject = true;
            }
            else if (currentEvent.button == 1)
            {
                removeObject = true;
            }
        }
        if (currentEvent.type == EventType.mouseUp)
        {
            if (currentEvent.button == 0)
            {
                addObject = false;
            }
            else if (currentEvent.button == 1)
            {
                removeObject = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tilesObject == null)
        {
            linkTileObject();
        }
        addObjectScene(mousePos);
        removeObjectFromScene();
    }

    //adds an object to the scene
    private void addObjectScene(Vector2 tilePos)
    {
        if (selectedAsset >= 0 && addObject == true && checkMultipleTiles(tilePos) == false)
        {
            if (checkForTile(tilePos) == true)
            {
                if (checkForeground(tilePos) == true && checkForeground(prefabs[selectedAsset]) == true)
                {
                    return;
                }
                if (checkForeground(tilePos) == false && checkForeground(prefabs[selectedAsset]) == false)
                {
                    return;
                }
            }

            if (checkForeground(prefabs[selectedAsset]) == true)
            {
                foreground.Add(new Vector2(tilePos.x, tilePos.y));
            }
            else if (checkForeground(prefabs[selectedAsset]) == false)
            {
                background.Add(new Vector2(tilePos.x, tilePos.y));
            }
            prefabs[selectedAsset].transform.position = tilePos;
            objectList.Add(Instantiate(prefabs[selectedAsset]));
            objectList[objectList.Count - 1].name = prefabs[selectedAsset].name;
            makeChild(tilesObject, objectList[objectList.Count - 1]);
        }
    }

    //checks if a tile is on a specific area in the editor
    private bool checkForTile(Vector2 area)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i].transform.position == new Vector3(area.x, area.y, 0))
            {
                return true;
            }
        }
        return false;
    }

    //checks for multiple tiles in an area
    private bool checkMultipleTiles(Vector2 tilePos)
    {
        bool forground = false;
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i].transform.position == new Vector3(tilePos.x, tilePos.y, 0))
            {
                if (objectList[i].tag == "Obstacle" ||
                    objectList[i].tag == "Building" ||
                    objectList[i].tag == "Enemy" ||
                    objectList[i].tag == "NPC" ||
                    objectList[i].tag == "PlayerTile")
                {
                    forground = true;
                }
            }
        }
        if (forground == true)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                if (objectList[i].transform.position == new Vector3(tilePos.x, tilePos.y, 0))
                {
                    if (objectList[i].tag == "Ground")
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //checks if a tile is in the foreground or background
    private bool checkForeground(Vector2 tilePos)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i].transform.position == new Vector3(tilePos.x, tilePos.y, 0))
            {
                if (objectList[i].tag == "Obstacle" ||
                    objectList[i].tag == "Building" ||
                    objectList[i].tag == "Enemy" ||
                    objectList[i].tag == "NPC" ||
                    objectList[i].tag == "PlayerTile")
                {
                    return true;
                }
            }
        }
        return false;
    }

    //checks if a tile is in the foreground or background
    private bool checkForeground(GameObject prefabObject)
    {
        if (prefabObject.gameObject.tag == "Obstacle" ||
            prefabObject.gameObject.tag == "Building" ||
            prefabObject.gameObject.tag == "Enemy" ||
            prefabObject.gameObject.tag == "NPC" ||
            prefabObject.gameObject.tag == "PlayerTile")
        {
            return true;
        }
        return false;
    }

    //removes an object from the scene
    private void removeObjectFromScene()
    {
        if (removeObject == true && checkForTile(mousePos) == true)
        {
            if (checkForeground(mousePos) == true)
            {
                for (int i = 0; i < foreground.Count; i++)
                {
                    if (foreground[i] == mousePos)
                    {
                        foreground.RemoveAt(i);
                    }
                }
                for (int i = 0; i < objectList.Count; i++)
                {
                    if (objectList[i].transform.position == new Vector3(mousePos.x, mousePos.y, 0) &&
                        checkForeground(objectList[i]) == true)
                    {
                        DestroyImmediate(objectList[i]);
                        objectList.RemoveAt(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < background.Count; i++)
                {
                    if (background[i] == mousePos)
                    {
                        background.RemoveAt(i);
                    }
                }
                for (int i = 0; i < objectList.Count; i++)
                {
                    if (objectList[i].transform.position == new Vector3(mousePos.x, mousePos.y, 0) &&
                        checkForeground(objectList[i]) == false)
                    {
                        DestroyImmediate(objectList[i]);
                        objectList.RemoveAt(i);
                    }
                }
            }
            removeObject = false;
        }
    }

    //makes an object a child of another object
    private void makeChild(GameObject parentObject, GameObject childObject)
    {
        childObject.transform.parent = parentObject.transform;
    }

    //returns the level tiles array
    public LevelTile[] saveLevel()
    {
        if (instanceOf == null)
        {
            instanceOf = GameObject.FindObjectOfType<TileBucket>();
        }

        LevelTile[] levelArray = new LevelTile[objectList.Count];
        for (int i = 0; i < levelArray.Length; i++)
        {
            levelArray[i] = new LevelTile(objectList[i].gameObject.name, new Vector2(objectList[i].transform.position.x, objectList[i].gameObject.transform.position.y));
        }
        return levelArray;
    }

    //loads a level tile array
    public void loadLevel(LevelTile[] levelArray)
    {
        if (instanceOf == null)
        {
            instanceOf = GameObject.FindObjectOfType<TileBucket>();
        }

        resetEditor();
        linkTileObject();

        for (int i = 0; i < levelArray.Length; i++)
        {
            for (int ii = 0; ii < tiles.Length; ii++)
            {
                if (levelArray[i].tileName == prefabs[ii].gameObject.name)
                {
                    if (checkForeground(prefabs[ii]) == true)
                    {
                        foreground.Add(new Vector2(levelArray[i].position.x, levelArray[i].position.y));
                    }
                    else if (checkForeground(prefabs[ii]) == false)
                    {
                        background.Add(new Vector2(levelArray[i].position.x, levelArray[i].position.y));
                    }
                    prefabs[ii].transform.position = levelArray[i].position;
                    objectList.Add(Instantiate(prefabs[ii]));
                    objectList[objectList.Count - 1].name = prefabs[ii].name;
                    makeChild(tilesObject, objectList[objectList.Count - 1]);
                }
            }
        }
    }

    //resets the level to nothing
    public void resetEditor()
    {
        if (instanceOf == null)
        {
            instanceOf = GameObject.FindObjectOfType<TileBucket>();
        }

        for (int i = 0; i < objectList.Count; i++)
        {
            DestroyImmediate(objectList[i]);
        }
        background.Clear();
        foreground.Clear();
        objectList.Clear();
    }
}


//class that contains information on a level tile
public class LevelTile
{
    public string tileName { get; private set; }
    public Vector2 position { get; private set; }

    public LevelTile(string name, Vector2 pos)
    {
        tileName = name;
        position = pos;
    }
}