using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    // Couleur bleu ciel
    public Color color = new Color(0.53f, 0.81f, 0.92f); // Valeurs RGB pour bleu ciel

    void Start()
    {
        // Créer un nouveau matériau avec le shader URP/Lit ou Standard
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        // Si vous n'utilisez pas URP, vous pouvez utiliser Shader.Find("Standard")

        // Définir la couleur principale du matériau
        material.SetColor("_BaseColor", color); // Pour URP/Lit Shader
        // material.color = color; // Pour Standard Shader

        // Appliquer le matériau à l'objet
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
            Debug.Log($"✅ Couleur bleu ciel appliquée à {gameObject.name}");
        }
        else
        {
            Debug.LogError($"❌ Aucun Renderer trouvé sur {gameObject.name}");
        }
    }
}
