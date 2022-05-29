//To use RabbitMQ
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
//Default C# application
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//For multithreading using API..
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RPCClient
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                // *For connecting RabbitMQ
                Console.WriteLine("Starting Communication with RabbitMQ :");
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    Port = 5672,
                    UserName = "guest",
                    Password = "guest"
                };
                using (var connection = factory.CreateConnection())


                using (var channel = connection.CreateModel())
                {
                    // *.............................. declaring Queues ...................................................*
                    channel.QueueDeclare(queue: "GoGet.Queues.rpc",
                                                durable: false,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);
                    //*...............................Sending Message.to Queues.....................................................*

                    var methodCall = new Program();
                    //* This method is calling  API which has stored in IIS server in local machine...
                    string message = methodCall.DeviceNetworkConnectJob();
                    Console.WriteLine(message);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "",
                                            routingKey: "GoGet.Queues.rpc",
                                            basicProperties: null,
                                            body: body);

                    Console.WriteLine("Message send to RabbitMQ done.....");
                    //*.......................................Msg Delivering from Queues...........................................*

                    channel.BasicQos(0, 1, false);                                
                    //basic quality of service
                    QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);  
                    //Consume creation
                    channel.BasicConsume("GoGet.Queues.rpc", false, consumer);
                    BasicDeliverEventArgs deliveryArguments = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                    String message1 = Encoding.UTF8.GetString(deliveryArguments.Body);     
                    //Colleting message from Body(previous msg)
                    Console.WriteLine("Message received: {0}", message1);
                    channel.BasicAck(deliveryArguments.DeliveryTag, false);                 
                    String consumerTag = channel.BasicConsume("GoGet.Queues.rpc", false, consumer);   
                    //Consumer tag from RabbitMQ 
                    //*.................... Printing Tag..........................................................................*
                    Console.WriteLine("Consumer Tag from RabbitMQ ......   :" + consumerTag);
                    
                    connection.Close();
                   
                }
               
                Console.WriteLine("Press q to quit / Any other key to continue... ");
               // While looop to take choice from Users if they wish to continue or quit.
                string message2 = Console.ReadLine();
                if (message2.ToLower() == "q") break;
            }
        }

        private string DeviceNetworkConnectJob()
            {
                //*.................... Reading From API..........................................................................*
              
                Console.Write("Please input the deviceId Number you want to search :  DeviceNetworkConnectJob{"+"deviceId :"+"}");
              
                string deviceIds = Console.ReadLine();
                //DeviceId will be addeded in this Http which will trigger the method   public DeviceDetails Get(int id) from DeviceDetails controller 
                string listn = String.Format("Http://localhost:6142/api/deviceDetails" + "/" + deviceIds);
                WebRequest requestObj = WebRequest.Create(listn);
                //selecting type of Method from API
                requestObj.Method = "GET";
                HttpWebResponse response = null;
                response = (HttpWebResponse)requestObj.GetResponse();
                string strResult = null;
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    strResult = sr.ReadToEnd();
                    sr.Close();
                }
                return strResult;
         }
    }
}