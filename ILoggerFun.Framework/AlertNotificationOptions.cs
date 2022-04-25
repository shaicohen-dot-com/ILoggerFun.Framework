using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ILoggerFun.Framework
{

    public class AlertNotification
    {
        private readonly ILogger<AlertNotification> _logger;
        private readonly IOptionsSnapshot<AlertNotificationOptions> _options;
        protected Timer Timer { get;}
        public AlertNotification(IOptionsSnapshot<AlertNotificationOptions> options, ILogger<AlertNotification> logger) =>
            (_logger, _options, Timer) = (logger, options, new Timer(options.Value.Interval));

        public void Start()
        {
            Timer.Interval = _options.Value.Interval;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        public void Slower()
        {
            Timer.Interval += _options.Value.Step;
        }

        public void Faster()
        {
            if (Timer.Interval - _options.Value.Step <= 0)
                Timer.Interval = 1;
            else
                Timer.Interval -= _options.Value.Step;
        }

        public void Flip()
        {
            Timer.Enabled = !Timer.Enabled;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _logger.LogInformation($"Elapsed {DateTime.Now}");
        }
    }

    public class AlertNotificationOptions
    {
        public bool Enabled { get; set; }
        public double Interval { get; set; }
        public double Step{ get; set; }
    }
}
