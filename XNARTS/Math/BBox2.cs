using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using XNARTS;
using XNARTS.Math;


namespace XNARTS.RTSMath
{
	public struct AABB2
	{
		private Vector2 mMin;
		private Vector2 mMax;
		private bool    mIsValid;


		public AABB2( Vector2 min, Vector2 max )
		{
			Validate( min, max );
			mMin = min;
			mMax = max;
			mIsValid = true;
		}


		public AABB2( Vector2 min_max )
		{
			mMin = min_max;
			mMax = min_max;
			mIsValid = true;
		}


		public AABB2( Vector2 center, float radius )
		{
			Utils.Assert( radius >= 0f );
			Vector2 half_span = new Vector2( radius, radius );
			mMin = center - half_span;
			mMax = center + half_span;
			mIsValid = true;
		}


		public void Reset()
		{
			mIsValid = false;
		}


		public bool IsValid()
		{
			return mIsValid;
		}


		public void Set( Vector2 min, Vector2 max )
		{
			Validate( min, max );
			mMin = min;
			mMax = max;
			mIsValid = true;
		}


		public void Set( Vector2 min_max )
		{
			mMin = min_max;
			mMax = min_max;
			mIsValid = true;
		}


		public void Set( Vector2 center, float radius )
		{
			Utils.Assert( radius >= 0f );
			Vector2 half_span = new Vector2( radius, radius );
			mMin = center - half_span;
			mMax = center + half_span;
			mIsValid = true;
		}


		public void Add( Vector2 point )
		{
			if( mIsValid )
			{
				mMin.X = MathHelper.Min( mMin.X, point.X );
				mMin.Y = MathHelper.Min( mMin.Y, point.Y );
				mMax.X = MathHelper.Max( mMax.X, point.X );
				mMax.Y = MathHelper.Max( mMax.Y, point.Y );
			}
			else
			{
				Set( point );
			}
		}


		public Vector2 GetMin()
		{
			Utils.Assert( mIsValid );
			return mMin;
		}


		public Vector2 GetMax()
		{
			Utils.Assert( mIsValid );
			return mMax;
		}


		public Vector2 GetSize()
		{
			Utils.Assert( mIsValid );
			return mMax - mMin;
		}


		public Vector2 GetRadius()
		{
			Utils.Assert( mIsValid );
			return 0.5f * GetSize();
		}


		public Vector2 GetCenter()
		{
			Utils.Assert( mIsValid );
			return 0.5f * (mMin + mMax);
		}


		public bool IsNonDegenerate()
		{
			Utils.Assert( mIsValid );
			Vector2 span = GetSize();
			return span.X > 0f && span.Y > 0f;
		}


		public float GetArea()
		{
			Utils.Assert( mIsValid );
			return (mMax.X - mMin.X) * (mMax.Y - mMin.Y);
		}


		public bool Contains( Vector2 point )
		{
			Utils.Assert( mIsValid );
			return point.X >= mMin.X && point.X <= mMax.X && point.Y >= mMin.Y && point.Y <= mMax.Y;
		}


		public void	ScaleWorld( float f )
		{
			Utils.Assert( mIsValid && f >= 0f );
			mMin *= f; mMax *= f;
		}


		public void	ScaleLocal( float f )
		{
			Utils.Assert( mIsValid && f >= 0f );
			Vector2 new_radius = f * GetRadius();
			Vector2 center = GetCenter();
			Set( center - new_radius, center + new_radius );
		}


		public void Translate( Vector2 translation )
		{
			Utils.Assert( mIsValid );
			mMin += translation;
			mMax += translation;
		}


		public void Resize( Vector2 change, bool assert_on_invert = false )
		{
			Utils.Assert( mIsValid );
			mMin -= change;
			mMax += change;
			bool inverted = false;

			if( mMin.X > mMax.X )
			{
				inverted = true;
				float avg = 0.5f * (mMin.X + mMax.X);
				mMin.X = avg;
				mMax.X = avg;
			}

			if( mMin.Y > mMax.Y )
			{
				inverted = true;
				float avg = 0.5f * (mMin.Y + mMax.Y);
				mMin.Y = avg;
				mMax.Y = avg;
			}

			Validate( mMin, mMax );

			if( assert_on_invert && inverted )
			{
				Utils.Assert( false );
			}
		}


		private static void Validate( Vector2 min, Vector2 max )
		{
			Utils.Assert( max.X >= min.X && max.Y >= min.Y );
		}


