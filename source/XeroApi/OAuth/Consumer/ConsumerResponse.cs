﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using DevDefined.OAuth.Utility;

namespace DevDefined.OAuth.Consumer
{
    public interface IConsumerResponse
    {
        HttpStatusCode ResponseCode { get; }
        WebHeaderCollection Headers { get; }

        bool IsServerError { get; }
        bool IsClientError { get; }
        bool IsGoodResponse { get; }

        Stream Stream { get; }
        byte[] ByteArray { get; }
        string Content { get; }
        string ContentType { get; }
        int ContentLength { get; }

        XDocument ToXDocument();
        T DeSerialiseTo<T>();
        NameValueCollection ToBodyParameters();
    }

    public class ConsumerResponse : IConsumerResponse
    {
        private readonly MemoryStream _responseContentStream = new MemoryStream();

        public ConsumerResponse(HttpWebResponse webResponse)
        {
            webResponse.GetResponseStream().CopyTo(_responseContentStream);

            if (webResponse.Headers["Content-Type"] != string.Empty)
                ContentType = webResponse.Headers["Content-Type"];

            if (webResponse.Headers["Content-Length"] != string.Empty)
                ContentLength = int.Parse(webResponse.Headers["Content-Length"]);

            ContentEncoding = webResponse.ContentEncoding;
            ResponseCode = webResponse.StatusCode;
            Headers = webResponse.Headers;
        }

        public Byte[] ByteArray
        {
            get
            {
                return _responseContentStream.ToArray();
            }
        }

        public Stream Stream
        {
            get
            {
                return _responseContentStream;
            }
        }

        public string Content
        {
            get
            {
                return Stream.ReadToEnd();
            }
        }

        public string ContentEncoding
        {
            get; 
            private set;
        }

        public string ContentType
        {
            get; 
            private set;
        }

        public int ContentLength
        { 
            get; 
            private set;
        }

        public XDocument ToXDocument()
        {
            return XDocument.Parse(Content);
        }

        public T DeSerialiseTo<T>()
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StreamReader sr = new StreamReader(Stream))
            using (XmlReader xr = new XmlTextReader(sr))
            {
                return (T)serializer.Deserialize(xr);
            }
        }

        public NameValueCollection ToBodyParameters()
        {
            return HttpUtility.ParseQueryString(Content);
        }

        public HttpStatusCode ResponseCode
        {
            get; 
            set;
        }

        public WebHeaderCollection Headers
        {
            get; 
            set;
        }

        public bool IsServerError
        {
            get { return (int) ResponseCode >= 500; }
        }

        public bool IsClientError
        {
            get { return (int) ResponseCode >= 400 && (int) ResponseCode <= 499; }
        }

        public bool IsGoodResponse
        {
            get { return (int)ResponseCode >= 200 && (int)ResponseCode <= 299; }
        }
    }
}
