using UnityEngine;

//adapted form here: https://answers.unity.com/questions/1217647/draw-axis-script-in-c.html
public class ShowTransformAxis : MonoBehaviour
{
    private Material lineMaterial;
    private void CreateLineMaterial()
    {
        // Unity has a built-in shader that is useful for drawing
        // simple colored things.
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        // Turn on alpha blending
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn backface culling off
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        // Turn off depth writes
        lineMaterial.SetInt("_ZWrite", 0);
    }

    // Will be called after all regular rendering is done
    private void OnRenderObject()
    {
        if(!lineMaterial)
            CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        //Draw X axis
        Color xColor = Color.red;
        GL.Color(xColor);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(3f, 0.0f, 0.0f);
        GL.Color(new Color(xColor.r, xColor.g, xColor.b, 0.3f));
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(-2f, 0.0f, 0.0f);
        //Draw Y axis
        Color yColor = Color.green;
        GL.Color(yColor);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0.0f, 3f, 0.0f);
        GL.Color(new Color(yColor.r, yColor.g, yColor.b, 0.3f));
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0.0f, -2f, 0.0f);
        //Draw Z axis
        Color zColor = Color.blue;
        GL.Color(zColor);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0.0f, 0.0f, 3f);
        GL.Color(new Color(zColor.r, zColor.g, zColor.b, 0.3f));
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0.0f, 0.0f, -2f);
        GL.End();
        GL.PopMatrix();
    }
}
