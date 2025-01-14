﻿using System;
using Sandbox;
using Grubs.Pawn;
using Grubs.States.SubStates;

namespace Grubs.Weapons
{
	public abstract partial class ArcWeapon : Weapon
	{
		// Weapon settings
		public override string WeaponName => "";
		public override string ModelPath => "";
		public virtual string ProjectileModel => "models/weapons/shell/shell.vmdl";
		public override int MaxQuantityFired { get; set; } = 1;
		public override HoldPose HoldPose => HoldPose.Bazooka;

		// Weapon properties
		public Entity Projectile { get; set; }
		private float ComputedForce { get; set; } = 0;
		public static PowerArrow PowerArrow { get; set; }
		public static AimReticle AimReticle { get; set; }

		public override void Simulate( Client player )
		{
			if ( Input.Down( InputButton.Attack1 ) && WeaponEnabled && TimeSinceFired > SecondsBetweenFired )
			{
				ComputedForce = (float)Math.Clamp( ComputedForce + 0.4, 0, 30 );
			}

			if ( Input.Released( InputButton.Attack1 ) )
			{
				QuantityFired++;
				OnFire();

				// Reset force for the next time we fire.
				ComputedForce = 0;
			}

			if ( IsClient )
			{
				AdjustReticle();
				AdjustArrow();
			}
		}

		private void AdjustReticle()
		{
			if ( !AimReticle.IsValid() )
				AimReticle = new();

			AimReticle.Position = Position + Parent.EyeRot.Forward.Normal * 80;
			AimReticle.Direction = Parent.EyeRot.Forward.Normal;
		}

		private void AdjustArrow()
		{
			if ( !PowerArrow.IsValid() )
				PowerArrow = new();

			PowerArrow.Position = Parent.EyePos;
			PowerArrow.Direction = Parent.EyeRot.Forward.Normal;
			PowerArrow.Power = (float)Math.Clamp( ComputedForce * 5, 0, 120 );
		}

		protected override void Fire()
		{
			var trace = new ArcTrace( Parent, Parent.EyePos ).RunTowards( Parent.EyeRot.Forward.Normal, ComputedForce, Turn.Instance?.WindForce ?? 0 );

			new Projectile().MoveAlongTrace( trace ).WithModel( ProjectileModel );
		}

		public override void ActiveEnd( Entity ent, bool dropped )
		{
			ActiveEndClient( To.Single( Parent ) );

			base.ActiveEnd( ent, dropped );
		}

		[ClientRpc]
		public void ActiveEndClient()
		{
			PowerArrow.Delete();
			AimReticle.Delete();
		}
	}
}
