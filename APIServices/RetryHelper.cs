using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODPizza.APIServices
{
    public static class RetryHelper
    {
        //private static ILog logger = LogManger.GetLogger(); // use a logger or trace of your choice.

        public static void RetryOnException(int times, TimeSpan delay, Action operation)
        {
            var attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    operation();
                    break; // Success! Let's exit the loop!
                }
                catch (Exception ex)
                {
                    if (attempts == times)
                        throw;

                    Task.Delay(delay).Wait();
                }
            } while (true);
        }

        public static void RetryOnException(int times, int milliseconds, Action operation)
        {
            var attempts = 0;
            do
            {

                try
                {
                    attempts++;
                    operation();
                    break; // Success! Let's exit the loop!
                }
                catch (Exception ex)
                {
                    if (attempts == times)
                        throw;

                    int delayInMilliseconds = (attempts * milliseconds);
                    var delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                    Task.Delay(delay).Wait();
                }
            } while (true);
        }
    }
}