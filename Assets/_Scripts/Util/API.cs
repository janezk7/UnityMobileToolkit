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

namespace Assets.Scripts.Util
{
    public class HttpResponse
    {
        public string JsonText;
        public Texture2D Texture;
        public byte[] RawData;
        public bool IsSuccessful;
        public string HttpError;

        public HttpResponse(string data)
        {
            JsonText = data;
            Texture = null;
            IsSuccessful = true;
            HttpError = null;
        }

        public HttpResponse(Texture2D data)
        {
            JsonText = null;
            Texture = data;
            IsSuccessful = true;
            HttpError = null;
        }

        public HttpResponse(byte[] data)
        {
            RawData = data;
            JsonText = null;
            Texture = null;
            IsSuccessful = true;
            HttpError = null;
        }

        public HttpResponse(bool isSuccessful, string data, string errorMessage = null)
        {
            JsonText = data;
            Texture = null;
            IsSuccessful = isSuccessful;
            HttpError = errorMessage;
        }

        public HttpResponse(bool isSuccessful, Texture2D data, string errorMessage = null)
        {
            JsonText = null;
            Texture = data;
            IsSuccessful = isSuccessful;
            HttpError = errorMessage;
        }
    }

    public class ApiResponse
    {
        public object Data;
        public string ErrorMessage;
        public bool Ok { get { return ErrorMessage == null; } }
    }

    public class API : MonoBehaviour
    {
        public static API Instance;

        public string apiDomain_prod = "TODO:azureDomainUrl";

        public bool UseMockServer;

        //public string apiDomain_mock = "http://localhost:8080";
        public string apiDomain_mock = "http://ebac-176-76-242-251.ngrok.io";
        public string ApiDomain => UseMockServer ? apiDomain_mock : apiDomain_prod;

        private Dictionary<string, string> ApiHeaders;

        private string ApiKeyConfigDirectory => ApiDomain + "/configs";
        private string FileDirectory => ApiDomain + "/files"; // Directory for files 

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
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
        public static IEnumerator GetServerJson(string endpoint)
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
        /// Sample Get request to API
        /// </summary>
        public IEnumerator GetCategoryList()
        {
            var endpoint = string.Format("{0}/{1}", FileDirectory, "categories");
            if (UseMockServer)
                endpoint = string.Format("{0}/categories_mock.json", FileDirectory);
            var cd = new CoroutineWithData(this, GetHttpResponse(endpoint));
            yield return cd.coroutine;
            var response = cd.result as HttpResponse;

            if (!response.IsSuccessful)
            {
                yield return new ApiResponse() { ErrorMessage = response.HttpError };
                yield break;
            }

            // Wrap content in a single parent json key if necessary
            //  var jsonObjectFix = string.Format("{{\"categories\": {0} }}", response.JsonText);

            // Deserialize and return 
            CategoryWrapper categories = JsonUtility.FromJson<CategoryWrapper>(response.JsonText);

            yield return new ApiResponse() { Data = categories.categories };
        }

        /// <summary>
        /// Load image from url as texture
        /// </summary>
        public IEnumerator LoadAndGetImage(string imageUrl)
        {
            var cd = new CoroutineWithData(this, GetTextureRequest(imageUrl));
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
            var cd = new CoroutineWithData(this, GetByteDataRequest(fileUrl));
            yield return cd.coroutine;
            var response = cd.result as HttpResponse;
            if (!response.IsSuccessful)
            {
                yield return new ApiResponse() { ErrorMessage = response.HttpError };
                yield break;
            }

            yield return new ApiResponse() { Data = response.RawData };
        }

        private IEnumerator GetHttpResponse(string endpoint)
        {
            while (!GlobalControl.Instance.IsUserSettingsInitialized)
                yield return null;
            int languageId = GlobalControl.Instance.UserSettings.LanguageId;
            if(!UseMockServer)
                endpoint += string.Format("&languageid={0}", languageId);

            var cd = new CoroutineWithData(this, GetRequest(endpoint));
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
