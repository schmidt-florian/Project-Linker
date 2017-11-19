//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectLinker.Helper.Design
{
    /// <summary>
    ///     Converter for <see cref="HierarchyNode" /> objects
    /// </summary>
    public class HierarchyNodeConverter : TypeConverter
    {
        /// <summary>
        ///     Specifies conversion from <see cref="string" /> type
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            Guard.ArgumentNotNull(context, "context");
            if (sourceType == typeof(string) || sourceType == typeof(HierarchyNode))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///     Specifies conversion to <see cref="string" /> type
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            Guard.ArgumentNotNull(context, "context");
            if (destinationType == typeof(string) || destinationType == typeof(HierarchyNode))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        ///     Converts from string objects
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Guard.ArgumentNotNull(context, "context");
            if (value is string)
            {
                IVsSolution vsSolution = (IVsSolution) context.GetService(typeof(IVsSolution));
                return new HierarchyNode(vsSolution, value.ToString());
            }
            if (value is HierarchyNode)
            {
                return ((HierarchyNode) value).UniqueName;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///     Converts to string objects
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Guard.ArgumentNotNull(context, "context");
            if (destinationType == typeof(string))
            {
                if (value is HierarchyNode node)
                {
                    return node.UniqueName;
                }
            }
            else if (destinationType == typeof(HierarchyNode))
            {
                if (value is string)
                {
                    IVsSolution vsSolution = (IVsSolution) context.GetService(typeof(IVsSolution));
                    return new HierarchyNode(vsSolution, value.ToString());
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}