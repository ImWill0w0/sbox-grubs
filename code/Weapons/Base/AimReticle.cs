﻿using Sandbox;
using System;

namespace Grubs.Weapons
{
	public partial class AimReticle : RenderEntity
	{
		public Material Material = Material.Load( "materials/reticle/reticle.vmat" );

		public Vector3 Direction = Vector3.Zero;
		public float Power = 0.0f;

		protected void DrawReticle( SceneObject obj, Vector3 startPos, Vector3 endPos, Vector3 direction, Vector3 size, Color color )
		{
			// vbos are drawn relative to world position
			startPos -= Position;
			endPos -= Position;

			var vertexBuffer = Render.GetDynamicVB( true );

			// Line
			Vertex a = new( startPos - size, Vector3.Up, Vector3.Right, new Vector4( 0, 1, 0, 0 ) );
			Vertex b = new( startPos + size, Vector3.Up, Vector3.Right, new Vector4( 1, 1, 0, 0 ) );
			Vertex c = new( endPos + size, Vector3.Up, Vector3.Right, new Vector4( 1, 0, 0, 0 ) );
			Vertex d = new( endPos - size, Vector3.Up, Vector3.Right, new Vector4( 0, 0, 0, 0 ) );

			vertexBuffer.Add( a );
			vertexBuffer.Add( b );
			vertexBuffer.Add( c );
			vertexBuffer.Add( d );

			vertexBuffer.AddTriangleIndex( 4, 3, 2 );
			vertexBuffer.AddTriangleIndex( 2, 1, 4 );

			Render.Set( "color", color );

			vertexBuffer.Draw( Material );
		}

		public override void DoRender( SceneObject obj )
		{
			Render.SetLighting( obj );

			var startPos = Position.WithY( -35 );
			var endPos = (Position + (Direction * 30)).WithY( -35 );
			var size = Vector3.Cross( Direction, Vector3.Right ) * 15f;

			var color = Color.White;
			DrawReticle( obj, startPos, endPos, Direction, size, color );
		}
	}
}
