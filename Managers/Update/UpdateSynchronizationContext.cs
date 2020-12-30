//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Threading;

//namespace Divine.SDK.Managers.Update
//{
//    internal sealed class UpdateSynchronizationContext : SynchronizationContext
//    {
//        private readonly ConcurrentQueue<KeyValuePair<SendOrPostCallback, object>> Queues = new ConcurrentQueue<KeyValuePair<SendOrPostCallback, object>>();

//        public override void Post(SendOrPostCallback d, object state)
//        {
//            Queues.Enqueue(new KeyValuePair<SendOrPostCallback, object>(d, state));
//        }

//        public void ProcessCallbacks()
//        {
//            while (Queues.TryDequeue(out var result))
//            {
//                try
//                {
//                    result.Key(result.Value);
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine(e);
//                    //LogManager.Error(e);
//                }
//            }
//        }
//    }
//}