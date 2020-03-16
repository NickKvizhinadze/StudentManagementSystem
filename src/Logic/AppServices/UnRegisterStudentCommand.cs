using CSharpFunctionalExtensions;
using Logic.Students;
using Logic.Utils;

namespace Logic.AppServices
{

    public class UnRegisterStudentCommand : ICommand
    {
        public UnRegisterStudentCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }

        internal class UnRegisterStudentCommandHandler : ICommandHandler<UnRegisterStudentCommand>
        {
            private readonly UnitOfWork _unitOfWork;

            public UnRegisterStudentCommandHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(UnRegisterStudentCommand command)
            {
                var studentRepository = new StudentRepository(_unitOfWork);

                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                studentRepository.Delete(student);
                _unitOfWork.Commit();

                return Result.Ok();
            }
        }
    }
}
