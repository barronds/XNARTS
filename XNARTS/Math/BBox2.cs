using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using XNARTS;


namespace XNARTS
{
	public struct xAABB2
	{
		private Vector2 mMin;
		private Vector2 mMax;
		private bool    mIsValid;

		private static xAABB2 sOrigin = new xAABB2( Vector2.Zero );

		public xAABB2( Vector2 min, Vector2 max )
		{
			Validate( min, max );
			mMin = min;
			mMax = max;
			mIsValid = true;
		}


		public xAABB2( Vector2 min_max )
		{
			mMin = min_max;
			mMax = min_max;
			mIsValid = true;
		}


		public xAABB2( Vector2 center, float radius )
		{
			XUtils.Assert( radius >= 0f );
			Vector2 half_span = new Vector2( radius, radius );
			mMin = center - half_span;
			mMax = center + half_span;
			mIsValid = true;
		}

		public static xAABB2 GetOrigin()
		{
			return sOrigin;
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
			XUtils.Assert( radius >= 0f );
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
			XUtils.Assert( mIsValid );
			return mMin;
		}


		public Vector2 GetMax()
		{
			XUtils.Assert( mIsValid );
			return mMax;
		}


		public Vector2 GetSize()
		{
			XUtils.Assert( mIsValid );
			return mMax - mMin;
		}


		public Vector2 GetRadius()
		{
			XUtils.Assert( mIsValid );
			return 0.5f * GetSize();
		}


		public Vector2 GetCenter()
		{
			XUtils.Assert( mIsValid );
			return 0.5f * (mMin + mMax);
		}


		public bool IsNonDegenerate()
		{
			XUtils.Assert( mIsValid );
			Vector2 span = GetSize();
			return span.X > 0f && span.Y > 0f;
		}


		public float GetArea()
		{
			XUtils.Assert( mIsValid );
			return (mMax.X - mMin.X) * (mMax.Y - mMin.Y);
		}


		public bool Contains( Vector2 point )
		{
			XUtils.Assert( mIsValid );
			return point.X >= mMin.X && point.X <= mMax.X && point.Y >= mMin.Y && point.Y <= mMax.Y;
		}


		public void	ScaleWorld( float f )
		{
			XUtils.Assert( mIsValid && f >= 0f );
			mMin *= f; mMax *= f;
		}


		public void	ScaleLocal( float f )
		{
			XUtils.Assert( mIsValid && f >= 0f );
			Vector2 new_radius = f * GetRadius();
			Vector2 center = GetCenter();
			Set( center - new_radius, center + new_radius );
		}


		public void ScaleLocal( double d )
		{
			ScaleLocal( (float)d );
		}


		public void Translate( Vector2 translation )
		{
			XUtils.Assert( mIsValid );
			mMin += translation;
			mMax += translation;
		}


		public void Resize( Vector2 change, bool assert_on_invert = false )
		{
			XUtils.Assert( mIsValid );
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
				XUtils.Assert( false );
			}
		}


		public override string ToString()
		{
			return mMin.ToString() + " -> " + mMax.ToString();
		}


		private static void Validate( Vector2 min, Vector2 max )
		{
			XUtils.Assert( max.X >= min.X && max.Y >= min.Y );
		}


