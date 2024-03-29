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

namespace netDxf
{
    /// <summary>
    /// Dxf entities codes.
    /// </summary>
    public sealed class DxfObjectCode
    {
        /// <summary>
        /// application registry.
        /// </summary>
        public const string AppId = "APPID";

        /// <summary>
        /// dimension style.
        /// </summary>
        public const string DimStyle = "DIMSTYLE";

        /// <summary>
        /// block record.
        /// </summary>
        public const string BlockRecord = "BLOCK_RECORD";

        /// <summary>
        /// line type.
        /// </summary>
        public const string LineType = "LTYPE";

        /// <summary>
        /// layer.
        /// </summary>
        public const string Layer = "LAYER";

        /// <summary>
        /// viewport.
        /// </summary>
        public const string ViewPort = "VPORT";

        /// <summary>
        /// text style.
        /// </summary>
        public const string TextStyle = "STYLE";

        /// <summary>
        /// view.
        /// </summary>
        public const string View = "VIEW";

        /// <summary>
        /// ucs.
        /// </summary>
        public const string Ucs = "UCS";

        /// <summary>
        /// block.
        /// </summary>
        public const string Block = "BLOCK";

        /// <summary>
        /// block.
        /// </summary>
        public const string BlockEnd = "ENDBLK";

        /// <summary>
        /// line.
        /// </summary>
        public const string Line = "LINE";

        /// <summary>
        /// ellipse.
        /// </summary>
        public const string Ellipse = "ELLIPSE";

        /// <summary>
        /// polyline.
        /// </summary>
        public const string Polyline = "POLYLINE";

        /// <summary>
        /// light weight polyline.
        /// </summary>
        public const string LightWeightPolyline = "LWPOLYLINE";

        /// <summary>
        /// circle.
        /// </summary>
        public const string Circle = "CIRCLE";

        /// <summary>
        /// point.
        /// </summary>
        public const string Point = "POINT";

        /// <summary>
        /// arc.
        /// </summary>
        public const string Arc = "ARC";

        /// <summary>
        /// solid.
        /// </summary>
        public const string Solid = "SOLID";

        /// <summary>
        /// text string.
        /// </summary>
        public const string Text = "TEXT";

        /// <summary>
        /// 3d face.
        /// </summary>
        public const string Face3D = "3DFACE";

        /// <summary>
        /// block insertion.
        /// </summary>
        public const string Insert = "INSERT";

        /// <summary>
        /// hatch.
        /// </summary>
        public const string Hatch = "HATCH";

        /// <summary>
        /// attribute definition.
        /// </summary>
        public const string AttributeDefinition = "ATTDEF";

        /// <summary>
        /// attribute.
        /// </summary>
        public const string Attribute = "ATTRIB";

        /// <summary>
        /// vertex.
        /// </summary>
        public const string Vertex = "VERTEX";

        /// <summary>
        /// end sequence.
        /// </summary>
        public const string EndSequence = "SEQEND";

        /// <summary>
        /// dim.
        /// </summary>
        public const string Dimension = "DIMENSION";

        /// <summary>
        /// dictionary.
        /// </summary>
        public const string Dictionary = "DICTIONARY";
    }
}