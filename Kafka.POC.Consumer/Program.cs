using Confluent.Kafka;
using System;
using System.Threading;

namespace Kafka.POC.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            // The Kafka endpoint address
            string kafkaEndpoint = "localhost:9092";

            // The Kafka topic we'll be using
            string kafkaTopic = "testtopic";


            // Create the consumer configuration
            var consumerConfig = new ConsumerConfig { GroupId = "myconsumer", BootstrapServers = kafkaEndpoint };



            // Create the consumer
            using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                c.Subscribe(kafkaTopic);

                CancellationTokenSource cts = new CancellationTokenSource();
                System.Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            System.Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            System.Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }

        }
    }
}
