using Ambition.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambition.Scheduler
{
    public class WebSocketRequestTask : BaseRequestTask, IRequestTask
    {
        #region Properties

        public int HeartBeatInterval = 5;

        public IDictionary<string, string> Commands { get; private set; } = new Dictionary<string, string>();

        public IDictionary<string, string> HeartBeatCommands { get; private set; } = new Dictionary<string, string>();

        public override string Identity => Encrypt.Md5Encrypt($"{Uri},{string.Join(",", Commands.Select(command => command.Key))}");

        #endregion Properties

        #region Ctor

        public WebSocketRequestTask(string url)
            : this(url, null)
        {
        }

        public WebSocketRequestTask(string url, IDictionary<string, string> commands)
            : this(url, commands, null)
        {
        }

        public WebSocketRequestTask(string url, IDictionary<string, string> commands, IDictionary<string, string> heartBeatCommands)
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

            if (heartBeatCommands != null)
            {
                foreach (var command in heartBeatCommands)
                {
                    AddHeartBeatCommand(command.Key, command.Value);
                }
            }
        }

        #endregion Ctor

        #region Public Methods

        public virtual WebSocketRequestTask AddCommand(string commandName, string command)
        {
            if (!Commands.ContainsKey(commandName))
            {
                Commands.Add(commandName, command);
            }
            return this;
        }

        public virtual WebSocketRequestTask AddHeartBeatCommand(string commandName, string command)
        {
            if (!HeartBeatCommands.ContainsKey(commandName))
            {
                HeartBeatCommands.Add(commandName, command);
            }
            return this;
        }

        #endregion Public Methods
    }
}