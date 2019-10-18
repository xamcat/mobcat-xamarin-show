using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AsyncAwait.Services
{
    public class DataflowService
    {
        public async Task<IList<int>> ProcessDataUsingDataFlow(IEnumerable<int> data)
        {
            var dataflowBlockOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 100 };
            var inputBuffer = new TransformBlock<int, int>(async input =>
             {
                 return await SimulateDeserialization(input);
             }, dataflowBlockOptions);

            var deserializationBlock = new TransformBlock<int, int>(async input =>
            {
                return await SimulateDeserialization(input);
            }, dataflowBlockOptions);

            var decryptionBlock = new TransformBlock<int, int>(async input =>
            {
                return await SimulateDecryption(input);
            }, dataflowBlockOptions);

            var outputBufferBlock = new BufferBlock<int>();

            //var printAction = new ActionBlock<int>(input =>
            //{
            //    Console.WriteLine(input);
            //}, dataflowBlockOptions);

            var dataflowLinkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            inputBuffer.LinkTo(deserializationBlock, dataflowLinkOptions);
            deserializationBlock.LinkTo(decryptionBlock, dataflowLinkOptions);
            decryptionBlock.LinkTo(outputBufferBlock, dataflowLinkOptions);

            //Parallel.ForEach(data, (datum) => inputBuffer.SendAsync(datum));
            foreach (var datum in data)
            {
                inputBuffer.Post(datum);
            }

            //await Task.WhenAll(tasks);

            inputBuffer.Complete();

            deserializationBlock.Completion.Wait();

            IList<int> results;
            var success = outputBufferBlock.TryReceiveAll(out results);
            return results;
        }

        private async Task<int> SimulateDownload(int input)
        {
            await Task.Delay((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
            return input;
        }

        private async Task<int> SimulateDeserialization(int input)
        {
            //Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
            await Task.Delay((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
            return input;
        }
        private async Task<int> SimulateDecryption(int input)
        {
            //Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
            //return Task.FromResult(input);

            await Task.Delay((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
            return input;
        }

        public async Task<IList<int>> ProcessDataWithoutDataFlow(IEnumerable<int> data)
        {
            var tasks = data.Select(a => Task.Run(async () =>
            {
                var deserializedDatum = await SimulateDeserialization(a);
                var result = await SimulateDecryption(deserializedDatum);
                return result;
            }));

            var results = await Task.WhenAll(tasks);
            return results;
        }
    }
}
