using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Classes
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
}
