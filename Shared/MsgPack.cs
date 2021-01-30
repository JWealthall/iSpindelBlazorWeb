#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;

namespace iSpindelBlazorWeb.Shared
{
    public static class MsgPack
    {
        public static string MessagePackMediaType = "application/x-msgpack";
        
        public static MessagePackSerializerOptions CustomFormatter =
            MessagePackSerializerOptions.Standard.WithResolver(
                    CompositeResolver.Create(
                        NativeDecimalResolver.Instance,
                        NativeGuidResolver.Instance,
                        NativeDateTimeResolver.Instance,
                        StandardResolver.Instance,
                        ContractlessStandardResolver.Instance))
            //.WithCompression(MessagePackCompression.Lz4BlockArray)
            ;

        #region Get
        public static Task<T?> GetFromMessagePackAsync<T>(this HttpClient client, string? requestUri, MessagePackSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var resultTask = client.GetMessagePackAsync(requestUri, cancellationToken);
            return GetFromMessagePackAsyncCore<T>(resultTask, options, cancellationToken);
        }

        public static Task<T?> GetFromMessagePackAsync<T>(this HttpClient client, Uri? requestUri, MessagePackSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var resultTask = client.GetMessagePackAsync(requestUri, cancellationToken);
            return GetFromMessagePackAsyncCore<T>(resultTask, options, cancellationToken);
        }

        public static Task<T?> GetFromMessagePackAsync<T>(this HttpClient client, string? requestUri, CancellationToken cancellationToken = default)
        {
            // Run with standard options
            //return await client.GetFromMessagePackAsync<T>(requestUri, ContractlessStandardResolver.Options, cancellationToken);

            // Can be run with compression - will make a smaller sizer, but is slower to run
            //return await client.GetFromMessagePackAsync<T>(requestUri, ContractlessStandardResolver.Options.WithCompression(MessagePackCompression.Lz4BlockArray), cancellationToken);

            // Can be run with custom resolver
            return client.GetFromMessagePackAsync<T>(requestUri, CustomFormatter, cancellationToken);

        }

        public static Task<T?> GetFromMessagePackAsync<T>(this HttpClient client, Uri? requestUri, CancellationToken cancellationToken = default)
            => client.GetFromMessagePackAsync<T>(requestUri, CustomFormatter, cancellationToken);
        
