// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://mrltcrpj:nG85peo0qknQoavrqwK7OJ3NOB9GTr4T@baboon.rmq.cloudamqp.com/mrltcrpj");


using var connection = factory.CreateConnection();

//rabbitMq'ya bir kanal uzerinden baglanilir.
var channel = connection.CreateModel();

//excluse tru yaparsak buradaki kuyruğa sadece olusturdugumuz channel uzerinden baglanabiliriz.Ama biz olusturdugumuz subscriber uzerinden baglanacagımız için false yapılmalıdır.
//outuDlete'nin true olması kuyruğa bağlı son subscriber down olunca queue'nin otomatik silinmesini ifade eder.Gerçek hayatta bu istenmez,o yüzden false yapılır,
channel.QueueDeclare("hello-queue", true, false, false);//durable false yaptık,true yapsaydık proje restart olsa bile belleğe fiziksel olarak kayıt olacagından mesaj kuruktan silinmezdi.Gerçekten dünyada true olması uygundur.

//Yukarıdaki kuyruğun varlığı hata olma olasılıgını azaltır.Yani publisher'da boyle bir kuyruk olmasa bile bunu olusturur ve akısında devametmesini saglar


//her bir subscriber'a kac mesaj gelecek bunu ayarlamak icin:
channel.BasicQos(0,1,false);//global parametresinin true olması tek bir seferde subscriberlara gonderilcek toplam queue sayısı doğrular:Yani 5 queue gonderilecekce bunun 2 Tanesi a sbuscribera gonderiri 3'sunu B subscriber'a gonderir.
//BasicQos(0,5,false) global'ı false olması tek seferde A subscriber ve B subscriber'lara 5 er queue gonderir.
var consumer = new EventingBasicConsumer(channel);

channel.BasicConsume("hello-queue",true,consumer);//Bir kuyruk ismi istiyor.Bir sonraki parametre autoAck mesaj ulastıktan sonra silinmesi isteniyorsa true isaretlenir.



consumer.Received += (object? sender, BasicDeliverEventArgs e)=>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Thread.Sleep(1500);

    Console.WriteLine("Gelen Mesaj:"+message);
    //DeliveryTag ile bana ulasılan şu taglı mesajı RabbitMQ'ya gonderiyorum,RabbitMq'da ilgili mesajı kuruktan siliyor.
    channel.BasicAck(e.DeliveryTag,false);//multiple parametresi true yapılırsa bunun gibi baska işlenmiş mesajlar var ise memory de onlarıda kuyruktan silmeye yarar.
};



Console.ReadKey();

