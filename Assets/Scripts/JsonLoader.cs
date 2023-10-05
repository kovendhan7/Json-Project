using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MiniJSON;
using UnityEngine.UI;

public class JSONLoaderWindow : EditorWindow
{
    private string jsonText = "";
    public InputField jsonInputField;
    public Canvas parentPrefab;
    public GameObject textChildPrefab;
    public GameObject imageChildPrefab;
    List<GameObject> allObjectCollection = new List<GameObject>();

    [MenuItem("Window/JSON Loader Window")]
    public static void ShowWindow()
    {
        GetWindow<JSONLoaderWindow>("JSON Loader");
    }

    private void Awake()
    {
        parentPrefab = GameObject.FindGameObjectWithTag("canvas").GetComponent<Canvas>();
        textChildPrefab = Resources.Load<GameObject>("Text");
        imageChildPrefab = Resources.Load<GameObject>("Image");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("JSON Loader Window", EditorStyles.boldLabel);

        jsonText = EditorGUILayout.TextArea(jsonText, GUILayout.Height(200));
        parentPrefab = EditorGUILayout.ObjectField("parent prefab", parentPrefab, typeof(Canvas), true) as Canvas;
        textChildPrefab = EditorGUILayout.ObjectField("text child prefab", textChildPrefab, typeof(GameObject), true) as GameObject;
        imageChildPrefab = EditorGUILayout.ObjectField("image child prefab", imageChildPrefab, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Load"))
        {
            Serialization();
        }

        if (GUILayout.Button("Delete"))
        {
            DestroyAllObjects();
        }

        if (GUILayout.Button("Edit"))
        {
            DestroyAllObjects();
            Serialization();
        }
    }

    private void DestroyAllObjects()
    {
        foreach (var item in allObjectCollection)
        {
            DestroyImmediate(item);
        }
    }

    private void Serialization()
    {
        if (!string.IsNullOrEmpty(jsonText))
        {
            Debug.Log("Loaded JSON: " + jsonText);

            Dictionary<string, object> dict = Json.Deserialize(jsonText) as Dictionary<string, object>;
            var obj2 = dict["Objects"] as List<object>;

            var objectCollection = new List<GameObject>();
            for (int i = 0; i < obj2.Count; i++)
            {
                Dictionary<string, object> dict2 = Json.Deserialize(obj2[i].ToString()) as Dictionary<string, object>;
                var name = dict2["Name"].ToString();
                var type = dict2["Type"].ToString();
                var parent = dict2["Parent"].ToString();
                var tempPosition = dict2["Position"].ToString();
                var pos = tempPosition.Split('~');
                var posX = int.Parse(pos[0]);
                var posy = int.Parse(pos[1]);

                if (type.Equals("Image"))
                {
                    var tempColor = dict2["Color"].ToString();
                    var color = tempColor.Split('~');
                    var r = float.Parse(color[0]);
                    var g = float.Parse(color[1]);
                    var b = float.Parse(color[2]);
                    var a = float.Parse(color[3]);
                    var gameObject = Instantiate(imageChildPrefab, parentPrefab.transform);
                    objectCollection.Add(gameObject);
                    gameObject.GetComponent<Image>().color = new Color(r, g, b, a);
                    //sprite color change, sprite change etc can be done here 
                }
                else
                {
                    var text = dict2["Text"].ToString();
                    var gameObject = Instantiate(textChildPrefab, parentPrefab.transform);
                    objectCollection.Add(gameObject);
                    gameObject.GetComponent<Text>().text = text;
                    //text change, font adjustments can be done here
                }

                objectCollection[objectCollection.Count - 1].transform.name = name;
                objectCollection[objectCollection.Count - 1].transform.localPosition = new Vector2(posX, posy);

                foreach (var item in objectCollection)
                {
                    if (parent.Equals(item.gameObject.name))
                    {
                        objectCollection[objectCollection.Count - 1].transform.SetParent(item.transform);
                    }
                    allObjectCollection.Add(item);
                }
            }
        }
        else
        {
            Debug.Log("json text is empty");
        }
    }
}

