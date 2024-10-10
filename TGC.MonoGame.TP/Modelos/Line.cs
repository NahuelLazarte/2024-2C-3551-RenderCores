using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace TGC.MonoGame.TP.Modelos
{
public class LineDrawer
{
    private GraphicsDevice graphicsDevice;
    private BasicEffect basicEffect;

    public LineDrawer(GraphicsDevice graphicsDevice)
    {
        this.graphicsDevice = graphicsDevice;
        basicEffect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            View = Matrix.Identity,
            Projection = Matrix.Identity,
            World = Matrix.Identity
        };
    }

    public void DrawLine(Vector3 start, Vector3 end, Color color, Matrix view, Matrix projection)
    {
        VertexPositionColor[] vertices = new VertexPositionColor[2];
        vertices[0] = new VertexPositionColor(start, color);
        vertices[1] = new VertexPositionColor(end, color);

        basicEffect.View = view;
        basicEffect.Projection = projection;

        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
}