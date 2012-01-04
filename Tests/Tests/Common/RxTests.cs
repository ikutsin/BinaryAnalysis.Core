using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using NUnit.Framework;

namespace BinaryAnalysis.Tests.Common
{
    [TestFixture]
    public class RxTests
    {
        static IEnumerable<string> EndlessBarrageOfEmail()
        {
            var random = new Random();
            var emails = new List<String> { "Here is an email!", "Another email!", "Yet another email!" };
            for (; ; )
            {
                // Return some random emails at random intervals.
                yield return emails[random.Next(emails.Count)];
                Thread.Sleep(random.Next(1000));
            }
        }

        [Test]
        public void RxBreakingTest()
        {
            var myInbox = EndlessBarrageOfEmail().ToObservable(
                System.Reactive.Concurrency.Scheduler.NewThread);

            // Instead of making you wait 5 minutes, we will just check every three seconds instead. :)
            var getMailEveryThreeSeconds = myInbox.Buffer(TimeSpan.FromSeconds(3)); //  Was .BufferWithTime(...

            getMailEveryThreeSeconds.Subscribe(emails =>
            {
                Debug.WriteLine("You've got {0} new messages!  Here they are!", emails.Count());
                foreach (var email in emails)
                {
                    Debug.WriteLine("> {0}", email);
                }
                Debug.WriteLine("");
            });

            Thread.Sleep(5000);        
        }
        //[Test]
        public void RxDelayTest()
        {
            var oneNumberEveryFiveSeconds = Observable.Interval(TimeSpan.FromSeconds(5));

            // Instant echo
            oneNumberEveryFiveSeconds.Subscribe(num =>
            {
                Debug.WriteLine(num);
            });

            // One second delay
            oneNumberEveryFiveSeconds.Delay(TimeSpan.FromSeconds(1)).Subscribe(num =>
            {
                Debug.WriteLine("...{0}...", num);
            });

            // Two second delay
            oneNumberEveryFiveSeconds.Delay(TimeSpan.FromSeconds(2)).Subscribe(num =>
            {
                Debug.WriteLine("......{0}......", num);
            });

            Thread.Sleep(12000);
        }
    }
}
