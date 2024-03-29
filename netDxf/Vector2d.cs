#region netDxf, Copyright(C) 2009 Daniel Carvajal, Licensed under LGPL.

//                        netDxf library
// Copyright (C) 2009 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 

#endregion

using System;

namespace netDxf
{
    /// <summary>
    /// Represent a two component vector of double precision.
    /// </summary>
    public struct Vector2d
    {
        #region private fields

        private double x;
        private double y;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of Vector2d.
        /// </summary>
        /// <param name="x">X component.</param>
        /// <param name="y">Y component.</param>
        public Vector2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Initializes a new instance of Vector2d.
        /// </summary>
        /// <param name="array">Array of two elements that represents the vector.</param>
        public Vector2d(double[] array)
        {
            if (array.Length != 2)
                throw new ArgumentOutOfRangeException("array", array.Length, "The dimension of the array must be two");
            this.x = array[0];
            this.y = array[1];
        }

        #endregion

        #region constants

        /// <summary>
        /// Zero vector.
        /// </summary>
        public static Vector2d Zero
        {
            get { return new Vector2d(0, 0); }
        }

        /// <summary>
        /// Unit X vector.
        /// </summary>
        public static Vector2d UnitX
        {
            get { return new Vector2d(1, 0); }
        }

        /// <summary>
        /// Unit Y vector.
        /// </summary>
        public static Vector2d UnitY
        {
            get { return new Vector2d(0, 1); }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the X component.
        /// </summary>
        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// Gets or sets the Y component.
        /// </summary>
        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        /// <summary>
        /// Gets or sets a vector element defined by its index.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:

                        return this.y;
                    default:

                        throw (new ArgumentOutOfRangeException("index"));
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:

                        this.y = value;
                        break;
                    default:

                        throw (new ArgumentOutOfRangeException("index"));
                }
            }
        }

        #endregion

        #region static methods

        /// <summary>
        /// Obtains the dot product of two vectors.
        /// </summary>
        /// <param name="u">Vector2d.</param>
        /// <param name="v">Vector2d.</param>
        /// <returns>The dot product.</returns>
        public static double DotProduct(Vector2d u, Vector2d v)
        {
            return (u.X*v.X) + (u.Y*v.Y);
        }

        /// <summary>
        /// Obtains the cross product of two vectors.
        /// </summary>
        /// <param name="u">Vector2d.</param>
        /// <param name="v">Vector2d.</param>
        /// <returns>Vector2d.</returns>
        public static double CrossProduct(Vector2d u, Vector2d v)
        {
            return (u.X*v.Y) - (u.Y*v.X);
        }

        /// <summary>
        /// Obtains the counter clockwise perpendicular vector .
        /// </summary>
        /// <param name="u">Vector2d.</param>
        /// <returns>Vector2d.</returns>
        public static Vector2d Perpendicular(Vector2d u)
        {
            return new Vector2d(-u.Y, u.X);
        }

        /// <summary>
        /// Obtains the distance between two points.
        /// </summary>
        /// <param name="u">Vector2d.</param>
        /// <param name="v">Vector2d.</param>
        /// <returns>Distancie.</returns>
        public static double Distance(Vector2d u, Vector2d v)
        {
            return (Math.Sqrt((u.X - v.X)*(u.X - v.X) + (u.Y - v.Y)*(u.Y - v.Y)));
        }

        /// <summary>
        /// Obtains the square distance between two points.
        /// </summary>
        /// <param name="u">Vector2d.</param>
        /// <param name="v">Vector2d.</param>
        /// <returns>Square distance.</returns>
        public static double SquareDistance(Vector2d u, Vector2d v)
        {
            return (u.X - v.X)*(u.X - v.X) + (u.Y - v.Y)*(u.Y - v.Y);
        }

        /// <summary>
        /// Obtains the angle between two vectors.
        /// </summary>
        /// <param name="u">Vector2d.</param>
        /// <param name="v">Vector2d.</param>
        /// <returns>Angle in radians.</returns>
        public static double AngleBetween(Vector2d u, Vector2d v)
        {
            double cos = DotProduct(u, v)/(u.Modulus()*v.Modulus());
            if (MathHelper.IsOne(cos))
            {
                return 0;
            }
            if (MathHelper.IsOne(-cos))
            {
                return Math.PI;
            }
            return Math.Acos(cos);

            //if (AreParallel(u, v))
            //{
            //    if (Math.Sign(u.X) == Math.Sign(v.X) && Math.Sign(u.Y) == Math.Sign(v.Y))
            //    {
            //        return 0;
            //    }
            //    return (double)Math.PI;
            //}
            //Vector3d normal = Vector3d.CrossProduct(new Vector3d(u.X, u.Y, 0), new Vector3d(v.X, v.Y, 0));

            //if (normal.Z < 0)
            //{
            //    return (double)(2 * Math.PI - Math.Acos(DotProduct(u, v) / (u.Modulus() * v.Modulus())));
            //}
            //return (double)(Math.Acos(DotProduct(u, v) / (u.Modulus() * v.Modulus())));
        }


        /// <summary>
        /// Obtains the midpoint.
        /// </summary>
        /// <param name="u">Vector2d.</param>
        /// <param name="v">Vector2d.</param>
        /// <returns>Vector2d.</returns>
        public static Vector2d MidPoint(Vector2d u, Vector2d v)
        {
            return new Vector2d((v.X + u.X)*0.5F, (v.Y + u.Y)*0.5F);
        }

        /// <summary>
        /// Checks if two vectors are perpendicular.
        /// </summary>
        /// <param name="u">Vector2d.</param>
        /// <param name="v">Vector2d.</param>
        /// <param name="threshold">Tolerance used.</param>
        /// <returns>True if are penpendicular or false in anyother case.</returns>
        public static bool ArePerpendicular(Vector2d u, Vector2d v, double threshold)
        {
            return MathHelper.IsZero(DotProduct(u, v), threshold);
        }

        /// <summary>
        /// Checks if two vectors are parallel.
        /// </summary>
        /// <param name="u">Vector2d.</param>
        /// <param name="v">Vector2d.</param>
        /// <param name="threshold">Tolerance used.</param>
        /// <returns>True if are parallel or false in anyother case.</returns>
        public static bool AreParallel(Vector2d u, Vector2d v, double threshold)
        {
            double a = u.X*v.Y - u.Y*v.X;
            return MathHelper.IsZero(a, threshold);
        }

        /// <summary>
        /// Rounds the components of a vector.
        /// </summary>
        /// <param name="u">Vector2d.</param>
        /// <param name="numDigits">Number of significative defcimal digits.</param>
        /// <returns>Vector2d.</returns>
        public static Vector2d Round(Vector2d u, int numDigits)
        {
            return new Vector2d(Math.Round(u.X, numDigits), Math.Round(u.Y, numDigits));
        }

        #endregion

        #region overloaded operators

        public static bool operator ==(Vector2d u, Vector2d v)
        {
            return ((v.X == u.X) && (v.Y == u.Y));
        }

        public static bool operator !=(Vector2d u, Vector2d v)
        {
            return ((v.X != u.X) || (v.Y != u.Y));
        }

        public static Vector2d operator +(Vector2d u, Vector2d v)
        {
            return new Vector2d(u.X + v.X, u.Y + v.Y);
        }

        public static Vector2d operator -(Vector2d u, Vector2d v)
        {
            return new Vector2d(u.X - v.X, u.Y - v.Y);
        }

        public static Vector2d operator -(Vector2d u)
        {
            return new Vector2d(-u.X, -u.Y);
        }

        public static Vector2d operator *(Vector2d u, double a)
        {
            return new Vector2d(u.X*a, u.Y*a);
        }

        public static Vector2d operator *(double a, Vector2d u)
        {
            return new Vector2d(u.X*a, u.Y*a);
        }

        public static Vector2d operator /(Vector2d u, double a)
        {
            double invEscalar = 1/a;
            return new Vector2d(u.X*invEscalar, u.Y*invEscalar);
        }

        public static Vector2d operator /(double a, Vector2d u)
        {
            double invEscalar = 1/a;
            return new Vector2d(u.X*invEscalar, u.Y*invEscalar);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public void Normalize()
        {
            double mod = this.Modulus();
            if (mod == 0)
                throw new ArithmeticException("Cannot normalize a zero vector");
            double modInv = 1/mod;
            this.x *= modInv;
            this.y *= modInv;
        }

        /// <summary>
        /// Obtains the modulus of the vector.
        /// </summary>
        /// <returns>Vector modulus.</returns>
        public double Modulus()
        {
            return (Math.Sqrt(DotProduct(this, this)));
        }

        /// <summary>
        /// Returns an array that represents the vector.
        /// </summary>
        /// <returns>Array.</returns>
        public double[] ToArray()
        {
            var u = new[] {this.x, this.y};
            return u;
        }

        #endregion

        #region comparision methods

        /// </summary>
        /// Check if the components of two vectors are approximate equals.
        /// <param name="obj">Vector2d.</param>
        /// <param name="threshold">Maximun tolerance.</param>
        /// <returns>True if the three components are almost equal or false in anyother case.</returns>
        public bool Equals(Vector2d obj, double threshold)
        {
            if (Math.Abs(obj.X - this.x) > threshold)
            {
                return false;
            }
            if (Math.Abs(obj.Y - this.y) > threshold)
            {
                return false;
            }

            return true;
        }

        public bool Equals(Vector2d obj)
        {
            return obj.x == this.x && obj.y == this.y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2d)
                return this.Equals((Vector2d) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return unchecked(this.x.GetHashCode() ^ this.y.GetHashCode());
        }

        #endregion

        #region overrides

        /// <summary>
        /// Obtains a string that represents the vector.
        /// </summary>
        /// <returns>A string text.</returns>
        public override string ToString()
        {
            return string.Format("{0};{1}", this.x, this.y);
        }

        /// <summary>
        /// Obtains a string that represents the vector.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A string text.</returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format("{0};{1}", this.x.ToString(provider), this.y.ToString(provider));
        }

        #endregion
    }
}