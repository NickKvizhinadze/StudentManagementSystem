using CSharpFunctionalExtensions;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.AppServices
{
    public class EnrollStudentCommand : ICommand
    {
        public EnrollStudentCommand(long id, string course, string grade)
        {
            Id = id;
            Course = course;
            Grade = grade;
        }

        public long Id { get; }
        public string Course { get; }
        public string Grade { get; }


        internal class EnrollStudentCommandHandler : ICommandHandler<EnrollStudentCommand>
        {
            private readonly UnitOfWork _unitOfWork;

            public EnrollStudentCommandHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(EnrollStudentCommand command)
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

                student.Enroll(course, grade);

                _unitOfWork.Commit();

                return Result.Ok();
            }
        }
    }
}
