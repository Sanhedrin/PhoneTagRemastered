using Newtonsoft.Json;
using PhoneTag.SharedCodebase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PhoneTag.SharedCodebase
{
    public static class HttpExtensions
    {
        //public const String BaseUri = "http://localhost:64098/api/";
        public const String BaseUri = "http://phonetag.northeurope.cloudapp.azure.com/api/";

        /// <summary>
        /// Sends a POST request to the given http resource with an optional parameter.
        /// </summary>
        /// <param name="i_RequestUri">URI of the http resources(Must complete the BaseUri defined in the HttpExtensions class).</param>
        /// <param name="i_Content">An optional parameter to send to the method.</param>
        /// <returns>A response object if any such are returned by the method.
        /// This object is returned as type T.</returns>
        public static async Task<dynamic> PostMethodAsync<T>(this HttpClient i_HttpClient, string i_RequestUri, T i_Content)
        {
            //Serialize the input parameter and send the request.
            i_HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string jsonContent = JsonConvert.SerializeObject(i_Content, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            StringContent stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await i_HttpClient.PostAsync(new Uri(new Uri(BaseUri), i_RequestUri), stringContent).ConfigureAwait(false);

            //Obtain the result and deserialize it.
            string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION!");
                System.Diagnostics.Debug.WriteLine(jsonResponse);
                throw new HttpRequestException(jsonResponse);
            }

            return JsonConvert.DeserializeObject(jsonResponse, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        /// <summary>
        /// Sends a POST request to the given http resource.
        /// </summary>
        /// <param name="i_RequestUri">URI of the http resources(Must complete the BaseUri defined in the HttpExtensions class).</param>
        /// <returns>A response object if any such are returned by the method.
        /// This object is returned as a dynamic which translates to a JObject on client side.</returns>
        public static async Task<dynamic> PostMethodAsync(this HttpClient i_HttpClient, string i_RequestUri)
        {
            //Serialize the input parameter and send the request.
            HttpResponseMessage response = await i_HttpClient.PostAsync(new Uri(new Uri(BaseUri), i_RequestUri), new StringContent("")).ConfigureAwait(false);

            //Obtain the result and deserialize it.
            string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION!");
                System.Diagnostics.Debug.WriteLine(jsonResponse);
                throw new HttpRequestException(jsonResponse);
            }

            return JsonConvert.DeserializeObject(jsonResponse, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        /// <summary>
        /// Sends a GET request to the given http resource with an optional parameter.
        /// </summary>
        /// <param name="i_RequestUri">URI of the http resources(Must complete the BaseUri defined in the HttpExtensions class).</param>
        /// <returns>A response object if any such are returned by the method.
        /// This object is returned as a dynamic which translates to a JObject on client side.</returns>
        public static async Task<dynamic> GetMethodAsync(this HttpClient i_HttpClient, string i_RequestUri)
        {
            //Send the request.
            HttpResponseMessage response = await i_HttpClient.GetAsync(new Uri(new Uri(BaseUri), i_RequestUri)).ConfigureAwait(false);

            //Obtain the result and deserialize it.
            string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION!");
                System.Diagnostics.Debug.WriteLine(jsonResponse);
                throw new HttpRequestException(jsonResponse);
            }

            return JsonConvert.DeserializeObject(jsonResponse, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        /// <summary>
        /// Sends a GET request to the given http resource with an optional parameter.
        /// An overloading of GetMethodAsync that allows specification of the return type.
        /// </summary>
        /// <param name="i_RequestUri">URI of the http resources(Must complete the BaseUri defined in the HttpExtensions class).</param>
        /// <returns>A response object if any such are returned by the method.
        /// This object is returned as type T.</returns>
        public static async Task<T> GetMethodAsync<T>(this HttpClient i_HttpClient, string i_RequestUri)
        {
            //Send the request.
            HttpResponseMessage response = await i_HttpClient.GetAsync(new Uri(new Uri(BaseUri), i_RequestUri)).ConfigureAwait(false);

            //Obtain the result and deserialize it.
            string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION!");
                System.Diagnostics.Debug.WriteLine(jsonResponse);
                throw new HttpRequestException(jsonResponse);
            }

            return (T)JsonConvert.DeserializeObject(jsonResponse, typeof(T), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        /// <summary>
        /// Uploads the given image to imgur as an annonymous upload.
        /// </summary>
        /// <param name="i_ImageData">The byte array representing the image we want to upload.</param>
        /// <returns>The ID of the uploaded image, as should be used in a get request for this image.</returns>
        public static async Task<String> PostImgurImageAsync(this HttpClient i_HttpClient, byte[] i_ImageData)
        {
            String imageId = String.Empty;

            i_HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", Keys.ImgurClientId);

            using (HttpRequestMessage uploadRequest = new HttpRequestMessage(HttpMethod.Post, Keys.ImageHostingServiceUploadUrl))
            {
                MultipartFormDataContent uploadContent = new MultipartFormDataContent($"{DateTime.UtcNow.Ticks}")
                {
                    { new StringContent("file"), "type" },
                    { new ByteArrayContent(i_ImageData), "image" }
                };

                uploadRequest.Content = uploadContent;

                try
                {
                    HttpResponseMessage uploadResponse = await i_HttpClient.SendAsync(uploadRequest).ConfigureAwait(false);

                    //Obtain the result and deserialize it.
                    string jsonResponse = await uploadResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!uploadResponse.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine("EXCEPTION!");
                        System.Diagnostics.Debug.WriteLine(jsonResponse);
                        throw new HttpRequestException(jsonResponse);
                    }

                    dynamic uploadedImageObject = JsonConvert.DeserializeObject(jsonResponse, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

                    if(uploadedImageObject != null)
                    {
                        imageId = uploadedImageObject.data.id;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }

                return imageId;
            }
        }
    }
}
