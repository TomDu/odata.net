//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

    /// <summary>
    /// Represents a navigation property synthesized for an association end that does not have a corresponding navigation property.
    /// </summary>
    internal class MetadataProviderEdmSilentNavigationProperty : EdmElement, IEdmNavigationProperty
    {
        /// <summary>The destination end of this navigation property.</summary>
        private readonly IEdmNavigationProperty partner;

        /// <summary>The type of the navigation property.</summary>
        private readonly IEdmTypeReference type;

        /// <summary>The on-delete action of the navigation property.</summary>
        private readonly EdmOnDeleteAction deleteAction;

        /// <summary>The name of this navigation property.</summary>
        private readonly string name;

        /// <summary>The dependent properties of the referential constraint.</summary>
        private ReadOnlyCollection<IEdmStructuralProperty> dependentProperties;

        /// <summary>
        /// Creates a new Silent partner for a navigation property
        /// </summary>
        /// <param name="partnerProperty">The navigation property this is a silent partner of.</param>
        /// <param name="propertyDeleteAction">The on delete action for this side of the association</param>
        /// <param name="multiplicity">The multiplicity of this side of the association.</param>
        /// <param name="name">The name of this navigation property.</param>
        public MetadataProviderEdmSilentNavigationProperty(IEdmNavigationProperty partnerProperty, EdmOnDeleteAction propertyDeleteAction, EdmMultiplicity multiplicity, string name)
        {
            this.partner = partnerProperty;
            this.deleteAction = propertyDeleteAction;
            this.name = name;
            switch (multiplicity)
            {
                case EdmMultiplicity.One:
                    this.type = new EdmEntityTypeReference(this.partner.DeclaringEntityType(), false);
                    break;
                case EdmMultiplicity.ZeroOrOne:
                    this.type = new EdmEntityTypeReference(this.partner.DeclaringEntityType(), true);
                    break;
                case EdmMultiplicity.Many:
                    this.type = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.partner.DeclaringEntityType(), false)), false);
                    break;
            }
        }

        /// <summary>
        /// Gets the destination end of this navigation property.
        /// </summary>
        public IEdmNavigationProperty Partner
        {
            get { return this.partner; }
        }

        /// <summary>
        /// Gets the action to execute on the deletion of this end of a bidirectional association.
        /// </summary>
        public EdmOnDeleteAction OnDelete
        {
            get { return this.deleteAction; }
        }

        /// <summary>
        /// Gets whether this navigation property originates at the principal end of an association.
        /// </summary>
        public bool IsPrincipal
        {
            get { return this.partner.DependentProperties != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the navigation target is contained inside the navigation source.
        /// </summary>
        public bool ContainsTarget
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the dependent properties of this navigation property, returning null if this is the principal end or if there is no referential constraint.
        /// </summary>
        public IEnumerable<IEdmStructuralProperty> DependentProperties
        {
            get
            {
                return this.dependentProperties;
            }
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Navigation; }
        }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmStructuredType DeclaringType
        {
            get { return this.partner.ToEntityType(); }
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Dependent properties of this navigation property.
        /// </summary>
        /// <param name="properties">The dependent properties</param>
        internal void SetDependentProperties(IList<IEdmStructuralProperty> properties)
        {
            this.dependentProperties = new ReadOnlyCollection<IEdmStructuralProperty>(properties);
        }
    }
}