		public static void UnitTest()
		{
			xAABB2 aabb1 = new xAABB2();
			XUtils.Assert( aabb1.mIsValid == false );

			xAABB2 aabb2 = new xAABB2( Vector2.Zero );
			xAABB2 aabb3 = new xAABB2( Vector2.Zero, 1f );
			xAABB2 aabb4 = new xAABB2( Vector2.Zero, Vector2.Zero );
			XUtils.Assert( aabb2.IsValid() && aabb3.IsValid() && aabb4.IsValid() );

			aabb2.Reset();
			XUtils.Assert( !aabb2.IsValid() );

			Vector2 oneTwo = new Vector2( 1f, 2f );
			Vector2 negTwoFour = -2f * oneTwo;
			Vector2 TwentyTen = new Vector2( 20f, 10f );
			const float kTol = 0.001f;

			aabb2.Set( oneTwo );
			XUtils.AssertVal( aabb2.mMin, oneTwo, kTol );
			XUtils.AssertVal( aabb2.mMax, oneTwo, kTol );
			XUtils.Assert( aabb2.IsValid() );

			//aabb2.Set( oneTwo, negTwoFour ); inside out, asserts, good
			aabb2.Set( negTwoFour, oneTwo );
			XUtils.Assert( aabb2.Contains( Vector2.Zero ) );
			XUtils.Assert( !aabb2.Contains( TwentyTen ) );
			XUtils.Assert( aabb2.Contains( negTwoFour ) );
			XUtils.Assert( aabb2.Contains( oneTwo ) );
			XUtils.Assert( !aabb2.Contains( -TwentyTen ) );

			//aabb2.Set( oneTwo, -3f ); asserts on negative radius, good
			Vector2 fiveFive = new Vector2( 5f, 5f );
			Vector2 sixSeven = oneTwo + fiveFive;
			Vector2 negFourThree = new Vector2( -4f, -3f );
			Vector2 epsilon = new Vector2( kTol, kTol );

			aabb2.Set( oneTwo, 5f );
			XUtils.Assert( aabb2.Contains( oneTwo ) );
			XUtils.Assert( aabb2.Contains( fiveFive ) );
			XUtils.Assert( aabb2.Contains( negFourThree ) );
			XUtils.Assert( !aabb2.Contains( TwentyTen ) );
			XUtils.Assert( !aabb2.Contains( -TwentyTen ) );
			XUtils.Assert( aabb2.Contains( Vector2.Zero ) );
			XUtils.Assert( aabb2.Contains( negFourThree + epsilon ) );
			XUtils.Assert( !aabb2.Contains( negFourThree - epsilon ) );
			XUtils.Assert( aabb2.Contains( sixSeven - epsilon ) );
			XUtils.Assert( !aabb2.Contains( sixSeven + epsilon ) );

			aabb2.Set( Vector2.Zero );
			XUtils.Assert( !aabb2.IsNonDegenerate() );

			aabb2.Add( Vector2.UnitX );
			XUtils.Assert( !aabb2.IsNonDegenerate() );
			XUtils.Assert( aabb2.Contains( new Vector2( 0.5f, 0f ) ) );

			aabb2.Add( oneTwo );
			XUtils.Assert( aabb2.IsNonDegenerate() );
			XUtils.Assert( aabb2.Contains( new Vector2( 0.5f, 1f ) ) );

			aabb2.Reset();
			aabb2.Add( Vector2.Zero );
			XUtils.Assert( aabb2.IsValid() );

			aabb2.Reset();
			aabb2.Add( -fiveFive );
			aabb2.Add( TwentyTen );
			XUtils.AssertVal( aabb2.GetMin(), -fiveFive, kTol );
			XUtils.AssertVal( aabb2.GetMax(), TwentyTen, kTol );
			XUtils.AssertVal( aabb2.GetCenter(), new Vector2( 7.5f, 2.5f ), kTol );
			XUtils.AssertVal( aabb2.GetRadius(), new Vector2( 12.5f, 7.5f ), kTol );
			XUtils.AssertVal( aabb2.GetSize(), new Vector2( 25f, 15f ), kTol );
			XUtils.AssertVal( aabb2.GetArea(), 375f, kTol );

			aabb2.Reset();
			aabb2.Add( Vector2.Zero );
			aabb2.Add( oneTwo );
			aabb2.ScaleWorld( 4f );
			XUtils.AssertVal( aabb2.GetArea(), 32f, kTol );

			aabb2.Translate( fiveFive );
			Vector2 center2 = new Vector2( 7f, 9f );
			XUtils.AssertVal( aabb2.GetArea(), 32f, kTol );
			XUtils.AssertVal( aabb2.GetCenter(), center2, kTol );
			XUtils.Assert( !aabb2.Contains( oneTwo ) );
			XUtils.Assert( aabb2.Contains( new Vector2( 6f, 8f ) ) );

			//aabb2.ScaleWorld( -1f ); asserts negative scalar, good
			//aabb2.ScaleLocal( -50f ); asserts negative scalar, good
			aabb2.ScaleLocal( 0.25f );
			XUtils.AssertVal( aabb2.GetCenter(), center2, kTol );
			XUtils.AssertVal( aabb2.GetArea(), 2f, kTol );
			XUtils.Assert( !aabb2.Contains( new Vector2( 6f, 8f ) ) );
			XUtils.Assert( aabb2.Contains( center2 ) );

			aabb2.Reset();
			aabb2.Add( Vector2.Zero );
			aabb2.Add( oneTwo );
			aabb2.Resize( new Vector2( 0.1f, -0.3f ) );
			XUtils.AssertVal( aabb2.GetArea(), 1.68f, kTol );
			XUtils.Assert( !aabb2.Contains( Vector2.Zero ) );
			XUtils.Assert( aabb2.Contains( new Vector2( -0.05f, 1.4f ) ) );
		}
	}
}
