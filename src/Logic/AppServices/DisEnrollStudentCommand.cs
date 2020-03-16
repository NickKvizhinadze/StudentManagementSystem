using CSharpFunctionalExtensions;
using Logic.Students;
using Logic.Utils;

namespace Logic.AppServices
{
    public class DisEnrollStudentCommand : ICommand
    {
        public DisEnrollStudentCommand(long id, int enrollmentNumber, string comment)
        {
            Id = id;
            Comment = comment;
            EnrollmentNumber = enrollmentNumber;
        }

        public long Id { get; }
        public int EnrollmentNumber { get; set; }
        public string Comment { get; }


        internal class DisEnrollStudentCommandHandler : ICommandHandler<DisEnrollStudentCommand>
        {
            private readonly UnitOfWork _unitOfWork;

            public DisEnrollStudentCommandHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(DisEnrollStudentCommand command)
            {
                var courseRepository = new CourseRepository(_unitOfWork);
                var studentRepository = new StudentRepository(_unitOfWork);

                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                if (string.IsNullOrWhiteSpace(command.Comment))
                    return Result.Fail("Disenrollment comment is required");

                var enrollment = student.GetEnrollment(command.EnrollmentNumber);

                if (enrollment == null)
                    return Result.Fail($"No enrollment found with number: '{command.EnrollmentNumber}'");

                student.RemoveEnrollment(enrollment, command.Comment);

                _unitOfWork.Commit();

                return Result.Ok();
            }
        }
    }
}
