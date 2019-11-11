using Ambition.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambition.Scheduler
{
    public class WebSocketRequestTask : BaseRequestTask, IRequestTask
    {
        #region Properties

        public virtual IDictionary<string, dynamic> Commands => new Dictionary<string, dynamic>();

        public override string Identity => Encrypt.Md5Encrypt($"{Uri},{string.Join(",", Commands.Select(command => command.Key))}");

        #endregion Properties

        #region Ctor

        public WebSocketRequestTask(string url) : this(url, null)
        {
        }

        public WebSocketRequestTask(string url, IDictionary<string, dynamic> commands)
            : base(url)
        {
            if (Uri.Scheme != "ws" && Uri.Scheme != "wss")
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
        }

        #endregion Ctor

        #region Public Methods

        public virtual WebSocketRequestTask AddCommand(string commandName, dynamic command)
        {
            if (!Commands.ContainsKey(commandName))
            {
                Commands.Add(commandName, command);
            }
            return this;
        }

        #endregion Public Methods
    }
}