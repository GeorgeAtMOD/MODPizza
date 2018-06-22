/**
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

namespace MODPizza.APIServices.d_fens
{
    public abstract class RetryStrategyBase
    {
        public int MaxAttempts = int.MaxValue;

        public int CurrentAttempt = 1;

        public int MaxWaitTimeMilliseconds = 300 * 1000;

        public int MinWaitTimeIntervalMilliseconds = 200;

        public int MaxWaitTimeIntervalMillisecons = 20 * 1000;

        public object StateInfo;

        public int NextAttempt()
        {
            return ++CurrentAttempt;
        }

        public abstract int GetNextWaitTime(int currentWaitTime);

        /**
         * public Class1()
         * {
         * }
         */
    }
}