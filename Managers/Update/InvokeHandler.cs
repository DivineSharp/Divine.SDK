//using System;

//namespace Divine.SDK.Managers.Update
//{
//    public class InvokeHandler
//    {
//        private static InvokeHandler instance;

//        public static InvokeHandler Default
//        {
//            get
//            {
//                if (instance == null)
//                {
//                    instance = new InvokeHandler();
//                }

//                return instance;
//            }
//        }

//        public virtual bool Invoke(Action callback)
//        {
//            callback.Invoke();
//            return true;
//        }

//        public override string ToString()
//        {
//            return "Handler";
//        }
//    }
//}