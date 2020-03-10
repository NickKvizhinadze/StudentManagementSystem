using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Students
{
    public sealed class Messages
    {
        private readonly IServiceProvider _serviceProvider;

        public Messages(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Result Dispatch(ICommand command)
        {
            Type type = typeof(ICommandHandler<>);
            Type[] argTypes = { command.GetType() };

            Type handlerType = type.MakeGenericType(argTypes);

            dynamic handler = _serviceProvider.GetService(handlerType);
            return handler.Handle((dynamic)command);
        }

        public TResult Dispatch<TResult>(IQuery<TResult> command)
        {
            Type type = typeof(IQueryHandler<,>);
            Type[] argTypes = { command.GetType(), typeof(TResult) };

            Type handlerType = type.MakeGenericType(argTypes);

            dynamic handler = _serviceProvider.GetService(handlerType);
            return handler.Handle((dynamic)command);
        }
    }
}
