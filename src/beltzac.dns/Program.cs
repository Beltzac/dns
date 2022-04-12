using DNS.Client;
using DNS.Client.RequestResolver;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Server;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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


            //var pong = await db.PingAsync();
            //Console.WriteLine(pong);


            // Proxy to google's DNS
            //MasterFile masterFile = new MasterFile();
            //CacheResolver cacheResolver = new CacheResolver(new UdpRequestResolver(new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53) ));
          var dnss = new List<string>();
             dnss.Add("8.8.8.8");
             dnss.Add("8.8.4.4");
             dnss.Add("9.9.9.9");
             dnss.Add("149.112.112.112");
             dnss.Add("208.67.222.222");
             dnss.Add("208.67.220.220");
             dnss.Add("1.1.1.1");
             dnss.Add("1.0.0.1");
             dnss.Add("185.228.168.9");
             dnss.Add("185.228.169.9");
             dnss.Add("76.76.19.19");
             dnss.Add("76.223.122.150");
             dnss.Add("94.140.14.14");
             dnss.Add("94.140.15.15");

            var resolvers = new List<IRequestResolver>();

            foreach(string d in dnss)
            {
                resolvers.Add(new LogResolver(new IPEndPoint(IPAddress.Parse(d), 53)));
            }

            FastestResolver fastestResolver = new FastestResolver(resolvers.ToArray());

            DnsServer server = new DnsServer(new CacheResolver(fastestResolver));

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
                
                Console.WriteLine("{0} => {1}", e.Request.Questions.FirstOrDefault()?.Name, (e.Response?.AnswerRecords.FirstOrDefault(a => a is IPAddressResourceRecord) as IPAddressResourceRecord)?.IPAddress);
            };
            // Log errors
            server.Errored += (sender, e) => Console.WriteLine(e.Exception.Message);

            // Start the server (by default it listens on port 53)
            //await server.Listen(53553);
            await server.Listen();

           // Console.WriteLine("Hello World!");
        }

        class IPAddressConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(IPAddress));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return IPAddress.Parse((string)reader.Value);
            }
        }

        class IPEndPointConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(IPEndPoint));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                IPEndPoint ep = (IPEndPoint)value;
                JObject jo = new JObject();
                jo.Add("Address", JToken.FromObject(ep.Address, serializer));
                jo.Add("Port", ep.Port);
                jo.WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
                int port = (int)jo["Port"];
                return new IPEndPoint(address, port);
            }
        }

        public class CacheResolver : IRequestResolver
        {
            static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = { "localhost:6379" }
            });

            JsonSerializerSettings settings;

            private IDatabase db;

            private IRequestResolver _inner;

            private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

            public CacheResolver(IRequestResolver inner)
            {
                db = redis.GetDatabase();
                _inner = inner;

                settings = new JsonSerializerSettings();
                settings.Converters.Add(new IPAddressConverter());
                settings.Converters.Add(new IPEndPointConverter());
                settings.Formatting = Formatting.Indented;
            }



            public async Task<IResponse> Resolve(IRequest request, CancellationToken cancellationToken = default)
            {





                if (request.Questions.Count == 1)
                {







                    var q = request.Questions.First();





                    //CurrentDateTime = DateTime.Now;

                    //if (!)
                    //{
                    //    cacheValue = CurrentDateTime;


                    //    _memoryCache.Set(CacheKeys.Entry, cacheValue);
                    //}

                    //CacheCurrentDateTime = cacheValue;

                    var chave = q.Name.ToString() + "_" + q.Type;

                    //var cached = _memoryCache.Get<ClientResponse>(chave);

                    //if (cached != null)
                    //{
                    //    Console.WriteLine("Cache HIT! => " + q.Name.ToString());
                    //    return cached;
                    //}
                    
                    
                    //var cache = db.StringGet(q.Name.ToString());
                    //if (cache.HasValue)
                    //{
                    //    Console.WriteLine("Cache HIT! => " + q.Name.ToString());
                    //    //return JsonConvert.DeserializeObject<Response>(cache, settings);
                    //}


                    var result = await _inner.Resolve(request, cancellationToken);


                    //var r = result.AnswerRecords;

                    _memoryCache.Set(chave, result);

                    // db.StringSet(q.Name.ToString(), JsonConvert.SerializeObject(result, settings));
                        //Console.WriteLine(q.Name.ToString());
                        //Console.WriteLine(r.ToString());
              



                    //foreach (var q in result.)
                    //{
                    //    string jsonString = JsonConvert.SerializeObject(q, Formatting.Indented);
                    //    Console.WriteLine(jsonString);
                    //}

                    //foreach (var q in request.)
                    //{
                    //string jsonString = JsonConvert.SerializeObject(r, Formatting.Indented);
                    //db.StringSet(q.Name.ToString(), r.ToString());
                    //Console.WriteLine(q.Name.ToString());
                    //Console.WriteLine(r.ToString());
                    //}

                    return result;




                }
                else
                {
                    return await _inner.Resolve(request, cancellationToken);
                }
            }
        }

        public class FastestResolver : IRequestResolver
        {


            private IRequestResolver[] _inner;

            public FastestResolver(params IRequestResolver[] inner)
            {
                _inner = inner;
            }

            public async Task<IResponse> Resolve(IRequest request, CancellationToken cancellationToken = default)
            {
                Stopwatch sw = Stopwatch.StartNew();

                IResponse fastest = null;

                var cancelToken = new CancellationTokenSource();


                try
                {
                    fastest = await await Task.WhenAny(_inner.Select(r => r.Resolve(request, cancelToken.Token)));

                    cancelToken.Cancel();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                
                sw.Stop();
                Console.WriteLine("FastestResolver " + request.Questions.First().Name + " => " + sw.ElapsedMilliseconds);

                return  fastest;
            }
        }

        public class LogResolver : IRequestResolver
        {
            IRequestResolver res;
            string dns;
            public LogResolver(IPEndPoint inner)
            {
                res = new UdpRequestResolver(inner);
                dns = inner.ToString();
            }

            public async Task<IResponse> Resolve(IRequest request, CancellationToken cancellationToken = default)
            {
                
                Stopwatch sw = Stopwatch.StartNew();

                IResponse response = await res.Resolve(request);
         

                sw.Stop();
                
                Console.WriteLine("LogResolver @" + dns + " => " + request.Questions.First().Name + " => " + sw.ElapsedMilliseconds);


                return response;
            }
        }
    }
}
