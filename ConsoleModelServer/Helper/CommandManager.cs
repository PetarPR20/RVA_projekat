using ConsoleModelServer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleModelServer.Helper
{
	public class CommandManager
	{
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoStack = new Stack<ICommand>();
        //dodato
        private readonly ILogger logger;

        public CommandManager(ILogger logger)
        {
            this.logger = logger;
        }

        public void ExecuteCommand(ICommand command)
		{
            command.Execute();
            undoStack.Push(command);
            redoStack.Clear(); // Novi korak briše redo istoriju
            logger?.Log($"[CommandManager] Executed command: {command.GetType().Name}");
        }

		public void Undo()
		{
            if (undoStack.Count == 0)
            {
                logger?.Log("[CommandManager] Undo stack is empty. Nothing to undo.");
                return;
            }

            var command = undoStack.Pop();
            command.Unexecute();
            redoStack.Push(command);
            logger?.Log($"[CommandManager] Undone command: {command.GetType().Name}");
        }

		public void Redo()
		{
            if (redoStack.Count == 0)
            {
                logger?.Log("[CommandManager] Redo stack is empty. Nothing to redo.");
                return;
            }

            var command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
            logger?.Log($"[CommandManager] Redone command: {command.GetType().Name}");
        }
	}
}
