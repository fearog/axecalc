﻿#region netDxf, Copyright(C) 2009 Daniel Carvajal, Licensed under LGPL.

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
using System.Collections.Generic;
using netDxf.Tables;

namespace netDxf.Entities
{

    /// <summary>
    /// Represents a circular arc <see cref="netDxf.Entities.IEntityObject">entity</see>.
    /// </summary>
    public class Arc :
        DxfObject,
        IEntityObject 
    {
        #region private fields

        private const EntityType TYPE = EntityType.Arc;
        private Vector3d center;
        private double radius;
        private double startAngle;
        private double endAngle;
        private double thickness;
        private Vector3d normal;
        private AciColor color;
        private Layer layer;
        private LineType lineType;
        private Dictionary<ApplicationRegistry, XData> xData;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Arc</c> class.
        /// </summary>
        /// <param name="center">Arc <see cref="netDxf.Vector3d">center</see> in object coordinates.</param>
        /// <param name="radius">Arc radius.</param>
        /// <param name="startAngle">Arc start angle in degrees.</param>
        /// <param name="endAngle">Arc end angle in degrees.</param>
        /// <remarks>The center Z coordinate represents the elevation of the arc along the normal.</remarks>
        public Arc(Vector3d center, double radius, double startAngle, double endAngle) : base (DxfObjectCode.Arc)
        {
            this.center = center;
            this.radius = radius;
            this.startAngle = startAngle;
            this.endAngle = endAngle;
            this.thickness = 0.0f;
            this.layer = Layer.Default;
            this.color = AciColor.ByLayer;
            this.lineType = LineType.ByLayer;
            this.normal = Vector3d.UnitZ;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Arc</c> class.
        /// </summary>
        public Arc() :
            base(DxfObjectCode.Arc)
        {
            this.center = Vector3d.Zero;
            this.radius = 0.0f;
            this.startAngle = 0.0f;
            this.endAngle = 0.0f;
            this.thickness = 0.0f;
            this.layer = Layer.Default;
            this.color = AciColor.ByLayer;
            this.lineType = LineType.ByLayer;
            this.normal = Vector3d.UnitZ;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the arc <see cref="netDxf.Vector3d">center</see>.
        /// </summary>
        /// <remarks>The center Z coordinate represents the elevation of the arc along the normal.</remarks>
        public Vector3d Center
        {
            get { return this.center; }
            set { this.center = value; }
        }

        /// <summary>
        /// Gets or sets the arc radius.
        /// </summary>
        public double Radius
        {
            get { return this.radius; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", value.ToString());
                this.radius = value;
            }
        }

        /// <summary>
        /// Gets or sets the arc start angle in degrees.
        /// </summary>
        public double StartAngle
        {
            get { return this.startAngle; }
            set { this.startAngle = value; }
        }

        /// <summary>
        /// Gets or sets the arc end angle in degrees.
        /// </summary>
        public double EndAngle
        {
            get { return this.endAngle; }
            set { this.endAngle = value; }
        }

        /// <summary>
        /// Gets or sets the arc thickness.
        /// </summary>
        public double Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        /// <summary>
        /// Gets or sets the arc <see cref="netDxf.Vector3d">normal</see>.
        /// </summary>
        public Vector3d Normal
        {
            get { return this.normal; }
            set
            {
                if (Vector3d.Zero == value)
                    throw new ArgumentNullException("value", "The normal can not be the zero vector");
                value.Normalize();
                this.normal = value;
            }
        }

        #endregion

        #region IEntityObject Members

       /// <summary>
        /// Gets the entity <see cref="netDxf.Entities.EntityType">type</see>.
        /// </summary>
        public EntityType Type
        {
            get { return TYPE; }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="netDxf.AciColor">color</see>.
        /// </summary>
        public AciColor Color
        {
            get { return this.color; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.color = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="netDxf.Tables.Layer">layer</see>.
        /// </summary>
        public Layer Layer
        {
            get { return this.layer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.layer = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="netDxf.Tables.LineType">line type</see>.
        /// </summary>
        public LineType LineType
        {
            get { return this.lineType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.lineType = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="netDxf.XData">extende data</see>.
        /// </summary>
        public Dictionary<ApplicationRegistry, XData> XData
        {
            get { return this.xData; }
            set{ this.xData = value;}
        }

        #endregion

        #region public methods

        /// <summary>
        /// Converts the arc in a list of vertexes.
        /// </summary>
        /// <param name="precision">Number of vertexes generated.</param>
        /// <returns>A list vertexes that represents the arc expresed in object coordinate system.</returns>
        public List<Vector2d> PoligonalVertexes(int precision)
        {
            if (precision < 2)
                throw new ArgumentOutOfRangeException("precision", precision, "The arc precision must be greater or equal to two");

            List<Vector2d> ocsVertexes = new List<Vector2d>();
            double start = (double) (this.startAngle*MathHelper.DegToRad);
            double end = (double) (this.endAngle*MathHelper.DegToRad);
            double angle = (end - start) / precision;

            for (int i = 0; i <= precision; i++)
            {
                double sine = (double)(this.radius * Math.Sin(start + angle * i));
                double cosine = (double)(this.radius * Math.Cos(start + angle * i));
                ocsVertexes.Add(new Vector2d(cosine + this.center.X, sine + this.center.Y));
            }

            return ocsVertexes;
        }

        /// <summary>
        /// Converts the arc in a list of vertexes.
        /// </summary>
        /// <param name="precision">Number of vertexes generated.</param>
        /// <param name="weldThreshold">Tolerance to consider if two new generated vertexes are equal.</param>
        /// <returns>A list vertexes that represents the arc expresed in object coordinate system.</returns>
        public List<Vector2d> PoligonalVertexes(int precision, double weldThreshold)
        {
            if (precision < 2)
                throw new ArgumentOutOfRangeException("precision", precision, "The arc precision must be greater or equal to two");

            List<Vector2d> ocsVertexes = new List<Vector2d>();
            double start = (double)(this.startAngle * MathHelper.DegToRad);
            double end = (double)(this.endAngle * MathHelper.DegToRad);

            if (2*this.radius >= weldThreshold)
            {
                double angulo = (end - start)/precision;
                Vector2d prevPoint;
                Vector2d firstPoint;

                double sine = (double) (this.radius*Math.Sin(start));
                double cosine = (double) (this.radius*Math.Cos(start));
                firstPoint = new Vector2d(cosine + this.center.X, sine + this.center.Y);
                ocsVertexes.Add(firstPoint);
                prevPoint = firstPoint;

                for (int i = 1; i <= precision; i++)
                {
                    sine = (double) (this.radius*Math.Sin(start + angulo*i));
                    cosine = (double) (this.radius*Math.Cos(start + angulo*i));
                    Vector2d point = new Vector2d(cosine + this.center.X, sine + this.center.Y);

                    if (!point.Equals(prevPoint, weldThreshold) && !point.Equals(firstPoint, weldThreshold))
                    {
                        ocsVertexes.Add(point);
                        prevPoint = point;
                    }
                }
            }

            return ocsVertexes;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return TYPE.ToString();
        }

        #endregion

    }
}