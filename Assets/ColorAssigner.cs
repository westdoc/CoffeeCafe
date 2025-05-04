using UnityEngine;

public class ColorAssigner : MonoBehaviour
{
    MeshRenderer meshRenderer;

    public ColorData.ColorOption selectedColor = ColorData.ColorOption.Empty;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Update()
    {
        if (meshRenderer != null && meshRenderer.material.color != ColorData.GetColor(selectedColor))
        {
            if (selectedColor != ColorData.ColorOption.Empty)
            {
                GetComponent<MeshRenderer>().enabled = true;

                // Create a new material instance to avoid modifying the shared material
                Material newMaterial = new Material(meshRenderer.material);

                newMaterial.color = ColorData.GetColor(selectedColor);

                // Apply the new material to the mesh renderer
                meshRenderer.material = newMaterial;
            }
            else
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public void setColor(ColorData.ColorOption color)
    {
        selectedColor = color;

        if (selectedColor != ColorData.ColorOption.Empty)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
