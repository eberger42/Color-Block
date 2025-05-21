using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Player.Interfaces;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Assets.Scripts.Blocks.commands
{

    public class CommandManager
    {

        public bool IsExecuting { get; private set; } = false;

        readonly CommandInvoker _commandInvoker = new CommandInvoker();

        public async Task ExecuteCommands(List<ICommand> commands)
        {
            IsExecuting = true;
            await _commandInvoker.ExecuteCommannd(commands);
            IsExecuting = false;
        }

        public class CommandBuilder
        {
            private List<ICommand> _commands = new List<ICommand>();

            public CommandBuilder AddCommand<T>(IEntity entity, IConfigureCommand commandConfigurer) where T : BlockCommand
            {
                var command = BlockCommand.Create<T>(entity);
                commandConfigurer.Configure(command);
                _commands.Add(command);
                return this;
            }

            public List<ICommand> Build()
            {
                return _commands;
            }
        }

        private class CommandInvoker
        {
            public async Task ExecuteCommannd(List<ICommand> commands)
            {
                foreach (var command in commands)
                {
                    await command.Execute();
                }
            }
        }

    }

    public abstract class BlockCommand : ICommand
    {

        protected readonly IEntity _entity;
        protected BlockCommand(IEntity entity)
        {
            this._entity = entity;
        }

        public abstract Task Execute();

        public static T Create<T>(IEntity entity) where T : BlockCommand
        {
            return (T)System.Activator.CreateInstance(typeof(T), entity);
        }
    }

   
}