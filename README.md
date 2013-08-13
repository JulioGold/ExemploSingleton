ExemploSingleton
================

Quem nunca precisou definir uma classe que implementasse o padrão singleton? HA HA

Segue abaixo dois exemplos prontos para serem utilizados de conexão com o SQL Server.

Um é o padrão singleton thread safe e o outro é o padrão not thread safe, a diferença principal entre ambos é que para a utilização em ambientes multi-thread e onde o momento de instanciação da classe possa demorar um tempo considerável por ter de executar muitas tarefas o ideal é que seja utilizado o padrão thread safe, pois ele garante que haverá apenas uma instância da classe mesmo em um ambiente multi-thread, isto pode acarretar em um bloqueio do trecho de código que é o construtor da classe e um bloqueio para as demais requisições pela instância da classe até que a instanciação da mesma tenha sido realizada.

Já o padrão not thread safe não implementa este bloqueio na sua instanciação, o que pode causar a existência de duas instâncias de uma mesma classe quando em ambiente multi-thread.


Esta é uma aplicação que utiliza as classes singleton e também um caso onde não é utilizada nenhuma classe singleton
para demonstrar as diferenças.


Para verificar as conexões no banco você pode utilizar o seguinte script SQL, filtrando pela palavra Singleton no ProgramName.

DECLARE @tmpWho2 TABLE
(
    SPID         int
    ,Status      varchar(50)
    ,Login       varchar(255)
    ,HostName    varchar(255)
    ,BlkBy       varchar(50)
    ,DBName      varchar(255)
    ,Command     varchar(255)
    ,CPUTime     int
    ,DiskIO      int
    ,LastBatch   varchar(255)
    ,ProgramName varchar(255)
    ,SPID2       int
    ,REQUESTID   int
)
 
INSERT INTO @tmpWho2 EXEC sp_who2
 
SELECT * FROM @tmpWho2 WHERE ProgramName LIKE '%Singleton%'


abraço