		public static void unitTest()
		{
			AABB2 aabb1 = new AABB2();
			Utils.Assert( aabb1.mIsValid == false );

			AABB2 aabb2 = new AABB2( Vector2.Zero );
			AABB2 aabb3 = new AABB2( Vector2.Zero, 1f );
			AABB2 aabb4 = new AABB2( Vector2.Zero, Vector2.Zero );
			Utils.Assert( aabb2.IsValid() && aabb3.IsValid() && aabb4.IsValid() );

			aabb2.Reset();
			Utils.Assert( !aabb2.IsValid() );

			Vector2 oneTwo = new Vector2( 1f, 2f );
			Vector2 negTwoFour = -2f * oneTwo;
			Vector2 TwentyTen = new Vector2( 20f, 10f );
			const float kTol = 0.001f;

			aabb2.Set( oneTwo );
			Utils.AssertVal( aabb2.mMin, oneTwo, kTol );
			Utils.AssertVal( aabb2.mMax, oneTwo, kTol );
			Utils.Assert( aabb2.IsValid() );

			//aabb2.Set( oneTwo, negTwoFour ); inside out, asserts, good
			aabb2.Set( negTwoFour, oneTwo );
			Utils.Assert( aabb2.Contains( Vector2.Zero ) );
			Utils.Assert( !aabb2.Contains( TwentyTen ) );
			Utils.Assert( aabb2.Contains( negTwoFour ) );
			Utils.Assert( aabb2.Contains( oneTwo ) );
			Utils.Assert( !aabb2.Contains( -TwentyTen ) );

			//aabb2.Set( oneTwo, -3f ); asserts on negative radius, good
			Vector2 fiveFive = new Vector2( 5f, 5f );
			Vector2 sixSeven = oneTwo + fiveFive;
			Vector2 negFourThree = new Vector2( -4f, -3f );
			Vector2 epsilon = new Vector2( kTol, kTol );

			aabb2.Set( oneTwo, 5f );
			Utils.Assert( aabb2.Contains( oneTwo ) );
			Utils.Assert( aabb2.Contains( fiveFive ) );
			Utils.Assert( aabb2.Contains( negFourThree ) );
			Utils.Assert( !aabb2.Contains( TwentyTen ) );
			Utils.Assert( !aabb2.Contains( -TwentyTen ) );
			Utils.Assert( aabb2.Contains( Vector2.Zero ) );
			Utils.Assert( aabb2.Contains( negFourThree + epsilon ) );
			Utils.Assert( !aabb2.Contains( negFourThree - epsilon ) );
			Utils.Assert( aabb2.Contains( sixSeven - epsilon ) );
			Utils.Assert( !aabb2.Contains( sixSeven + epsilon ) );

			aabb2.Set( Vector2.Zero );
			Utils.Assert( !aabb2.IsNonDegenerate() );

			aabb2.Add( Vector2.UnitX );
			Utils.Assert( !aabb2.IsNonDegenerate() );
			Utils.Assert( aabb2.Contains( new Vector2( 0.5f, 0f ) ) );

			aabb2.Add( oneTwo );
			Utils.Assert( aabb2.IsNonDegenerate() );
			Utils.Assert( aabb2.Contains( new Vector2( 0.5f, 1f ) ) );

			aabb2.Reset();
			aabb2.Add( Vector2.Zero );
			Utils.Assert( aabb2.IsValid() );

			aabb2.Reset();
			aabb2.Add( -fiveFive );
			aabb2.Add( TwentyTen );
			Utils.AssertVal( aabb2.GetMin(), -fiveFive, kTol );
			Utils.AssertVal( aabb2.GetMax(), TwentyTen, kTol );
			Utils.AssertVal( aabb2.GetCenter(), new Vector2( 7.5f, 2.5f ), kTol );
			Utils.AssertVal( aabb2.GetRadius(), new Vector2( 12.5f, 7.5f ), kTol );
			Utils.AssertVal( aabb2.GetSize(), new Vector2( 25f, 15f ), kTol );
			Utils.AssertVal( aabb2.GetArea(), 375f, kTol );

			aabb2.Reset();
			aabb2.Add( Vector2.Zero );
			aabb2.Add( oneTwo );
			aabb2.ScaleWorld( 4f );
			Utils.AssertVal( aabb2.GetArea(), 32f, kTol );

			aabb2.Translate( fiveFive );
			Vector2 center2 = new Vector2( 7f, 9f );
			Utils.AssertVal( aabb2.GetArea(), 32f, kTol );
			Utils.AssertVal( aabb2.GetCenter(), center2, kTol );
			Utils.Assert( !aabb2.Contains( oneTwo ) );
			Utils.Assert( aabb2.Contains( new Vector2( 6f, 8f ) ) );

			//aabb2.ScaleWorld( -1f ); asserts negative scalar, good
			//aabb2.ScaleLocal( -50f ); asserts negative scalar, good
			aabb2.ScaleLocal( 0.25f );
			Utils.AssertVal( aabb2.GetCenter(), center2, kTol );
			Utils.AssertVal( aabb2.GetArea(), 2f, kTol );
			Utils.Assert( !aabb2.Contains( new Vector2( 6f, 8f ) ) );
			Utils.Assert( aabb2.Contains( center2 ) );

			aabb2.Reset();
			aabb2.Add( Vector2.Zero );
			aabb2.Add( oneTwo );
			aabb2.Resize( new Vector2( 0.1f, -0.3f ) );
			Utils.AssertVal( aabb2.GetArea(), 1.68f, kTol );
			Utils.Assert( !aabb2.Contains( Vector2.Zero ) );
			Utils.Assert( aabb2.Contains( new Vector2( -0.05f, 1.4f ) ) );
		}
	}
}
