using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Player.Interfaces;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.Rendering;
using UnityEngine;

namespace Assets.Scripts.Blocks.commands
{

    public class CommandManager
    {

        public bool IsExecuting { get; private set; } = false;

        private List<ICommand> _bufferedCommand = new List<ICommand>();

        readonly CommandInvoker _commandInvoker = new CommandInvoker();

        private CancellationTokenSource _cts;


        public async Task ExecuteCommands(List<ICommand> commands)
        {

            _cts = new CancellationTokenSource();

            try
            {
                IsExecuting = true;
                await _commandInvoker.ExecuteCommannd(commands, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Optional: handle cancellation feedback
                Debug.Log("Command execution was cancelled.");
            }
            finally
            {
                IsExecuting = false;

                if (_cts != null)
                {
                    _cts.Dispose();
                    _cts = null;
                }
            }

        }

        public async Task ExecuteCommands(List<ICommand> commands, Func<bool> IsExecutingGetter, Action<bool> isExecutingSetter )
        {
            if(IsExecutingGetter())
                return;

            isExecutingSetter(true);
            await _commandInvoker.ExecuteCommannd(commands);
            isExecutingSetter(false);
        }

        public void Cancel()
        {
            _cts?.Cancel();
        }

        public class CommandBuilder
        {
            private List<ICommand> _commands = new List<ICommand>();

            public CommandBuilder AddCommand<T>(ITakeBlockCommand target, IConfigureCommand commandConfigurer) where T : BlockCommand
            {
                var command = BlockCommand.Create<T>(target);
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
            public async Task ExecuteCommannd(List<ICommand> commands, CancellationToken ct)
            {
                foreach (var command in commands)
                {
                    ct.ThrowIfCancellationRequested();
                    await command.Execute();
                }
            }
        }

    }

    public abstract class BlockCommand : ICommand
    {

        protected readonly ITakeBlockCommand _target;
        protected BlockCommand(ITakeBlockCommand target)
        {
            this._target = target;
        }

        public abstract Task Execute();

        public static T Create<T>(ITakeBlockCommand target) where T : BlockCommand
        {
            return (T)System.Activator.CreateInstance(typeof(T), target);
        }
    }

}
/*
using Assets.Scripts.Blocks.interfaces;
using Assets.Scripts.Player.Interfaces;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.Rendering;
using UnityEngine;

namespace Assets.Scripts.Blocks.commands
{

    public class CommandManager
    {

        public bool IsExecuting { get; private set; } = false;

        private List<ICommand> _bufferedCommand = new List<ICommand>();

        readonly CommandInvoker _commandInvoker = new CommandInvoker();

        private CancellationTokenSource _cts;

        public async Task ExecuteCommands(List<ICommand> commands)
        {
            Debug.Log($"Executing {commands.Count} commands");
            IsExecuting = true;
            await _commandInvoker.ExecuteCommannd(commands);
            IsExecuting = false;
        }

        public async Task ExecuteCommands(List<ICommand> commands, Func<bool> IsExecutingGetter, Action<bool> isExecutingSetter)
        {
            Debug.Log($"Executing {commands.Count}$$$$$$$$$$$$$$$$$$$$t");
            if (IsExecutingGetter())
                return;

            _cts = new CancellationTokenSource();
            isExecutingSetter(true);

            try
            {
                Debug.Log($"Executing {commands.Count} commands with cancellation support");
                await _commandInvoker.ExecuteCommannd(commands, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Optional: handle cancellation feedback
                Debug.Log("Command execution was cancelled.");
            }
            finally
            {
                Debug.Log("Command execution completed.");
                IsExecuting = false;
                _cts.Dispose();
                _cts = null;
            }
        }

        public void Cancel()
        {
            _cts?.Cancel();
        }

        public class CommandBuilder
        {
            private List<ICommand> _commands = new List<ICommand>();

            public CommandBuilder AddCommand<T>(ITakeBlockCommand target, IConfigureCommand commandConfigurer) where T : BlockCommand
            {
                var command = BlockCommand.Create<T>(target);
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
            public async Task ExecuteCommannd(List<ICommand> commands, CancellationToken ct)
            {
                foreach (var command in commands)
                {
                    ct.ThrowIfCancellationRequested();
                    await command.Execute();
                }
            }

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

        protected readonly ITakeBlockCommand _target;
        protected BlockCommand(ITakeBlockCommand target)
        {
            this._target = target;
        }

        public abstract Task Execute();

        public static T Create<T>(ITakeBlockCommand target) where T : BlockCommand
        {
            return (T)System.Activator.CreateInstance(typeof(T), target);
        }
    }

}*/