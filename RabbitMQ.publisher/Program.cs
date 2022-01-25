// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://mrltcrpj:nG85peo0qknQoavrqwK7OJ3NOB9GTr4T@baboon.rmq.cloudamqp.com/mrltcrpj");


using var connection = factory.CreateConnection();

//rabbitMq'ya bir kanal uzerinden baglanilir.
var channel=connection.CreateModel();

//excluse tru yaparsak buradaki kuyruğa sadece olusturdugumuz channel uzerinden baglanabiliriz.Ama biz olusturdugumuz subscriber uzerinden baglanacagımız için false yapılmalıdır.
//outuDlete'nin true olması kuyruğa bağlı son subscriber down olunca queue'nin otomatik silinmesini ifade eder.Gerçek hayatta bu istenmez,o yüzden false yapılır,
channel.QueueDeclare("hello-queue",true,false,false);//durable false yaptık,true yapsaydık proje restart olsa bile belleğe fiziksel olarak kayıt olacagından mesaj kuruktan silinmezdi.Gerçekten dünyada true olması uygundur.

Enumerable.Range(1, 50).ToList().ForEach(x =>
 {
     string message = $"Message {x}";

     //mesaj byte cevriliyor.Turkce karakterlerde sorun yasamamak icin
     var messageBody = Encoding.UTF8.GetBytes(message);
     channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);
     Console.WriteLine($"Message Gonderilmiştir: {message}");

 });






Console.ReadKey();