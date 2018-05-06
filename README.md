# ConfigReader
Simple Config Reader
Configlerin Sql server düzeyinde yönetilmesini saglayan ufak çapli bir proje.
ConfigurationApi DotnetCore projesi web arayüzünün özelliklerini sagliyor.
ConfigReader belirli araliklarla ilgili servisin kayitlarinin güncellenmesini sagliyor.
Pub/Sub yapisiyla api üzerinden degistirilen kayitlar dll tarafinda consume ediliyor.


Ef Core connectionstring localdb ye bakiyor.
RabbitMq localhost üzerinden ayaga kalkiyor.
