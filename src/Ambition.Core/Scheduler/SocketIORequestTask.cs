using Ambition.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambition.Core.Scheduler
{
    public class SocketIORequestTask : BaseRequestTask, IRequestTask
    {
        #region Properties

        public virtual IDictionary<string, dynamic> Commands => new Dictionary<string, dynamic>();

        public virtual ISet<string> Events => new HashSet<string>();

        public new string Identity => Encrypt.Md5Encrypt($"{Uri},{string.Join(",", Commands.Select(command => command.Key))}");

        #endregion Properties

        #region Ctor

        public SocketIORequestTask(string url) : this(url, null, null)
        {
        }

        public SocketIORequestTask(string url, IDictionary<string, dynamic> commands, ISet<string> events)
            : base(url)
        {
            if (Uri.Scheme != "http" && Uri.Scheme != "https")
            {
                Status = RequestTaskStatus.Failed;
                return;
            }

            if (commands != null)
            {
                foreach (var command in commands)
                {
                    AddCommand(command.Key, command.Value);
                }
            }

            if (events != null)
            {
                foreach (var @event in events)
                {
                    AddEvent(@event);
                }
            }
        }

        #endregion Ctor

        #region Public Methods

        public virtual SocketIORequestTask AddCommand(string commandName, dynamic command)
        {
            if (!Commands.ContainsKey(commandName))
            {
                Commands.Add(commandName, command);
            }
            return this;
        }

        public virtual SocketIORequestTask AddEvent(string eventName)
        {
            if (!Events.Contains(eventName))
            {
                Events.Add(eventName);
            }
            return this;
        }

        #endregion Public Methods
    }
}