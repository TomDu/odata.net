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

namespace System.Data.Services.Client
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using Microsoft.Data.OData;

    /// <summary>
    /// This class represents the contract WCF Data Services client with the request message.
    /// </summary>
    public abstract class DataServiceClientRequestMessage : IODataRequestMessage
    {
        /// <summary>Http method.</summary>
        private readonly string actualHttpMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceClientRequestMessage"/> class.
        /// </summary>
        /// <param name="actualMethod">The actual method.</param>
        public DataServiceClientRequestMessage(string actualMethod)
        {
            this.actualHttpMethod = actualMethod;
        }

        /// <summary>
        /// Returns the collection of request headers.
        /// </summary>
        public abstract IEnumerable<KeyValuePair<string, string>> Headers
        {
            get;
        }

        /// <summary>
        /// Gets or sets the request url.
        /// </summary>
        public abstract Uri Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the method for this request.
        /// </summary>
        public abstract string Method
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or set the credentials for this request.
        /// </summary>
        public abstract ICredentials Credentials { get; set; }

#if !ASTORIA_LIGHT && !PORTABLELIB
        /// <summary>
        /// Gets or sets the timeout (in seconds) for this request.
        /// </summary>
        public abstract int Timeout { get; set; }
#endif

        /// <summary>
        /// Gets or sets a value that indicates whether to send data in segments to the
        ///  Internet resource. 
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Make code very confusing and cumbersome to write code for various platforms. Hence suppressing the message")]
#if !ASTORIA_LIGHT && !PORTABLELIB
        public abstract bool SendChunked { get; set; }
#else
        internal bool SendChunked { get; set; }
#endif

        /// <summary>
        /// Gets or the actual method. In post tunneling situations method will be POST instead of the specified verb method.
        /// </summary>
        protected virtual string ActualMethod
        {
            get
            {
                return this.actualHttpMethod;
            }
        }

        /// <summary>
        /// Returns the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>Returns the value of the header with the given name.</returns>
        public abstract string GetHeader(string headerName);

        /// <summary>
        /// Sets the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value of the header.</param>
        public abstract void SetHeader(string headerName, string headerValue);

        /// <summary>
        /// Gets the stream to be used to write the request payload.
        /// </summary>
        /// <returns>Stream to which the request payload needs to be written.</returns>
        public abstract Stream GetStream();

        /// <summary>
        /// Abort the current request.
        /// </summary>
        public abstract void Abort();

        /// <summary>
        /// Begins an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request.</returns>
        public abstract IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state);

        /// <summary>
        /// Ends an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="asyncResult">The pending request for a stream.</param>
        /// <returns>A System.IO.Stream to use to write request data.</returns>
        public abstract Stream EndGetRequestStream(IAsyncResult asyncResult);

        /// <summary>
        ///  Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request for a response.</returns>
        public abstract IAsyncResult BeginGetResponse(AsyncCallback callback, object state);

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response.</param>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        public abstract IODataResponseMessage EndGetResponse(IAsyncResult asyncResult);

#if !ASTORIA_LIGHT && !PORTABLELIB
        /// <summary>
        /// Returns a response from an Internet resource.
        /// </summary>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is intentionally a method.")]
        public abstract IODataResponseMessage GetResponse();
#endif
    }
}
