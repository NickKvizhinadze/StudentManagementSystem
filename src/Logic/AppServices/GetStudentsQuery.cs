using Logic.Dtos;
using Logic.Students;
using Logic.Utils;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data.SqlClient;

namespace Logic.AppServices
{
    public class GetStudentsQuery : IQuery<List<StudentDto>>
    {
        public GetStudentsQuery(string enrolled, int? number)
        {
            EnrolledIn = enrolled;
            NumberOfCourses = number;
        }

        public string EnrolledIn { get; }
        public int? NumberOfCourses { get; }

        public class GetStudentsQueryHandler : IQueryHandler<GetStudentsQuery, List<StudentDto>>
        {
            private readonly ConnectionString _connectionString;

            public GetStudentsQueryHandler(ConnectionString connectionString)
            {
                _connectionString = connectionString;
            }

            public List<StudentDto> Handle(GetStudentsQuery query)
            {
                string sql = @"
                   SELECT s.*, e.Grade, c.Name CourseName, c.Credits
                    FROM dbo.Student s
                        LEFT JOIN
                            (
                                SELECT e.StudentID,
                                       COUNT(*) Number
                                FROM dbo.Enrollment e
                                GROUP BY e.StudentID
                            ) t
                            ON s.StudentID = t.StudentID
                        LEFT JOIN dbo.Enrollment e
                            ON e.CourseID = s.StudentID
                        LEFT JOIN dbo.Course c
                            ON c.CourseID = e.CourseID
                    WHERE (
                              c.Name = @Course
                              OR @Course IS NULL
                          )
                          AND
                          (
                              ISNULL(t.Number, 0) = @Number
                              OR @Number IS NULL
                          )
                    ORDER BY s.StudentID ASC;";

                using (SqlConnection connection = new SqlConnection(_connectionString.Value))
                {
                    List<StudentInDb> students = connection
                        .Query<StudentInDb>(sql, new
                        {
                            Course = query.EnrolledIn,
                            Number = query.NumberOfCourses
                        })
                        .ToList();

                    var ids = students.GroupBy(x => x.StudentID).Select(x => x.Key).ToList();

                    var result = new List<StudentDto>();

                    foreach (var id in ids)
                    {
                        var data = students.Where(x => x.StudentID == id).ToList();
                        var dto = new StudentDto
                        {
                            Id = data[0].StudentID,
                            Name = data[0].Name,
                            Email = data[0].Email,
                            Course1 = data[0].CourseName,
                            Course1Credits = data[0].Credits,
                            Course1Grade = data[0].Grade?.ToString(),
                        };

                        if(data.Count > 1)
                        {
                            dto.Course2 = data[1].CourseName;
                            dto.Course2Credits = data[1].Credits;
                            dto.Course2Grade = data[1].Grade?.ToString();
                        }
                        result.Add(dto);
                    }
                    return result;
                }
            }

            private class StudentInDb
            {

                public readonly long StudentID;
                public readonly string Name;
                public readonly string Email;
                public readonly Grade? Grade;
                public readonly string CourseName;
                public readonly int? Credits;

                public StudentInDb(long studentID, string name, string email, Grade? grade, string courseName, int? credits)
                {
                    StudentID = studentID;
                    Name = name;
                    Email = email;
                    Grade = grade;
                    CourseName = courseName;
                    Credits = credits;
                }
            }
        }

    }
}
