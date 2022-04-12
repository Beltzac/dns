using DNS.Client;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace beltzac.dns
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //dig -p 53553 @DESKTOP-2E32IJH.local A www.beltzac.com.br

            //// Bind to a Domain Name Server
            //DnsClient client = new DnsClient("8.8.8.8");

            //// Create request bound to 8.8.8.8
            //ClientRequest request = client.Create();

            //// Returns a list of IPs
            //IList<IPAddress> ips1 = await client.Lookup("www.beltzac.com.br");


            //IList<IPAddress> ips = await client.Lookup("www.google.com");


            //Request request = new Request();

            //request.RecursionDesired = true;
            //request.Id = 123;
            //request.Questions.Add(new Question(new Domain("beltzac.com.br")));

            //UdpClient udp = new UdpClient();
            //IPEndPoint google = new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53);

            //// Send to google's DNS server
            //await udp.SendAsync(request.ToArray(), request.Size, google);

            //UdpReceiveResult result = await udp.ReceiveAsync();
            //byte[] buffer = result.Buffer;
            //Response response = Response.FromArray(buffer);

            //// Outputs a human readable representation
            //Console.WriteLine(response);




            // Proxy to google's DNS
            MasterFile masterFile = new MasterFile();
            DnsServer server = new DnsServer(masterFile, "8.8.8.8");

            //// Resolve these domain to localhost
            //masterFile.AddIPAddressResourceRecord("google.com", "127.0.0.1");
            //masterFile.AddIPAddressResourceRecord("github.com", "127.0.0.1");

            // Log every request
            //server.Requested += (sender, e) =>
            //{
            //    Console.WriteLine(e.Request.Questions.First().Name);
            //};
            // On every successful request log the request and the response
            server.Responded += (sender, e) =>
            {
                Console.WriteLine("{0} => {1}", e.Request.Questions.First().Name, (e.Response.AnswerRecords.First(a => a is IPAddressResourceRecord) as IPAddressResourceRecord).IPAddress);
            };
            // Log errors
            server.Errored += (sender, e) => Console.WriteLine(e.Exception.Message);

            // Start the server (by default it listens on port 53)
            await server.Listen(53553);

           // Console.WriteLine("Hello World!");
        }
    }
}
