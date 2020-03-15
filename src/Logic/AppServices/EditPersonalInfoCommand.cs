﻿using CSharpFunctionalExtensions;
using Logic.Students;
using Logic.Utils;

namespace Logic.AppServices
{
    public class EditPersonalInfoCommand : ICommand
    {
        public EditPersonalInfoCommand(long id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }

        public long Id { get; }
        public string Name { get; }
        public string Email { get; }

        public class EditPersonalInfoCommandHandler : ICommandHandler<EditPersonalInfoCommand>
        {
            private readonly SessionFactory _sessionFactory;

            public EditPersonalInfoCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory;
            }
            public Result Handle(EditPersonalInfoCommand command)
            {
                var unitOfWork = new UnitOfWork(_sessionFactory);
                var studentRepository = new StudentRepository(unitOfWork);
                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                student.Name = command.Name;
                student.Email = command.Email;

                unitOfWork.Commit();

                return Result.Ok();
            }
        }
    }
}
