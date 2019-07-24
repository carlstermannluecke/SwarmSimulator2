using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// The GizmoManager is a class for debugging and flexible data visualization. 
/// In contrast to unitys gizmo system it is fully supported by built applications.
/// It can be used in a similar fashion to unitys build in gizmo system and 
/// tries to minimize performance overhead by making use of extensive caching.
/// </summary>
public class GizmoManager : MonoBehaviour
{
    private class GizmoType
    {
        public GameObject template;
        public List<GameObject> list;
        public int nextIndex;
    }
    
    [SerializeField] private Material solidGizmoMaterial;
    [SerializeField] private Material wireGizmoMaterial;
    [SerializeField] private GameObject templateSolidSphere;
    [SerializeField] private GameObject templateLine;
    [SerializeField] private GameObject templateTricircleSphere;
    [SerializeField] private GameObject templatePolySphere;
    [SerializeField] private GameObject templateWireCube;
    [SerializeField] private GameObject templateText;
    public bool activateGizmos = true;

    public static GizmoManager Instance { get; private set; }
    private Dictionary<string, bool> drawGroups = new Dictionary<string, bool>();
    private bool initialized = false;
    public delegate void NotificationHandler(GizmoManager gizmoManager); //Signature for DrawGizmos methods
    public NotificationHandler GizmoSubscribers; //Container storing all subscribed methods of above signature

    private Color currentColor;
    private Material currentSolidMaterial;
    private Material currentWireMaterial;

    GizmoType solidSpheres = new GizmoType();
    GizmoType lines = new GizmoType();
    GizmoType tricircleSpheres = new GizmoType();
    GizmoType polySpheres = new GizmoType();
    GizmoType wireCubes = new GizmoType();
    GizmoType texts = new GizmoType();
    List<GizmoType> gizmoTypes = new List<GizmoType>(6);

    private void Awake()
    {
        Instance = this;
        Initialize();
    }

    public void Initialize()
    {
        if (initialized) return;
        initialized = true;

        GizmoMaterialLibrary.Initialize(solidGizmoMaterial, wireGizmoMaterial);
        currentSolidMaterial = GizmoMaterialLibrary.GetMaterial(GizmoTopology.Solid, Color.white);
        currentWireMaterial = GizmoMaterialLibrary.GetMaterial(GizmoTopology.Wire, Color.white);

        gizmoTypes.Add(solidSpheres);
        gizmoTypes.Add(lines);
        gizmoTypes.Add(tricircleSpheres);
        gizmoTypes.Add(polySpheres);
        gizmoTypes.Add(wireCubes);
        gizmoTypes.Add(texts);

        solidSpheres.template = templateSolidSphere;
        lines.template = templateLine;
        tricircleSpheres.template = templateTricircleSphere;
        polySpheres.template = templatePolySphere;
        wireCubes.template = templateWireCube;
        texts.template = templateText;

        foreach (GizmoType type in gizmoTypes)
        {
            type.list = new List<GameObject>(2000);
            type.nextIndex = 0;
        }
    }

    private void Update ()
    {
        SetAllGizmosInactive();

        if (!activateGizmos) return;
        if (GizmoSubscribers != null) GizmoSubscribers.Invoke(this);
    }

    private void SetAllGizmosInactive()
    {
        for (int index = 0; index < gizmoTypes.Count; index++)
        {
            GizmoType type = gizmoTypes[index];
            int count = type.list.Count;

            for (int gizmoIndex = 0; gizmoIndex < count; gizmoIndex++)
                type.list[gizmoIndex].SetActive(false);

            type.nextIndex = 0;
        }
    }

    public void SetColor(Color color)
    {
        currentColor = color;
        currentSolidMaterial = GizmoMaterialLibrary.GetMaterial(GizmoTopology.Solid, currentColor);
        currentWireMaterial = GizmoMaterialLibrary.GetMaterial(GizmoTopology.Wire, currentColor);
    }

    public void DrawSolidSphere(Vector3 center, float scale, string drawGroup)
    {
        if (DrawGroupValue(drawGroup) == false) return;

        GameObject gizmo = NextGizmoOf(solidSpheres);
        gizmo.SetActive(true);
        gizmo.GetComponent<Renderer>().material = currentSolidMaterial;
        gizmo.transform.position = center;
        gizmo.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void DrawLine(Vector3 from, Vector3 to, float width, string drawGroup)
    {
        if (DrawGroupValue(drawGroup) == false) return;

        GameObject gizmo = NextGizmoOf(lines);
        gizmo.SetActive(true);
        LineRenderer lineRenderer = gizmo.GetComponent<LineRenderer>();
        lineRenderer.material = currentSolidMaterial;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    public void DrawTricircleSphere(Vector3 center, float scale, string drawGroup)
    {
        if (DrawGroupValue(drawGroup) == false) return;

        GameObject gizmo = NextGizmoOf(tricircleSpheres);
        gizmo.SetActive(true);
        gizmo.GetComponent<Renderer>().material = currentWireMaterial;
        gizmo.transform.position = center;
        gizmo.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void DrawPolySphere(Vector3 center, float scale, string drawGroup)
    {
        if (DrawGroupValue(drawGroup) == false) return;

        GameObject gizmo = NextGizmoOf(polySpheres);
        gizmo.SetActive(true);
        gizmo.GetComponent<Renderer>().material = currentWireMaterial;
        gizmo.transform.position = center;
        gizmo.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void DrawWireCube(Vector3 center, Vector3 scale, string drawGroup)
    {
        if (DrawGroupValue(drawGroup) == false) return;

        GameObject gizmo = NextGizmoOf(wireCubes);
        gizmo.SetActive(true);
        gizmo.GetComponent<Renderer>().material = currentWireMaterial;
        gizmo.transform.position = center;
        gizmo.transform.localScale = scale;
    }

    public void DrawText(string text, Vector3 upperLeftCorner, Vector3 scale, string drawGroup)
    {
        if (DrawGroupValue(drawGroup) == false) return;

        GameObject gizmo = NextGizmoOf(texts);
        gizmo.SetActive(true);
        gizmo.GetComponent<Text>().text = text;
        gizmo.transform.position = upperLeftCorner;//Does scale break things when the text is fixed at the upper left corner?
        gizmo.transform.localScale = scale;
    }

    private GameObject NextGizmoOf(GizmoType type)
    {
        GameObject gizmo;
        if (type.nextIndex > type.list.Count - 1)
        {
            gizmo = Instantiate(type.template);
            type.list.Add(gizmo);
        }
        else gizmo = type.list[type.nextIndex];
        type.nextIndex++;

        return gizmo;
    }

    public void EnableDrawGroup(string groupID, bool enable)
    {
        DrawGroupValue(groupID); //Just to ensure it exists 
        drawGroups[groupID] = enable;
        return;
    }

    public bool DrawGroupValue(string groupID)
    {
        if (!drawGroups.ContainsKey(groupID)) drawGroups.Add(groupID, true);
        return drawGroups[groupID];
    }
}