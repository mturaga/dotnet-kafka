using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Kafka.POC.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // The Kafka endpoint address
            string kafkaEndpoint = "localhost:9092";

            // The Kafka topic we'll be using
            string kafkaTopic = "testtopic";


            //// Create the consumer configuration
            //var consumerConfig = new ConsumerConfig { GroupId = "myconsumer", BootstrapServers = kafkaEndpoint };



            //// Create the consumer
            //using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            //{
            //    c.Subscribe(kafkaTopic);

            //    CancellationTokenSource cts = new CancellationTokenSource();
            //    System.Console.CancelKeyPress += (_, e) => {
            //        e.Cancel = true; // prevent the process from terminating.
            //        cts.Cancel();
            //    };

            //    try
            //    {
            //        while (true)
            //        {
            //            try
            //            {
            //                var cr = c.Consume(cts.Token);
            //                System.Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
            //            }
            //            catch (ConsumeException e)
            //            {
            //                System.Console.WriteLine($"Error occured: {e.Error.Reason}");
            //            }
            //        }
            //    }
            //    catch (OperationCanceledException)
            //    {
            //        // Ensure the consumer leaves the group cleanly and final offsets are committed.
            //        c.Close();
            //    }
            //}



            var conf = new ProducerConfig { BootstrapServers = "localhost:9092" };

            Action<DeliveryReport<Null, string>> handler = r =>
                System.Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");

            using (var p = new ProducerBuilder<Null, string>(conf).Build())
            {
                for (int i = 0; i < 100; ++i)
                {
                    p.Produce(kafkaTopic, new Message<Null, string> { Value = i.ToString() }, handler);
                }

                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }

            System.Console.ReadLine();
        }
    }
}
