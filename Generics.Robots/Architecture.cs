using System;
using System.Collections.Generic;

namespace Generics.Robots
{
    public interface IGetCommand<out T>
    {
        T GetCommand();
    }
	
    public abstract class RobotAI<T>
    {
        public abstract T GetCommand();
    }
	
    public class ShooterAI : IGetCommand<ShooterCommand>
    {
        int counter = 1;
        public ShooterCommand GetCommand()
        {
            return ShooterCommand.ForCounter(counter++);
        }
    }
	
    public class BuilderAI : IGetCommand<BuilderCommand>
    {
        int counter = 1;
        public BuilderCommand GetCommand()
        {
            return BuilderCommand.ForCounter(counter++);
        }
    }
	
    public interface IExecuteCommand<in T>
    {
        string ExecuteCommand(T command);
    }
	
    public abstract class Device<T>
    {
        public abstract string ExecuteCommand(T command);
    }
	
    public class Mover : IExecuteCommand<IMoveCommand>
    {
        public string ExecuteCommand(IMoveCommand command)
        {
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }
	
    public class Robot
    {
        readonly IGetCommand<IMoveCommand> ai;
        readonly Mover device;
        public Robot(IGetCommand<IMoveCommand> ai, Mover executor)
        {
            this.ai = ai;
            device = executor;
        }
		
        public IEnumerable<string> Start(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                var command = ai.GetCommand();
                if (command == null)
                    break;
                yield return device.ExecuteCommand(command);
            }
        }
		
        public static Robot Create(IGetCommand<IMoveCommand> ai, Mover executor)
        {
            return new Robot(ai, executor);
        }
    }
}
}
