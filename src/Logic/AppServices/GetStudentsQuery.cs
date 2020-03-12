using Logic.Dtos;
using Logic.Students;
using Logic.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Logic.AppServices
{
    public class GetStudentsQuery : IQuery<List<StudentDto>>
    {
        public GetStudentsQuery(string enrolled, int? number)
        {
            Enrolledin = enrolled;
            NumberOfCourses = number;
        }

        public string Enrolledin { get; }
        public int? NumberOfCourses { get; }

        internal class GetStudentsQueryHandler : IQueryHandler<GetStudentsQuery, List<StudentDto>>
        {
            private readonly UnitOfWork _unitOfWork;

            public GetStudentsQueryHandler(UnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public List<StudentDto> Handle(GetStudentsQuery query)
            {
                var studentRepository = new StudentRepository(_unitOfWork);
                IReadOnlyList<Student> students = studentRepository.GetList(query.Enrolledin, query.NumberOfCourses);
                return students.Select(x => ConvertToDto(x)).ToList();
            }

            private StudentDto ConvertToDto(Student student)
            {
                return new StudentDto
                {
                    Id = student.Id,
                    Name = student.Name,
                    Email = student.Email,
                    Course1 = student.FirstEnrollment?.Course?.Name,
                    Course1Grade = student.FirstEnrollment?.Grade.ToString(),
                    Course1Credits = student.FirstEnrollment?.Course?.Credits,
                    Course2 = student.SecondEnrollment?.Course?.Name,
                    Course2Grade = student.SecondEnrollment?.Grade.ToString(),
                    Course2Credits = student.SecondEnrollment?.Course?.Credits,
                };
            }
        }

    }
}
