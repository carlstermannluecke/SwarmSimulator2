using UnityEngine;
using System.Collections;

public enum GizmoTopology
{
    Solid,
    Wire
}

/// <summary>
/// The GizmoMaterialLibrary stores all ever used gizmo materials (differing only by color) to
/// avoid expensive instantiations and enable efficient material swapping.
/// </summary>
public static class GizmoMaterialLibrary
{
    private static Material solidMaterialTemplate;
    private static Material wireMaterialTemplate;

    //Standard materials for quick access
    private static Hashtable solidMaterials = new Hashtable();
    private static Hashtable wireMaterials = new Hashtable();

    public static void Initialize(Material solidGizmoMaterial, Material wireGizmoMaterial)
    {
        solidMaterialTemplate = solidGizmoMaterial;
        wireMaterialTemplate = wireGizmoMaterial;

        //Define standard materials
        DefineMaterials(Color.white);
        DefineMaterials(Color.gray);
        DefineMaterials(Color.black);
        DefineMaterials(Color.red);
        DefineMaterials(Color.green);
        DefineMaterials(Color.blue);
        DefineMaterials(Color.yellow);
        DefineMaterials(Color.cyan);
        DefineMaterials(Color.magenta);
    }
    
    private static void DefineMaterials(Color color)
    {
        if (solidMaterials.ContainsKey(color) || wireMaterials.ContainsKey(color)) return;

        Material solidMaterial = new Material(solidMaterialTemplate);
        Material wireMaterial = new Material(wireMaterialTemplate);
        solidMaterial.color = color;
        wireMaterial.color = color;
        solidMaterials.Add(color, solidMaterial);
        wireMaterials.Add(color, wireMaterial);
    }

    public static Material GetMaterial(GizmoTopology gizmoTopology, Color gizmoColor)
    {
        Hashtable relevantTable = null;
        if (gizmoTopology == GizmoTopology.Solid) relevantTable = solidMaterials;
        if (gizmoTopology == GizmoTopology.Wire) relevantTable = wireMaterials;

        if (relevantTable.ContainsKey(gizmoColor) == false) DefineMaterials(gizmoColor);
        
        return (Material)relevantTable[gizmoColor];
    }
}
