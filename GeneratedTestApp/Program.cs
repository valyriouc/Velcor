using System.Net;

// ask: how can the ai get more accruate so nothing is missing 
// ask: how can the ai be trained to better adapt to the current framework 

public class Program
            {
                private static readonly IHttpListener listener = new HttpListener();
                private static readonly IHttpRequestProcessor processor = new HttpRequestProcessor();
                private static readonly IResponseBuilder responseBuilder = new ResponseBuilder();

                public static void Main(string[] args)
                {
                    listener.StartListening("http://localhost:8080/");
                    
                    while (true)
                    {
                        var request = listener.GetRequest();
                        var response = processor.Process(request);
                        
                        if (response != null)
                        {
                            responseBuilder.BuildResponse(request, response);
                        }
                    }
                }
            
        
            public interface IHttpListener
            {
                void StartListening(string url);
                Request GetRequest();
            }
        
            public class HttpListener : IHttpListener
            {
                private readonly HttpServer server;

                public HttpListener()
                {
                    server = new HttpServer();
                }

                public void StartListening(string url)
                {
                    server.Start(url);
                }

                public Request GetRequest()
                {
                    return server.GetRequest();
                }
            }

            public class Request
            {
                public string Method { get; set; }
                public Uri Url { get; set; }
                public Dictionary<string, string> Headers { get; set; }
                public byte[] Body { get; set; }

                public Request()
                {
                    Headers = new Dictionary<string, string>();
                }
            }
        

            public class Response
            {
                public int StatusCode { get; set; }
                public Dictionary<string, string> Headers { get; set; }
                public byte[] Body { get; set; }

                public Response()
                {
                    Headers = new Dictionary<string, string>();
                }
            }
        

            public interface IHttpRequestProcessor
            {
                bool Process(Request request, out Response response);
            }


            public class HttpRequestProcessor : IHttpRequestProcessor
            {
                public bool Process(Request request, out Response response)
                {
                    response = new Response();

                    switch (request.Method.ToUpperInvariant())
                    {
                        case "GET":
                            HandleGetRequest(request, response);
                            break;
                        case "POST":
                            HandlePostRequest(request, response);
                            break;
                        default:
                            response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                            break;
                    }

                    return true;
                }

                private void HandleGetRequest(Request request, Response response)
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Headers.Add("Content-Type", "text/plain");
                    response.Body = System.Text.Encoding.UTF8.GetBytes("Hello, this is a GET request.");
                }

                private void HandlePostRequest(Request request, Response response)
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Headers.Add("Content-Type", "application/json");
                    var responseBody = $"Received POST request with body: {System.Text.Encoding.UTF8.GetString(request.Body)}";
                    response.Body = System.Text.Encoding.UTF8.GetBytes(responseBody);
                }
            }


        public class ResponseBuilder : IResponseBuilder
            {
                public void BuildResponse(Request request, Response response)
                {
                    Console.WriteLine($"Building response for {request.Method} {request.Url}");

                    var statusCode = (HttpStatusCode)response.StatusCode;
                    var responseLine = $"HTTP/1.1 {(int)statusCode} {statusCode}\r\n";
                    var headers = string.Join("\r\n", response.Headers.Select(h => $"{h.Key}: {h.Value}"));
                    var responseBody = System.Text.Encoding.UTF8.GetString(response.Body);

                    var fullResponse = $"{responseLine}{headers}\r\n\r\n{responseBody}";

                    Console.WriteLine(fullResponse);
                }
            }
        }

public interface IResponseBuilder
{
    public void BuildResponse(Program.Request request, Program.Response response);
}
