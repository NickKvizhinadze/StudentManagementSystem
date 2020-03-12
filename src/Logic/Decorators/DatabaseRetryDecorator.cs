﻿using CSharpFunctionalExtensions;
using Logic.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Decorators
{
    public sealed class DatabaseRetryDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        //private readonly Config _config;

        public DatabaseRetryDecorator(ICommandHandler<TCommand> handler)
        {
            //_config = config;
            _handler = handler;
        }

        public Result Handle(TCommand command)
        {
            for (int i = 0; ; i++)
            {
                try
                {
                    Result result = _handler.Handle(command);
                    return result;
                }
                catch (Exception ex)
                {
                    if (i >= 3 || !IsDatabaseException(ex))
                        throw;
                }
            }
        }


        private bool IsDatabaseException(Exception ex)
        {
            string message = ex.InnerException?.Message;

            if (message == null)
                return false;

            return message.Contains("The connection is broken and recovery is not possible")
                || message.Contains("error occurred while establishing a connection");
        }
    }
}
