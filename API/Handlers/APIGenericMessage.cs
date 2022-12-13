using System.Net;

namespace API.Handlers
{
    //HttpContentType
    public enum HttpContentType : byte
    {
        Json
    }

    //APIGenericMessageBody
    public abstract class APIGenericMessageBody
    {
        public object Content { get; set; }
        public APIGenericMessageBody()
        {
            this.Content = new object();
        }
    }

    //APIGenericMessage
    public abstract class APIGenericMessage<T> where T : class, new()
    {
        public HttpContentType ContentType { get; protected set; }
        public HttpStatusCode StatusCode { get; set; }
        public T MessageBody { get; set; }

        public APIGenericMessage()
        {
            this.MessageBody = new T();

            this.StatusCode = HttpStatusCode.OK;
        }
    }
}
