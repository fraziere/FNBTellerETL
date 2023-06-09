using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Logging
{
    public static class EventManager
    {
        public delegate void ETLAppLogHandler(ETLAppLogEventArgs eventArgs);
        private static event ETLAppLogHandler OnETLAppLogEventError;
        private static event ETLAppLogHandler OnETLAppLogEventErrorAndWarn;
        private static event ETLAppLogHandler OnETLAppLogEventAll;

        public delegate void ETLJobLogHandler(ETLJobLogEventArgs eventArgs);
        private static event ETLJobLogHandler OnETLJobLogEvent;

        public static void SubscribeToAppLog(VerbosityEnum verbosityLevel, ETLAppLogHandler method)
        {
            //You cant return an event to subscribe to (because events aren't 1st class obj's in C#)
            //you have to let the subscriber pass up the delegate it wishes to subscribe to

            switch (verbosityLevel)
            {
                case VerbosityEnum.Error:
                    OnETLAppLogEventError += method;
                    break;
                case VerbosityEnum.ErrorAndWarn:
                    OnETLAppLogEventErrorAndWarn += method;
                    break;
                case VerbosityEnum.All:
                    OnETLAppLogEventAll += method;
                    break;
                default:
                    throw new ArgumentException("Invalid VerbosityLevel Type");
            }
        }

        public static void SubscribeToJobLog(ETLJobLogHandler method)
        {
            OnETLJobLogEvent += method;
        }


        internal static void LogETLApplicationEvent(ETLAppLogEventArgs eventArgs)
        {
            if (OnETLAppLogEventError != null)
            {
                var subscribers = OnETLAppLogEventError.GetInvocationList();

                for (int i = 0; i < subscribers.Count(); i++)
                {
                    //local var for thread safety
                    var method = (ETLAppLogHandler)subscribers[i];

                    if (eventArgs.Severity == SeverityEnum.Error)
                    {
                        //dont do async w/ console apps.  Log not guaranteed to be written unless you put in
                        //a bkground thread count (which isn't worth the pain)
                        //Task.Run(() => method(eventArgs))

                        method(eventArgs); //sync call
                    }
                }
            }

            if (OnETLAppLogEventErrorAndWarn != null)
            {
                var subscribers = OnETLAppLogEventErrorAndWarn.GetInvocationList();

                for (int i = 0; i < subscribers.Count(); i++)
                {
                    //local var for thread safety
                    var method = (ETLAppLogHandler)subscribers[i];

                    if (eventArgs.Severity == SeverityEnum.Error || eventArgs.Severity == SeverityEnum.Warning)
                    {
                        //dont do async w/ console apps.  Log not guaranteed to be written unless you put in
                        //a bkground thread count (which isn't worth the pain)
                        //Task.Run(() => method(eventArgs))

                        method(eventArgs); //sync call
                    }
                }
            }

            if (OnETLAppLogEventAll != null)
            {
                var subscribers = OnETLAppLogEventAll.GetInvocationList();

                for (int i = 0; i < subscribers.Count(); i++)
                {
                    //local var for thread safety
                    var method = (ETLAppLogHandler)subscribers[i];

                    //dont do async w/ console apps.  Log not guaranteed to be written unless you put in
                    //a bkground thread count (which isn't worth the pain)
                    //Task.Run(() => method(eventArgs))

                    method(eventArgs); //sync call
                }
            }
        }

        internal static void LogETLJobEvent(ETLJobLogEventArgs eventArgs)
        {
            if (OnETLJobLogEvent != null)
            {
                var subscribers = OnETLJobLogEvent.GetInvocationList();

                for (int i = 0; i < subscribers.Count(); i++)
                {
                    //local var for thread safety
                    var method = (ETLJobLogHandler)subscribers[i];

                    //dont do async w/ console apps.  Log not guaranteed to be written unless you put in
                    //a bkground thread count (which isn't worth the pain)
                    //Task.Run(() => method(eventArgs))

                    method(eventArgs); //sync call
                }
            }
        }
    }
}
