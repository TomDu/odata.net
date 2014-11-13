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
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Stores information about a association and its ends
    /// </summary>
    public class ResourceAssociationType
    {
        /// <summary>FullName of the association.</summary>
        private readonly string fullName;

        /// <summary>Name of the association </summary>
        private readonly string name;

        /// <summary>Namespace of the association type.</summary>
        private readonly string namespaceName;

        /// <summary>end1 for this association.</summary>
        private readonly ResourceAssociationTypeEnd end1;

        /// <summary>end2 for this association.</summary>
        private readonly ResourceAssociationTypeEnd end2;

        /// <summary>list of custom annotations that needs to be flowed via $metadata endpoint.</summary>
        private Dictionary<string, object> customAnnotations;

        /// <summary>
        /// Creates a new instance of AssociationInfo to store information about an association.
        /// </summary>
        /// <param name="name">name of the association.</param>
        /// <param name="namespaceName">namespaceName of the association.</param>
        /// <param name="end1">first end of the association.</param>
        /// <param name="end2">second end of the association.</param>
        public ResourceAssociationType(string name, string namespaceName, ResourceAssociationTypeEnd end1, ResourceAssociationTypeEnd end2)
        {
            Debug.Assert(!String.IsNullOrEmpty(name), "!String.IsNullOrEmpty(name)");
            Debug.Assert(end1 != null && end2 != null, "end1 != null && end2 != null");

            this.name = name;
            this.namespaceName = namespaceName;
            this.fullName = namespaceName + "." + name;
            this.end1 = end1;
            this.end2 = end2;
        }

        /// <summary>Returns the instance of ResourceReferentialConstraint.</summary>
        public ResourceReferentialConstraint ReferentialConstraint
        {
            get;
            set;
        }

        /// <summary>FullName of the association.</summary>
        internal string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>Name of the association.</summary>
        internal string Name
        {
            get { return this.name; }
        }

        /// <summary>Namespace of the association type.</summary>
        internal string NamespaceName
        {
            get { return this.namespaceName; }
        }

        /// <summary>end1 for this association.</summary>
        internal ResourceAssociationTypeEnd End1
        {
            get { return this.end1; }
        }

        /// <summary>end2 for this association.</summary>
        internal ResourceAssociationTypeEnd End2
        {
            get { return this.end2; }
        }

        /// <summary>
        /// Returns the list of custom annotations defined on this set.
        /// </summary>
        internal IEnumerable<KeyValuePair<string, object>> CustomAnnotations
        {
            get
            {
                if (this.customAnnotations == null)
                {
                    return WebUtil.EmptyKeyValuePairStringObject;
                }

                return this.customAnnotations;
            }
        }

        /// <summary>
        /// Add the given annotation to the list of annotations that needs to flowed via the $metadata endpoint
        /// </summary>
        /// <param name="annotationNamespace">NamespaceName to which the custom annotation belongs to.</param>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <param name="annotationValue">Value of the annotation.</param>
        public void AddCustomAnnotation(string annotationNamespace, string annotationName, object annotationValue)
        {
            WebUtil.ValidateAndAddAnnotation(ref this.customAnnotations, annotationNamespace, annotationName, annotationValue);
        }

        /// <summary>
        /// Return the end with the given name.
        /// </summary>
        /// <param name="endName">Name of the end.</param>
        /// <returns>An instance of the end with the given name.</returns>
        public ResourceAssociationTypeEnd GetEnd(string endName)
        {
            if (this.end1.Name == endName)
            {
                return this.end1;
            }

            if (this.end2.Name == endName)
            {
                return this.end2;
            }

            Debug.Assert(false, "GetEnd: The endName should be always valid");
            return null;
        }

        /// <summary>
        /// Retrieve the end for the given resource set, type and property.
        /// </summary>
        /// <param name="resourceType">resource type for the end</param>
        /// <param name="resourceProperty">resource property for the end</param>
        /// <returns>Association type end for the given parameters</returns>
        internal ResourceAssociationTypeEnd GetResourceAssociationTypeEnd(ResourceType resourceType, ResourceProperty resourceProperty)
        {
            Debug.Assert(resourceType != null, "resourceType != null");

            foreach (ResourceAssociationTypeEnd end in new[] { this.end1, this.end2 })
            {
                if (end.ResourceType == resourceType && end.ResourceProperty == resourceProperty)
                {
                    return end;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve the related end for the given resource set, type and property.
        /// </summary>
        /// <param name="resourceType">resource type for the source end</param>
        /// <param name="resourceProperty">resource property for the source end</param>
        /// <returns>Related association type end for the given parameters</returns>
        internal ResourceAssociationTypeEnd GetRelatedResourceAssociationSetEnd(ResourceType resourceType, ResourceProperty resourceProperty)
        {
            Debug.Assert(resourceType != null, "resourceType != null");

            ResourceAssociationTypeEnd thisEnd = this.GetResourceAssociationTypeEnd(resourceType, resourceProperty);

            if (thisEnd != null)
            {
                foreach (ResourceAssociationTypeEnd end in new[] { this.end1, this.end2 })
                {
                    if (end != thisEnd)
                    {
                        return end;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the related end with the given name.
        /// </summary>
        /// <param name="endName">Name of the end.</param>
        /// <returns>Returns the related end of the end with the given name.</returns>
        internal ResourceAssociationTypeEnd GetRelatedEnd(string endName)
        {
            if (this.end1.Name == endName)
            {
                return this.end2;
            }

            if (this.end2.Name == endName)
            {
                return this.end1;
            }

            Debug.Assert(false, "GetRelatedEnd: The endName should be always valid");
            return null;
        }
    }
}
