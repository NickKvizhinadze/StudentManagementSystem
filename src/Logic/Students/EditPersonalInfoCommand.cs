using CSharpFunctionalExtensions;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Students
{
    public interface ICommand
    {
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Result Handle(TCommand command);
    }

    public class EditPersonalInfoCommand : ICommand
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class EditPersonalInfoCommandHandler : ICommandHandler<EditPersonalInfoCommand>
    {
        private readonly UnitOfWork _unitOfWork;

        public EditPersonalInfoCommandHandler(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Result Handle(EditPersonalInfoCommand command)
        {
            var studentRepository = new StudentRepository(_unitOfWork);
            Student student = studentRepository.GetById(command.Id);
            if (student == null)
                return Result.Fail($"No student found for Id {command.Id}");

            student.Name = command.Name;
            student.Email = command.Email;

            _unitOfWork.Commit();

            return Result.Ok();
        }
    }
}
