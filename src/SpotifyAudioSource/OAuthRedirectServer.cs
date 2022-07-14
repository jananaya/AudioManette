using System.Net;

namespace AudioManette.Authorization
{
    public class OAuthRedirectServer
    {
        private int _port;
        private HttpListener _listener;
        private System.Uri _uri;
        private HttpListenerContext? _context;

        public OAuthRedirectServer(int port)
        {
            _port = port;
            _uri = new System.Uri($"http://localhost:{_port}/");
            
            _listener = new HttpListener();
            _listener.Prefixes.Add(_uri.AbsoluteUri);
        }

        public void Listen()
        {
            _listener.Start();
        }

        public string GetCode()
        {
            _context = _listener.GetContext();

            var query = _context.Request.QueryString;
            var code = query.Get("code");

            var response = _context.Response;

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Close();

            return code ?? "";
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public System.Uri Uri => _uri;
    }
}