        public static async Task<HttpResponseMessage?> GetMessagePackAsync(this HttpClient client, string? requestUri, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MessagePackMediaType));
            return await client.SendAsync(request, cancellationToken);
        }

        public static async Task<HttpResponseMessage?> GetMessagePackAsync(this HttpClient client, Uri? requestUri, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MessagePackMediaType));
            return await client.SendAsync(request, cancellationToken);
        }

        private static async Task<T?> GetFromMessagePackAsyncCore<T>(Task<HttpResponseMessage?> taskResponse, MessagePackSerializerOptions? options, CancellationToken cancellationToken)
        {
            using (HttpResponseMessage response = await taskResponse.ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                // Nullable forgiving reason:
                // GetAsync will usually return Content as not-null.
                // If Content happens to be null, the extension will throw.
                return await response.Content!.ReadFromMessagePackAsync<T>(options, cancellationToken).ConfigureAwait(false);
            }
        }
        #endregion Get

        #region Post
        public static Task<HttpResponseMessage> PostAsMessagePackAsync<T>(this HttpClient client, string? requestUri, T value, MessagePackSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            return client.SendAsMessagePackAsyncCore(request, value, options, cancellationToken);
        }

        public static Task<HttpResponseMessage> PostAsMessagePackAsync<T>(this HttpClient client, Uri? requestUri, T value, MessagePackSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            return client.SendAsMessagePackAsyncCore(request, value, options, cancellationToken);
        }

        public static Task<HttpResponseMessage> PostAsMessagePackAsync<T>(this HttpClient client, string? requestUri, T value, CancellationToken cancellationToken = default)
            => client.PostAsMessagePackAsync(requestUri, value, CustomFormatter, cancellationToken);

        public static Task<HttpResponseMessage> PostAsMessagePackAsync<T>(this HttpClient client, Uri? requestUri, T value, CancellationToken cancellationToken = default)
            => client.PostAsMessagePackAsync(requestUri, value, CustomFormatter, cancellationToken);        
        #endregion Post
        
        #region PostRead
        public static Task<T?> PostReadAsMessagePackAsync<T>(this HttpClient client, string? requestUri, T value, MessagePackSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            return client.SendReadAsMessagePackAsyncCore(request, value, options, cancellationToken);
        }

        public static Task<T?> PostReadAsMessagePackAsync<T>(this HttpClient client, Uri? requestUri, T value, MessagePackSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            return client.SendReadAsMessagePackAsyncCore(request, value, options, cancellationToken);
        }

        public static Task<T?> PostReadAsMessagePackAsync<T>(this HttpClient client, string? requestUri, T value, CancellationToken cancellationToken = default)
            => client.PostReadAsMessagePackAsync(requestUri, value, CustomFormatter, cancellationToken);

        public static Task<T?> PostReadAsMessagePackAsync<T>(this HttpClient client, Uri? requestUri, T value, CancellationToken cancellationToken = default)
            => client.PostReadAsMessagePackAsync(requestUri, value, CustomFormatter, cancellationToken);        
        #endregion PostRead
        
        #region Put
        public static Task<HttpResponseMessage> PutAsMessagePackAsync<T>(this HttpClient client, string? requestUri, T value, MessagePackSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var request = new HttpRequestMessage(HttpMethod.Put, requestUri);
            return client.SendAsMessagePackAsyncCore(request, value, options, cancellationToken);
        }

        public static Task<HttpResponseMessage> PutAsMessagePackAsync<T>(this HttpClient client, Uri? requestUri, T value, MessagePackSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var request = new HttpRequestMessage(HttpMethod.Put, requestUri);
            return client.SendAsMessagePackAsyncCore(request, value, options, cancellationToken);
        }

        public static Task<HttpResponseMessage> PutAsMessagePackAsync<T>(this HttpClient client, string? requestUri, T value, CancellationToken cancellationToken = default)
            => client.PutAsMessagePackAsync(requestUri, value, CustomFormatter, cancellationToken);

        public static Task<HttpResponseMessage> PutAsMessagePackAsync<T>(this HttpClient client, Uri? requestUri, T value, CancellationToken cancellationToken = default)
            => client.PutAsMessagePackAsync(requestUri, value, CustomFormatter, cancellationToken);        
        #endregion Put
        
        #region PutRead
        public static Task<T?> PutReadAsMessagePackAsync<T>(this HttpClient client, string? requestUri, T value, MessagePackSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var request = new HttpRequestMessage(HttpMethod.Put, requestUri);
            return client.SendReadAsMessagePackAsyncCore(request, value, options, cancellationToken);
        }

        public static Task<T?> PutReadAsMessagePackAsync<T>(this HttpClient client, Uri? requestUri, T value, MessagePackSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var request = new HttpRequestMessage(HttpMethod.Put, requestUri);
            return client.SendReadAsMessagePackAsyncCore(request, value, options, cancellationToken);
        }

        public static Task<T?> PutReadAsMessagePackAsync<T>(this HttpClient client, string? requestUri, T value, CancellationToken cancellationToken = default)
            => client.PutReadAsMessagePackAsync(requestUri, value, CustomFormatter, cancellationToken);

        public static Task<T?> PutReadAsMessagePackAsync<T>(this HttpClient client, Uri? requestUri, T value, CancellationToken cancellationToken = default)
            => client.PutReadAsMessagePackAsync(requestUri, value, CustomFormatter, cancellationToken);        
        #endregion PutRead
        
        #region Read
        public static Task<T?> ReadFromMessagePackAsync<T>(this HttpContent content, MessagePackSerializerOptions? options, CancellationToken cancellationToken = default)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            return content.ReadFromMessagePackAsyncCore<T>(options, cancellationToken);
        }
        public static Task<T?> ReadFromMessagePackAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
        => content.ReadFromMessagePackAsync<T>(CustomFormatter, cancellationToken);

        private static async Task<T?> ReadFromMessagePackAsyncCore<T>(this HttpContent content, MessagePackSerializerOptions? options, CancellationToken cancellationToken = default)
        {
            var bytes = await content.ReadAsByteArrayAsync(cancellationToken);
            return MessagePackSerializer.Deserialize<T>(bytes, options);
        }
        #endregion Read
        
        #region Send
        private static Task<HttpResponseMessage> SendAsMessagePackAsyncCore<T>(this HttpClient client,  HttpRequestMessage request, T value, MessagePackSerializerOptions options, CancellationToken cancellationToken)
        {
            var buffer = MessagePackSerializer.Serialize(value, options, cancellationToken);
            var content = new ByteArrayContent(buffer);
            request.Content = content;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MessagePackMediaType));
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(MessagePackMediaType);
            return client.SendAsync(request, cancellationToken);
        }

        private static async Task<T?> SendReadAsMessagePackAsyncCore<T>(this HttpClient client,  HttpRequestMessage request, T value, MessagePackSerializerOptions options, CancellationToken cancellationToken)
        {
            var resultTask = await client.SendAsMessagePackAsyncCore(request, value, options, cancellationToken);
            return await resultTask.Content.ReadFromMessagePackAsync<T>(options, cancellationToken);
        }
        #endregion Send
    }
}
