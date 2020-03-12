using CSharpFunctionalExtensions;
using Logic.Students;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Decorators
{
    public class AuditLogDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        //private readonly Config _config;

        public AuditLogDecorator(ICommandHandler<TCommand> handler)
        {
            //_config = config;
            _handler = handler;
        }

        public Result Handle(TCommand command)
        {
            var json = JsonConvert.SerializeObject(command);
            Console.WriteLine($"Command of type {command.GetType()} : {json}");

            return _handler.Handle(command);
        }
    }
}
