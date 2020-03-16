using CSharpFunctionalExtensions;
using Logic.Students;
using Logic.Utils;
using System;

namespace Logic.AppServices
{
    public class TransferStudentCommand : ICommand
    {
        public TransferStudentCommand(long id, int enrollmentNumber, string course, string grade)
        {
            Id = id;
            Course = course;
            Grade = grade;
            EnrollmentNumber = enrollmentNumber;
        }

        public long Id { get; }
        public int EnrollmentNumber { get; }
        public string Course { get; }
        public string Grade { get; }

        internal class TransferStudentCommandHandler : ICommandHandler<TransferStudentCommand>
        {
            private readonly UnitOfWork _unitOfWork;

            public TransferStudentCommandHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(TransferStudentCommand command)
            {
                var courseRepository = new CourseRepository(_unitOfWork);
                var studentRepository = new StudentRepository(_unitOfWork);

                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                var course = courseRepository.GetByName(command.Course);
                if (course == null)
                    return Result.Fail($"Course is incorrect: '{command.Course}");

                if (!Enum.TryParse(command.Grade, out Grade grade))
                    return Result.Fail($"Grade is incorrect: '{command.Grade}'");

                var enrollment = student.GetEnrollment(command.EnrollmentNumber);

                if (enrollment == null)
                    return Result.Fail($"No enrollment found with number: '{command.EnrollmentNumber}'");

                enrollment.Update(course, grade);

                _unitOfWork.Commit();

                return Result.Ok();
            }
        }
    }
}
