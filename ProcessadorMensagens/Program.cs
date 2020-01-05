using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.ServiceBus;

namespace ProcessadorMensagens
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");
            var configuration = builder.Build();

            var client = new QueueClient(
                configuration["AzureServiceBus:ConnectionString"],
                configuration["AzureServiceBus:Queue"],
                ReceiveMode.ReceiveAndDelete);
            try
            {
                client.RegisterMessageHandler(
                       async (message, token) =>
                       {
                           ProcessarCargaCotacoes(message);
                       },
                       new MessageHandlerOptions(
                           async (e) =>
                           {
                               Console.WriteLine("[Erro] " +
                                   e.Exception.GetType().FullName + " " +
                                   e.Exception.Message);
                           }
                       )
                );

                Console.ReadKey();
            }
            finally
            {
                client.CloseAsync().Wait();
            }

            Console.WriteLine("Encerrando o processamento de mensagens!");
        }

        private static void ProcessarCargaCotacoes(
            Message message)
        {
            var conteudo = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine(Environment.NewLine +
                "[Nova mensagem recebida] " + conteudo);
        }
    }
}