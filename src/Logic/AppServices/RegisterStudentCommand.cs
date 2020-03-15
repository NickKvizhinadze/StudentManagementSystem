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
    public class RegisterStudentCommand : ICommand
    {
        public RegisterStudentCommand(string name, string email, string course1, string course1Grade, string course2, string course2Grade)
        {
            Name = name;
            Email = email;
            Course1 = course1;
            Course1Grade = course1Grade;
            Course2 = course2;
            Course2Grade = course2Grade;
        }

        public string Name { get; }
        public string Email { get; }
        public string Course1 { get; }
        public string Course1Grade { get; }
        public string Course2 { get; }
        public string Course2Grade { get; }

        public class RegisterStudentCommandHandler : ICommandHandler<RegisterStudentCommand>
        {
            private readonly UnitOfWork _unitOfWork;

            public RegisterStudentCommandHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(RegisterStudentCommand command)
            {
                var courseRepository = new CourseRepository(_unitOfWork);
                var studentRepository = new StudentRepository(_unitOfWork);

                var student = new Student(command.Name, command.Email);

                if (command.Course1 != null && command.Course1Grade != null)
                {
                    Course course = courseRepository.GetByName(command.Course1);
                    student.Enroll(course, Enum.Parse<Grade>(command.Course1Grade));
                }

                if (command.Course2 != null && command.Course2Grade != null)
                {
                    Course course = courseRepository.GetByName(command.Course2);
                    student.Enroll(course, Enum.Parse<Grade>(command.Course2Grade));
                }

                studentRepository.Save(student);
                _unitOfWork.Commit();

                return Result.Ok();
            }
        }
    }
}
