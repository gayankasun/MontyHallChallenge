namespace API.Handlers
{
    //APIJsonMessageBody
    public class APIJsonMessageBody : APIGenericMessageBody
    {
        public string Description { get; set; }
        public bool IsSuccessful { get; set; }
        public APIJsonMessageBody()
        {
            this.IsSuccessful = true;
            this.Description = string.Empty;
        }
    }

    //APIJsonMessage
    public class APIJsonMessage : APIGenericMessage<APIJsonMessageBody>
    {
        public APIJsonMessage()
        {
            this.ContentType = HttpContentType.Json;
        }
    }
}
