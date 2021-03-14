//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//using Divine.SDK.Managers.Log;

//namespace Divine.SDK.Managers.Update
//{
//    public static class UpdateManager
//    {
//        private static readonly List<UpdateHandler> Handlers = new List<UpdateHandler>();

//        private static readonly List<UpdateHandler> InvokeHandlers = new List<UpdateHandler>();

//        static UpdateManager()
//        {
//            Divine.UpdateManager.Update += OnUpdate;
//        }

//        private static void OnUpdate()
//        {
//            foreach (var handler in InvokeHandlers.ToArray())
//            {
//                try
//                {
//                    if (handler.Invoke())
//                    {
//                        InvokeHandlers.Remove(handler);
//                    }
//                }
//                catch (Exception e)
//                {
//                    InvokeHandlers.Remove(handler);
//                    LogManager.Error(e);
//                }
//            }

//            foreach (var handler in Handlers.ToArray())
//            {
//                try
//                {
//                    handler.Invoke();
//                }
//                catch (Exception e)
//                {
//                    LogManager.Error(e);
//                }
//            }
//        }

//        public static void BeginInvoke(Action callback)
//        {
//            Divine.UpdateManager.UpdateSynchronizationContext.Post(d => callback(), null);
//        }

//        public static void BeginInvoke(uint timeout, Action callback)
//        {
//            if (timeout == 0)
//            {
//                BeginInvoke(callback);
//            }
//            else
//            {
//                InvokeHandlers.Add(new UpdateHandler(callback, new TimeoutHandler(timeout, true)));
//            }
//        }

//        public static Task BeginInvokeAsync(Action callback)
//        {
//            return BeginInvokeAsync(0, callback);
//        }

//        public static Task BeginInvokeAsync(uint timeout, Action callback)
//        {
//            var taskCompletionSource = new TaskCompletionSource<object>();

//            BeginInvoke(timeout, () =>
//            {
//                try
//                {
//                    callback();

//                    taskCompletionSource.SetResult(null);
//                }
//                catch (Exception e)
//                {
//                    taskCompletionSource.SetException(e);
//                }
//            });

//            return taskCompletionSource.Task;
//        }

//        public static UpdateHandler Subscribe(Action callback)
//        {
//            return Subscribe(0, true, callback);
//        }

//        public static UpdateHandler Subscribe(uint timeout, Action callback)
//        {
//            return Subscribe(timeout, true, callback);
//        }

//        public static UpdateHandler Subscribe(bool isEnabled, Action callback)
//        {
//            return Subscribe(0, isEnabled, callback);
//        }

//        public static UpdateHandler Subscribe(uint timeout, bool isEnabled, Action callback)
//        {
//            return Subscribe(Handlers, timeout, isEnabled, callback);
//        }

//        private static UpdateHandler Subscribe(ICollection<UpdateHandler> handlers, uint timeout, bool isEnabled, Action callback)
//        {
//            var handler = handlers.FirstOrDefault(h => h.Callback == callback);
//            if (handler == null)
//            {
//                if (timeout == 0)
//                {
//                    handler = new UpdateHandler(callback, InvokeHandler.Default, isEnabled);
//                }
//                else
//                {
//                    handler = new UpdateHandler(callback, new TimeoutHandler(timeout), isEnabled);
//                }

//                handlers.Add(handler);
//            }

//            return handler;
//        }

//        public static void Unsubscribe(Action callback)
//        {
//            Unsubscribe(Handlers, callback);
//        }

//        public static void Unsubscribe(UpdateHandler handler)
//        {
//            Handlers.Remove(handler);
//        }

//        private static void Unsubscribe(ICollection<UpdateHandler> handlers, Action callback)
//        {
//            var handler = handlers.FirstOrDefault(h => h.Callback == callback);
//            if (handler == null)
//            {
//                return;
//            }

//            handlers.Remove(handler);
//        }
//    }
//}