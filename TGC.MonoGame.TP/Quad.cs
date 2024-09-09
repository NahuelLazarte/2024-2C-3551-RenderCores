using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP
{
    public class Quad : IDisposable
    {
        private VertexBuffer _vertexBuffer; //clase de MonoGame
        private IndexBuffer _indexBuffer; //clase de MonoGame

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            //Crear el Quad
            var vertices = new VertexPositionColor[4]
            {
            new VertexPositionColor(new Vector3(-1.0f,-1.0f,0f), Color.Red),
            new VertexPositionColor(new Vector3(-1.0f,1.0f,0f), Color.Green),
            new VertexPositionColor(new Vector3(1.0f,-1.0f,0f), Color.Yellow),
            new VertexPositionColor(new Vector3(1.0f,1.0f,0f), Color.Blue)
            };

            // int = 32 bits
            // short = 16 bits
            // short sin signo = ushort
            var indices = new ushort[6]
            {
            //Primer trinagulo
            0,1,3,
            //Segundo triangulo
            0,3,2, // así está bien el orden de indexado
            };

            _vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColor.VertexDeclaration, 4, BufferUsage.None);//un quad tiene 6 vertices si los triangulos son individuales, pero nosotros queremos juntar esos triangulos. Es mejor hacerlo así para trabajar con los dos juntos y es menos costoso para la placa de video
            _vertexBuffer.SetData(vertices);// un vertice tiene una posicion, color, la normal y coordenada de textura
            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.None); //el index buffer apunta a los vertices y tiene que decir cómo hacer los triangulos
            _indexBuffer.SetData(indices);
        }
        public void Draw(Effect effect)
        {
            //sacado se samples, GeometricPrimiteve, del método Draw
            var graphicsDevice = effect.GraphicsDevice;
            //Set our vertex declaration, vertex buffer, and index buffer.
            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;

            foreach (var effectPass in effect.CurrentTechnique.Passes)
            { // después vamos a ver qué es una pasada
                effectPass.Apply();
                int primitiveCount = 2;
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount); // hay más tipos de primitivas en el min  50:00 lo explica
            }
        }
        public void Dispose()
        {   // implementación de la interfaz que la clase hereda
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
}