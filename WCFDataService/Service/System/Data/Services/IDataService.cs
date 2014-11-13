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

namespace System.Data.Services
{
    #region Namespaces

    using System.Data.Services.Providers;
    using System.Linq;
#if ASTORIA_FF_CALLBACKS
    using System.ServiceModel.Syndication;
#endif
    using Microsoft.Data.OData;

    #endregion Namespaces

    /// <summary>Provides a non-generic interface for web data services.</summary>
    internal interface IDataService
    {
        #region Properties

        /// <summary>Service configuration information.</summary>
        DataServiceConfiguration Configuration
        {
            get;
        }

        /// <summary>Data provider for this data service.</summary>
        DataServiceProviderWrapper Provider
        {
            get;
        }

        /// <summary>Returns the instance of the data service.</summary>
        object Instance
        {
            get;
        }

        /// <summary>Context for current operation.</summary>
        DataServiceOperationContext OperationContext
        {
            get;
        }

        /// <summary>Processing pipeline events</summary>
        /// <remarks>
        /// Note that this is the same as DataService&lt;T&gt;.ProcessingPipeline.  Internally we pass IDataService around and
        /// we can't always cast IDataService back to DataService&lt;T&gt; easily because we don't always know what T is.
        /// IDataService.ProcessingPipeline is internal and it makes the pipeline object more accessible.
        /// </remarks>
        DataServiceProcessingPipeline ProcessingPipeline
        {
            get;
        }

        /// <summary>IUpdatable interface for this provider</summary>
        UpdatableWrapper Updatable
        {
            get;
        }

        /// <summary>Reference to IDataServiceStreamProvider interface</summary>
        DataServiceStreamProviderWrapper StreamProvider
        {
            get;
        }

        /// <summary>Reference to the wrapper to IDataServicePagingProvider interface.</summary>
        DataServicePagingProviderWrapper PagingProvider
        {
            get;
        }

        /// <summary>Reference to the wrapper for the IDataServiceExecutionProvider interface.</summary>
        DataServiceExecutionProviderWrapper ExecutionProvider
        {
            get;
        }

        /// <summary>Reference to the wrapper for the IDataServiceActionProvider interface.</summary>
        DataServiceActionProviderWrapper ActionProvider
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>Processes a catchable exception.</summary>
        /// <param name="args">The arguments describing how to handle the exception.</param>
        void InternalHandleException(HandleExceptionArgs args);

#if ASTORIA_FF_CALLBACKS
        /// <summary>
        /// Invoked once feed has been written to override the feed elements
        /// </summary>
        /// <param name="feed">Feed being written</param>
        void InternalOnWriteFeed(SyndicationFeed feed);

        /// <summary>
        /// Invoked once an element has been written to override the element
        /// </summary>
        /// <param name="item">Item that has been written</param>
        /// <param name="obj">Object with content for the <paramref name="item"/></param>
        void InternalOnWriteItem(SyndicationItem item, object obj);
#endif
        /// <summary>
        /// Returns the segmentInfo of the resource referred by the given content Id;
        /// </summary>
        /// <param name="contentId">content id for a operation in the batch request.</param>
        /// <returns>segmentInfo for the resource referred by the given content id.</returns>
        SegmentInfo GetSegmentForContentId(string contentId);

        /// <summary>
        /// Get the resource referred by the segment in the request with the given index
        /// </summary>
        /// <param name="description">description about the request url.</param>
        /// <param name="segmentIndex">index of the segment that refers to the resource that needs to be returned.</param>
        /// <param name="typeFullName">typename of the resource.</param>
        /// <returns>the resource as returned by the provider.</returns>
        object GetResource(RequestDescription description, int segmentIndex, string typeFullName);

        /// <summary>
        /// Dispose the data source instance and set it to null
        /// </summary>
        void DisposeDataSource();

        /// <summary>
        /// This method is called before a request is processed.
        /// </summary>
        /// <param name="args">Information about the request that is going to be processed.</param>
        void InternalOnStartProcessingRequest(ProcessRequestArgs args);

        /// <summary>
        /// This method is called once the request query is constructed.
        /// </summary>
        /// <param name="query">The query which is going to be executed against the provider.</param>
        void InternalOnRequestQueryConstructed(IQueryable query);

        /// <summary>
        /// Method to wrap the current DataServiceODataWriter with custom one to intercept 
        /// WCF Data Services calls to ODataWriter. This enables seeing the ODataFeed/ODataEntry/
        /// ODataNavigationLink instances that gets passed to underlying instance.
        /// </summary>
        /// <param name="odataWriter">DataServiceODataWriter instance to wrap.</param>
        /// <returns>an instance of DataServiceODataWriter.</returns>
        DataServiceODataWriter CreateODataWriterWrapper(ODataWriter odataWriter);

        #endregion Methods
    }
}
