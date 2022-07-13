using Assets._Scripts.Classes;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace Assets._Scripts.Services.ApiService
{
    public interface IApiService
    {
        public bool UseMockServer { get; set; }
        public string ApiDomain { get; }
        public void SetApiEndpoint(string endpoint);
        public IEnumerator GetServerJson(string endpoint);
        public IEnumerator GetHttpResponse(string endpoint);
        public IEnumerator LoadAndGetImage(string imageUrl);
    }

    public class ApiService : IApiService
    {
        private string apiDomain_prod = "TODO:azureDomainUrl";
        public bool UseMockServer { get; set; } = false;
        private string apiDomain_mock = "http://ebac-176-76-242-251.ngrok.io";

        private Dictionary<string, string> ApiHeaders;

        public string ApiDomain => UseMockServer ? apiDomain_mock : apiDomain_prod;
        private string ApiKeyConfigDirectory => ApiDomain + "/configs";

        public ApiService()
        {
            ApiHeaders = new Dictionary<string, string>();
        }

        /// <summary>
        /// Call once, on app initialization
        /// </summary>
        /// <param name="endpoint"></param>
        public void SetApiEndpoint(string endpoint)
        {
            Debug.Log("Setting api endpoint domain : " + endpoint);
            apiDomain_prod = endpoint;
        }

        /// <summary>
        /// Set headers to pass when making a server request
        /// </summary>
        /// <param name="Headers"></param>
        public void SetApiHeaders(Dictionary<string, string> Headers)
        {
            ApiHeaders = Headers;
        }

        /// <summary>
        /// Get json from server
        /// </summary>
        /// <param name="endpoint">Full endpoint url</param>
        /// <returns>HttpResponse object</returns>
        public IEnumerator GetServerJson(string endpoint)
        {
            Debug.Log("GET Server Json " + endpoint);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(endpoint))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    yield return new HttpResponse(false, webRequest.downloadHandler.error, webRequest.error);
                    yield break;
                }

                var json = webRequest.downloadHandler.text;
                yield return new HttpResponse(json);
            }
        }

        /// <summary>
        /// Get config file based on 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetApiKeyConfig(string apiKey, bool isGetDefaultSettings)
        {
            var headerDictionary = new Dictionary<string, string>()
            {
                { "api-key", apiKey }
            };
            SetApiHeaders(headerDictionary);

            var endpoint = ApiKeyConfigDirectory;
            if(isGetDefaultSettings)
                endpoint += "/mockSettings.json";

            Debug.Log("GET ApiKey config Json " + endpoint);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(endpoint))
            {
                // Add headers
                foreach (var headerEntry in ApiHeaders)
                    webRequest.SetRequestHeader(headerEntry.Key, headerEntry.Value);

                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    yield return new HttpResponse(false, webRequest.downloadHandler.error, webRequest.error);
                    yield break;
                }

                var json = webRequest.downloadHandler.text;
                yield return new HttpResponse(json);
            }
        }

        /// <summary>
        /// Load image from url as texture
        /// </summary>
        public IEnumerator LoadAndGetImage(string imageUrl)
        {
            var cd = new CoroutineWithData(GlobalControl.Instance, GetTextureRequest(imageUrl));
            yield return cd.coroutine;
            var response = cd.result as HttpResponse;
            if(!response.IsSuccessful)
            {
                yield return new ApiResponse() { ErrorMessage = response.HttpError };
                yield break;
            }

            yield return new ApiResponse() { Data = response.Texture };
        }

        /// <summary>
        /// Load data from url as byte array
        /// </summary>
        public IEnumerator LoadAndGetByteData(string fileUrl)
        {
            var cd = new CoroutineWithData(GlobalControl.Instance, GetByteDataRequest(fileUrl));
            yield return cd.coroutine;
            var response = cd.result as HttpResponse;
            if (!response.IsSuccessful)
            {
                yield return new ApiResponse() { ErrorMessage = response.HttpError };
                yield break;
            }

            yield return new ApiResponse() { Data = response.RawData };
        }

        public IEnumerator GetHttpResponse(string endpoint)
        {
            while (!GlobalControl.Instance.IsUserSettingsInitialized)
                yield return null;
            int languageId = GlobalControl.Instance.UserSettings.LanguageId;
            if(!UseMockServer)
                endpoint += string.Format("&languageid={0}", languageId);

            var cd = new CoroutineWithData(GlobalControl.Instance, GetRequest(endpoint));
            yield return cd.coroutine;
            yield return cd.result;
        }

        private IEnumerator GetRequest(string endpoint)
        {
            Debug.Log("GET " + endpoint);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(endpoint))
            {
                // Add headers
                foreach (var headerEntry in ApiHeaders)
                    webRequest.SetRequestHeader(headerEntry.Key, headerEntry.Value);

                yield return webRequest.SendWebRequest();

                if(webRequest.result != UnityWebRequest.Result.Success)
                {
                    yield return new HttpResponse(false, webRequest.downloadHandler.error, webRequest.error);
                    yield break;
                }

                var json = webRequest.downloadHandler.text;
                yield return new HttpResponse(json);
            }
        }

        /// <summary>
        /// Requests Http image and returns HttpResponse
        /// </summary>
        /// <param name="url"></param>
        /// <returns>HttpResponse</returns>
        private IEnumerator GetTextureRequest(string url)
        {
            //Debug.Log("GET tex " + url);
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    yield return new HttpResponse(false, webRequest.downloadHandler.error, webRequest.error);
                    yield break;
                }

                var texture = DownloadHandlerTexture.GetContent(webRequest);
                yield return new HttpResponse(texture);
            }
        }


        /// <summary>
        /// Requests Http byte data and returns HttpResponse
        /// </summary>
        /// <param name="url"></param>
        /// <returns>HttpResponse</returns>
        private IEnumerator GetByteDataRequest(string url)
        {
            //Debug.Log("GET tex " + url);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    yield return new HttpResponse(false, webRequest.downloadHandler.error, webRequest.error);
                    yield break;
                }

                var data = webRequest.downloadHandler.data;
                yield return new HttpResponse(data);
            }
        }
    }
}
