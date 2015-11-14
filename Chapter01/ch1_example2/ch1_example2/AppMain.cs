using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace ch1_example2
{
	public class AppMain
	{
		private static GraphicsContext _graphics;
		private static Texture2D _texture;
		private static VertexBuffer _vertexBuffer;
		private static ShaderProgram _textureShaderProgram;
		private static Matrix4 _localMatrix;
		private static Matrix4 _projectionMatrix;
		private static Matrix4 _viewMatrix;
		private static float _viewportWidth;
		private static float _viewportHeight;
		
		public static void Main (string[] args)
		{
			Initialize ();

			while (true) {
				SystemEvents.CheckEvents ();
				Update ();
				Render ();
			}
		}

		public static void Initialize ()
		{
			// Set up the graphics system
			_graphics = new GraphicsContext ();
			
			_viewportWidth = _graphics.GetFrameBuffer().Width;
			_viewportHeight= _graphics.GetFrameBuffer().Height;
			
			_texture = new Texture2D("/images.jpg", false);
			
			_vertexBuffer = new VertexBuffer(4, VertexFormat.Float3, VertexFormat.Float2);
			_vertexBuffer.SetVertices(0, new float[]{
				1,0,0, 
				_texture.Width,0,0, 
				_texture.Width, 
				_texture.Height,0,0,
				_texture.Height,0});
			
			_vertexBuffer.SetVertices(1, new float[]{
				0.0f, 0.0f,
				1.0f, 0.0f,
				1.0f, 1.0f,
				0.0f, 1.0f});
			
			_textureShaderProgram = new ShaderProgram("/Application/shaders/Texture.cgx");
			
			_projectionMatrix = Matrix4.Ortho (0,_viewportWidth,0,_viewportHeight,0.0f,32768.0f);
			
			_viewMatrix = Matrix4.LookAt(
				new Vector3(0,_viewportHeight,0), 
				new Vector3(0,_viewportHeight,1),
				new Vector3(0,-1,0));
			
			_localMatrix = Matrix4.Translation(new Vector3(-_texture.Width/2,-_texture.Height/2,0.0f))
				* Matrix4.Translation(new Vector3(_viewportWidth/2, _viewportHeight/2,0.0f));
		}

		public static void Update ()
		{
			// Query gamepad for current state
			var gamePadData = GamePad.GetData (0);
		}

		public static void Render ()
		{
			// Clear the screen
			_graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			_graphics.Clear ();
			
			var worldViewProjection = _projectionMatrix * _viewMatrix * _localMatrix;
			
			_textureShaderProgram.SetUniformValue(0, ref worldViewProjection);
			
			_graphics.SetShaderProgram(_textureShaderProgram);
			_graphics.SetVertexBuffer(0,_vertexBuffer);
			_graphics.SetTexture(0,_texture);
			
			_graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
			
			// Present the screen
			_graphics.SwapBuffers ();
		}
	}
}
