// -----------------------------------------------------------------------
// <copyright file="Primitive.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System;
    using AdminToys;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="AdminToys.PrimitiveObjectToy"/>.
    /// </summary>
    public class Primitive : AdminToy
    {
        private bool collidable = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Primitive"/> class.
        /// </summary>
        /// <param name="toyAdminToyBase">The <see cref="PrimitiveObjectToy"/> of the toy.</param>
        public Primitive(PrimitiveObjectToy toyAdminToyBase)
            : base(toyAdminToyBase, AdminToyType.PrimitiveObject)
        {
            Base = toyAdminToyBase;

            Vector3 actualScale = Base.transform.localScale;
            Collidable = actualScale.x > 0f || actualScale.y > 0f || actualScale.z > 0f;
        }

        /// <summary>
        /// Gets the base <see cref="PrimitiveObjectToy"/>.
        /// </summary>
        public PrimitiveObjectToy Base { get; }

        /// <summary>
        /// Gets or sets the type of the primitive.
        /// </summary>
        public PrimitiveType Type
        {
            get => Base.NetworkPrimitiveType;
            set => Base.NetworkPrimitiveType = value;
        }

        /// <summary>
        /// Gets or sets the material color of the primitive.
        /// </summary>
        public Color Color
        {
            get => Base.NetworkMaterialColor;
            set => Base.NetworkMaterialColor = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the primitive can be collided with.
        /// </summary>
        public bool Collidable
        {
            get => collidable;
            set
            {
                collidable = value;
                RefreshCollidable();
            }
        }

        /// <summary>
        /// Gets or sets the scale of the primitive.
        /// </summary>
        public override Vector3 Scale
        {
            get => AdminToyBase.transform.localScale;
            set
            {
                AdminToyBase.transform.localScale = value;
                RefreshCollidable();
            }
        }

        /// <summary>
        /// Creates a new <see cref="Primitive"/>.
        /// </summary>
        /// <returns>The new primitive.</returns>
        public static Primitive Create() => new Primitive(UnityEngine.Object.Instantiate(ToysHelper.PrimitiveBaseObject));

        private void RefreshCollidable()
        {
            Vector3 actualScale = Scale;

            if (Collidable)
            {
                Base.transform.localScale = new Vector3(Math.Abs(actualScale.x), Math.Abs(actualScale.y), Math.Abs(actualScale.z));
            }
            else
            {
                Base.transform.localScale = new Vector3(-Math.Abs(actualScale.x), -Math.Abs(actualScale.y), -Math.Abs(actualScale.z));
            }
        }
    }
}
