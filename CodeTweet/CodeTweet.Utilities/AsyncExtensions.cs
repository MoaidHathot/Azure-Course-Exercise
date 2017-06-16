using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeTweet.Utilities
{
    public static class AsyncExtensions
    {
        public static Task WithTimeout(this Task task, TimeSpan timeout) 
            => Task.WhenAny(task, Task.Delay(timeout));
    }
}
