/**
 * Shamelessly copied from https://d-fens.ch/2017/03/18/transient-error-handling-with-automatic-retries-in-c/
 * by George 
 * 
 * Copyright 2017 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace MODPizza.APIServices.d_fens
{
    public class Retry
    {
        public static RetryStrategyExponential ExponentialStrategy = new RetryStrategyExponential();

        public static RetryStrategyIncremental IncrementalStrategy = new RetryStrategyIncremental();

        public static RetryStrategyFixed FixedStrategy = new RetryStrategyFixed();

        public static TResult Invoke<TResult>
        (
            Func<RetryStrategyBase, TResult> func,
            Action<RetryStrategyBase, Exception> exceptionFilter = null,
            Func<RetryStrategyBase, TResult> defaultReturnValueFactory = null,
            RetryStrategyExponential strategy = null
        )
        {
            Contract.Requires(null != func);

            if (null == strategy)
            {
                strategy = ExponentialStrategy;
            }

            var waitTime = strategy.MinWaitTimeIntervalMilliseconds;
            var stopwatch = Stopwatch.StartNew();

            for (; ; )
            {
                try
                {
                    return func.Invoke(strategy);
                }
                catch (Exception ex)
                {
                    exceptionFilter?.Invoke(strategy, ex);
                }

                if (strategy.CurrentAttempt >= strategy.MaxAttempts)
                {
                    break;
                }

                // check if maximum time has already elapsed
                if (stopwatch.ElapsedMilliseconds >= strategy.MaxWaitTimeMilliseconds)
                {
                    break;
                }

                // wait for next retry
                System.Threading.Thread.Sleep(waitTime);

                // get next wait time interval
                waitTime = strategy.GetNextWaitTime(waitTime);

                strategy.NextAttempt();
            }

            return null != defaultReturnValueFactory
                ? defaultReturnValueFactory.Invoke(strategy)
                : default(TResult);
        }

        public static TResult Invoke<TResult>
        (
            Func<TResult> func,
            Action<Exception> exceptionFilter = null,
            Func<TResult> defaultReturnValueFactory = null,
            RetryStrategyExponential strategy = null
        )
        {
            Contract.Requires(null != func);

            if (null == strategy)
            {
                strategy = ExponentialStrategy;
            }

            var waitTime = strategy.MinWaitTimeIntervalMilliseconds;
            var stopwatch = Stopwatch.StartNew();

            for (; ; )
            {
                try
                {
                    return func.Invoke();
                }
                catch (Exception ex)
                {
                    exceptionFilter?.Invoke(ex);
                }

                if (strategy.CurrentAttempt >= strategy.MaxAttempts)
                {
                    break;
                }

                // check if maximum time has already elapsed
                if (stopwatch.ElapsedMilliseconds >= strategy.MaxWaitTimeMilliseconds)
                {
                    break;
                }

                // wait for next retry
                System.Threading.Thread.Sleep(waitTime);

                // get next wait time interval
                waitTime = strategy.GetNextWaitTime(waitTime);

                strategy.NextAttempt();
            }

            return null != defaultReturnValueFactory
                ? defaultReturnValueFactory.Invoke()
                : default(TResult);
        }
    }
}